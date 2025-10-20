using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tas.Application.Features.TransportFeature.TransportBookingInfo;
using tas.Domain.Entities;

namespace tas.Persistence.Repositories
{
    public partial class TransportRepository
    {
        public async Task<List<TransportBookingInfoResponse>> TransportBookingInfo(TransportBookingInfoRequest request, CancellationToken cancellationToken)
        {
            var empIds = new List<int>();
            var startDate = request.startDate;
            var endDate = request.endDate;
            var searchEmpIds = new List<int>();


            IQueryable<Employee> empFilter = Context.Employee.AsNoTracking().Where(x => x.Active == 1);


            if (!string.IsNullOrWhiteSpace(request.empIds))
            {

                string numericInput = Regex.Replace(request.empIds, "[^0-9]+", ",");
                if (!string.IsNullOrWhiteSpace(numericInput))
                {
                    var Ids = numericInput.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(int.Parse)
                      .ToList();


                    empFilter = empFilter.AsNoTracking().Where(x => Ids.Contains(x.Id));
                    //if (Ids.Count > 0)
                    //{
                    //    searchEmpIds =Ids;
                    //}

                }
            }
            if (!string.IsNullOrWhiteSpace(request.sapids))
            {
                string numericInput = Regex.Replace(request.sapids, "[^0-9]+", ",");
                if (!string.IsNullOrWhiteSpace(numericInput))
                {
                    var sapIds = numericInput.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(int.Parse)
                      .ToList();

                    if (sapIds != null)
                    {
                        if (sapIds.Count > 0)
                        {
                            //   var sapEmpids = await Context.Employee
                            //       .AsNoTracking()
                            //       .Where(x => sapIds.Contains(x.SAPID.Value) && searchEmpIds.Contains(x.Id))
                            //       .Select(x => x.Id)
                            //       .ToListAsync(cancellationToken);
                            //   if (sapEmpids.Count > 0)
                            //   { 
                            ////      empIds.AddRange(sapEmpids);
                            //      searchEmpIds= sapEmpids;
                            //   }

                            empFilter = empFilter.AsNoTracking().Where(x => sapIds.Contains(x.SAPID.Value));
                        }
                    }


                }
            }

            if (request.EmployerId.HasValue)
            {
                //var searchEmpidDatas = await Context.Employee
                //        .AsNoTracking()
                //        .Where(x => x.EmployerId == request.EmployerId && searchEmpIds.Contains(x.Id))
                //        .Select(x => x.Id)
                //        .ToListAsync(cancellationToken);


                empFilter = empFilter.AsNoTracking().Where(x => x.EmployerId ==request.EmployerId.Value);
                //if (searchEmpidDatas.Count > 0)
                //{
                //   // empIds.AddRange(searchEmpids);
                //    searchEmpIds = searchEmpidDatas;
                //}
            }


            if (request.DepartmentId.HasValue)
            {
                //var searchEmpidDatas = await Context.Employee
                //        .AsNoTracking()
                //        .Where(x => x.DepartmentId == request.DepartmentId && searchEmpIds.Contains(x.Id))
                //        .Select(x => x.Id)
                //        .ToListAsync(cancellationToken);

                empFilter = empFilter.AsNoTracking().Where(x => x.DepartmentId == request.DepartmentId.Value);

                //if (searchEmpidDatas.Count > 0)
                //{
                //    //empIds.AddRange(searchEmpids);
                //    empIds = searchEmpidDatas;
                //}
            }


            empIds = await empFilter.Select(x => x.Id).ToListAsync();




            if (empIds.Count > 0)
            {
                var transportData = await (from transport in Context.Transport.AsNoTracking().Where(x => x.EventDate.Value.Date >= startDate && x.EventDate <= endDate.Date && empIds.Contains(x.EmployeeId.Value))
                                           join employee in Context.Employee.AsNoTracking() on transport.EmployeeId equals employee.Id
                                           join department in Context.Department.AsNoTracking() on employee.DepartmentId equals department.Id into departmentData
                                           from department in departmentData.DefaultIfEmpty()
                                           join employer in Context.Employer.AsNoTracking() on employee.EmployerId equals employer.Id into employerData
                                           from employer in employerData.DefaultIfEmpty()
                                           join location in Context.Location.AsNoTracking() on employee.LocationId equals location.Id into locationData
                                           from location in locationData.DefaultIfEmpty()
                                           join activetransport in Context.ActiveTransport.AsNoTracking() on transport.ActiveTransportId equals activetransport.Id into activetransportData
                                           from activetransport in activetransportData.DefaultIfEmpty()
                                           join transportschedule in Context.TransportSchedule.AsNoTracking() on transport.ScheduleId equals transportschedule.Id into transportscheduleData
                                           from transportschedule in transportscheduleData.DefaultIfEmpty()
                                           select new
                                           {
                                               TransportId = transport.Id,
                                               EmployeeId = employee.Id,
                                               Fullname =
                                                   (employee.SAPID != null ? employee.SAPID.ToString() : string.Empty) +
                                                   (!string.IsNullOrWhiteSpace(employee.Firstname) || !string.IsNullOrWhiteSpace(employee.Lastname) ?
                                                   " | " + (employee.Firstname ?? string.Empty) + " " + (employee.Lastname ?? string.Empty) : string.Empty) +
                                                   (!string.IsNullOrWhiteSpace(employer.Description) ? " | " + employer.Description : string.Empty) +
                                                   (!string.IsNullOrWhiteSpace(department.Name) ? " | " + department.Name : string.Empty) +
                                                   (!string.IsNullOrWhiteSpace(location.Description) ? " | " + location.Description : string.Empty),
                                               Direction = transport.Direction,
                                               Description = transportschedule.Description,
                                               TransportCode = activetransport.Code,
                                               EventDate = transport.EventDate // Do not format it here
                                           })
                           .OrderBy(x => x.EventDate)
                           .Take(300)
                           .ToListAsync(cancellationToken);

                var formattedTransportData = transportData.Select(x => new TransportBookingInfoResponse
                {
                    TransportId = x.TransportId,
                    EmployeeId = x.EmployeeId,
                    Fullname = x.Fullname,
                    Direction = x.Direction,
                    Description = x.Description,
                    TransportCode = x.TransportCode,
                    EventDate = x.EventDate.HasValue ? x.EventDate.Value.ToString("yyyy-MM-dd") : string.Empty // Format in-memory
                }).OrderBy(x => x.EventDate).ToList();
                return formattedTransportData;

            }
            else {
                return new List<TransportBookingInfoResponse>();
            }


        }
    }
}
