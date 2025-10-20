using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.DashboardDataAdminFeature.GetEmployeeRegisterData;
using tas.Application.Features.DashboardDataAdminFeature.GetOnsiteEmployeesData;
using tas.Application.Features.DashboardDataAdminFeature.GetPackMealData;
using tas.Application.Features.DashboardDataAdminFeature.GetProfileChangeDepartmentData;
using tas.Application.Features.DashboardDataAdminFeature.GetSeatBlockOnsiteData;
using tas.Application.Features.DashboardDataAdminFeature.GetTransportData;
using tas.Application.Features.RequestDocumentFeature.GetDocumentListCancelled;
using tas.Application.Features.TransportFeature.GetDataRequest;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class DashboardDataAdminRepository : BaseRepository<Employee>, IDashboardDataAdminRepository
    {
        public DashboardDataAdminRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {

        }


        #region TransportData

        public async Task<GetTransportDataResponse> GetTransportData(GetTransportDataRequest request, CancellationToken cancellationToken)
        {

            var returnData = new GetTransportDataResponse();
            var transportData =await GetTransportRawData(cancellationToken);
            if (transportData.Count > 0)
            {
                var returnDataTransport = new List<GetTransportDataResponseTransport>();
                var returnDataTransportDetails = new List<GetTransportDataResponseTransportDetails>();


                var depData = transportData.Select(x => new
                {
                    Name = x.Name,
                    Cnt = x.cnt,
                    EventDate = x.ChangeDate,
                    DepartmentId = x.DepartmentId,
                    Type = x.Type
                }).ToList();


                var uniqueTravelData = depData.Select(x => x.Type).Distinct().ToList();

                foreach (var item in uniqueTravelData)
                {

                    var depTravelDataItem = depData.Where(x => x.Type == item).OrderBy(x=> x.EventDate).ToList();

                    var TypeCountSums = depTravelDataItem
                                .GroupBy(t => t.EventDate)
                                .Select(g => new {
                                    Key = g.Key,
                                    TotalCnt = g.Sum(t => t.Cnt)
                                }).Select(x => new GetTransportDataResponseTransportDate {
                                    Cnt = x.TotalCnt,
                                    Key = $"{item}-{x.Key}",
                                    Datekey = x.Key

                                }).ToList();
                    var newRecord = new GetTransportDataResponseTransport()
                    {
                        Type = item,
                        DateData = TypeCountSums
                    };


                    returnDataTransport.Add(newRecord);
                }

                var uniqueTravelDate = depData.Select(x => new { x.EventDate, x.Type }).Distinct().ToList();

                foreach (var item in uniqueTravelDate)
                {

                    var depTravelDataDateItem = depData.Where(x => x.EventDate == item.EventDate && x.Type == item.Type).Select(x=> new GetTransportDataResponseTransportDetailDate { 
                        Cnt = x.Cnt,
                        Name = x.Name
                    }).ToList();
        
                  
                    var newRecord = new GetTransportDataResponseTransportDetails()
                    {
                        key = $"{item.Type}-{item.EventDate}",
                        EventDate = item.EventDate, 
                        Data = depTravelDataDateItem
                    };


                    returnDataTransportDetails.Add(newRecord);
                }


                returnData.TransportData = returnDataTransport;
                returnData.Details = returnDataTransportDetails.OrderBy(x=> x.EventDate).ToList();

                return returnData;

            }


            return returnData;



        }

        private async Task<List<dynamic>> GetTransportRawData(CancellationToken cancellationToken)
        {
            DateTime endDate = DateTime.Today;
            DateTime startDate = DateTime.Today.AddDays(-7);
            var dataQuery = @$"SELECT Count(*) cnt, d.Name, CONVERT(varchar, ta.DateCreated, 23) ChangeDate, 'Add travel' Type, d.Id DepartmentId FROM
                        TransportAudit ta
                        LEFT JOIN Employee e ON ta.EmployeeId = e.Id AND e.Active = 1
                        left JOIN DashboardDepartmentData d ON e.DepartmentId = d.Id

                         WHERE CONVERT(varchar, ta.DateCreated, 23) >= '{startDate.ToString("yyyy-MM-dd")}' AND CONVERT(varchar, ta.DateCreated, 23) <= '{endDate.ToString("yyyy-MM-dd")}'
                                  AND (ta.UpdateSource LIKE 'Request add travel%' or ta.UpdateSource = 'Add travel profile' OR ta.UpdateSource = 'Roster executed from profile'
                                  OR ta.UpdateSource = 'Bulk roster executed'
                                  ) AND ta.Direction = 'IN'
                        GROUP BY d.Name, CONVERT(varchar, ta.DateCreated, 23), d.Id

                        union all
                        SELECT Count(*) cnt, d.Name, CONVERT(varchar, ta.DateCreated, 23) ChangeDate, 'Remove travel', d.Id DepartmentId FROM TransportAudit ta
                        LEFT JOIN Employee e ON ta.EmployeeId = e.Id AND e.Active = 1
                        left JOIN DashboardDepartmentData d ON e.DepartmentId = d.Id
                         WHERE CONVERT(varchar, ta.DateCreated, 23) >= '{startDate.ToString("yyyy-MM-dd")}' AND CONVERT(varchar, ta.DateCreated, 23) <= '{endDate.ToString("yyyy-MM-dd")}'
                                  AND (ta.UpdateSource LIKE 'Request remove travel%' or ta.UpdateSource = 'Remove travel profile'
                                  ) AND ta.Direction = 'IN'
                        GROUP BY d.Name, CONVERT(varchar, ta.DateCreated, 23), d.Id
                        union all
                        SELECT Count(*) cnt, d.Name, CONVERT(varchar, ta.DateCreated, 23) ChangeDate, 'Reschedule travel',  d.Id DepartmentId FROM TransportAudit ta
                        LEFT JOIN Employee e ON ta.EmployeeId = e.Id AND e.Active = 1
                        left JOIN DashboardDepartmentData d ON e.DepartmentId = d.Id

                         WHERE CONVERT(varchar, ta.DateCreated, 23) >= '{startDate.ToString("yyyy-MM-dd")}' AND CONVERT(varchar, ta.DateCreated, 23) <= '{endDate.ToString("yyyy-MM-dd")}'
                                  AND (ta.UpdateSource LIKE 'Request reschedule%' or ta.UpdateSource = 'Reschedule travel profile' OR ta.UpdateSource = 'Roster executed from profile'
                                  OR ta.UpdateSource = 'Reschedule from TAS'
                                  ) AND ta.Direction = 'IN'
                        GROUP BY d.Name, CONVERT(varchar, ta.DateCreated, 23), d.Id";


            try
            {
                var rawData = await GetRawQueryData<dynamic>(dataQuery, cancellationToken);
                return rawData;
            }
            catch (BadRequestException ex)
            {
                throw new BadRequestException("Failed to retrieve transport data");
            }

        }


        #endregion


        #region ProfileChanges

        public async Task<GetProfileChangeDepartmentDataResponse> GetProfileChangeDepartmentData(GetProfileChangeDepartmentDataRequest request, CancellationToken cancellationToken)
        {
            var returnData = new GetProfileChangeDepartmentDataResponse();

            var fieldData = await GetProfileChangesFieldData(request.startDate, request.endDate, cancellationToken);

            var cols = new List<string>();
            foreach (var field in fieldData)
            {
                var data = JsonConvert.DeserializeObject<List<string>>(field.AffectedColumns.ToString());

                cols.AddRange(data);

            }

            var reportColumns = new List<string> { "CostCodeId", "PeopleTypeId", "DepartmentId", "Mobile", "EmployerId", "LocationId", "PackMeal" };
            var groupedResult = cols
                .GroupBy(item => item)
                .Select(group => new { Item = group.Key, Count = group.Count() })
                .Where(x => reportColumns.Contains(x.Item))
                .ToDictionary(x => x.Item, x => x.Count);

            var dateData = reportColumns.Select(item => new GetProfileChangeEditDataDate
            {
                ColumnName = item switch
                {
                    "CostCodeId" => "CostCode",
                    "PeopleTypeId" => "PeopleType",
                    "DepartmentId" => "Department",
                    "Mobile" => "Mobile",
                    "EmployerId" => "Employer",
                    "LocationId" => "Location",
                    "PackMeal" => "PackMeal",
                    _ => item
                },
                Cnt = groupedResult.TryGetValue(item, out var count) ? count : 0
            }).ToList();

            returnData.EditFields = dateData;

            return returnData;
        }
       



        private async Task<List<dynamic>> GetProfileChangesFieldData( DateTime? start, DateTime? end, CancellationToken cancellationToken)
        {
            DateTime endDate = DateTime.Today;
            DateTime startDate = DateTime.Today.AddDays(-7);
            if (start.HasValue)
            {
                startDate = start.Value;
            }
            if (end.HasValue)
            {
                endDate = end.Value;
            }

            var dataQuery = @$"
                          SELECT 
                              ea.EmployeeId,
                              ea.AffectedColumns,
                              CONVERT(varchar, ea.DateCreated, 23) AS ChangeDate
                 
                          FROM EmployeeAudit ea 
                          LEFT JOIN Employee e ON ea.EmployeeId = e.Id
                  
                          WHERE ea.DateCreated >= '{startDate}' AND ea.DateCreated <= '{endDate}'";

                try
                {
                    var rawData = await GetRawQueryData<dynamic>(dataQuery, cancellationToken);
                    return rawData;
                }
                catch (BadRequestException ex)
                {
                    return new List<dynamic>();
                }
        }



        #endregion



        #region SeatBlockOnsiteData

        public async Task<List<GetSeatBlockOnsiteDataResponse>> GetSeatBlockOnsiteData(GetSeatBlockOnsiteDataRequest request, CancellationToken cancellationToken)
        {

            DateTime date = DateTime.Today.AddDays(-7);
            if (request.startDate.HasValue)
            {
                date = request.startDate.Value;
            }

            string dataQuery = @$" SELECT Count(*) cnt, d.Name, es.EventDate from EmployeeStatus es 
                                      left JOIN Employee e ON es.EmployeeId = e.Id
                                      LEFT JOIN PeopleType pt ON e.PeopleTypeId = pt.Id
                                      left join DashboardDepartmentData d ON e.DepartmentId = d.Id
                                    WHERE  es.EventDate >= '{date.ToString("yyyy-MM-dd")}' AND es.EventDate <= GETDATE() AND pt.Code = 'Visitor'
                                    AND es.RoomId is not NULL
                                    GROUP BY d.Name, es.EventDate";



            try
            {
                var rawData = await GetRawQueryData<dynamic>(dataQuery, cancellationToken);
                var returnData = rawData.Select(d => new GetSeatBlockOnsiteDataResponse
                {
                    DepartmentName = d.Name,
                    Cnt = d.cnt,
                    EventDate = d.EventDate,
                }).ToList();

                return returnData;
            }
            catch (BadRequestException ex)
            {
                throw new BadRequestException("Failed to retrieve seat block onsite data");
            }

        }


        #endregion


        #region OnSiteEmployeeData

        public async Task<GetOnsiteEmployeesDataResponse> GetOnsiteEmployeesData(GetOnsiteEmployeesDataRequest request, CancellationToken cancellationToken)
        {

            var returnData = new GetOnsiteEmployeesDataResponse();

            var returnDataDepartment =new List<GetOnsiteEmployeesDataDepartments>();
            var returnDataPeopleType = new List<GetOnsiteEmployeesDataPeopleType>();

            var departmentData = await GetDepartmentData(cancellationToken);
            if (departmentData.Count > 0)
            {
                var depData = departmentData.Select(x => new
                {
                    Name = x.Name,
                    Cnt = x.cnt,
                    EventDate = x.EventDate,
                    DepartmentId = x.DepartmentId
                }).ToList();

                var uniqueDepartments = depData.Where(x=> x.Name != null).Select(x => new { x.Name, x.DepartmentId }).Distinct();



                var peoplytypeData =await GetPeopTypeData(uniqueDepartments.Select(x =>x.DepartmentId).ToList(), cancellationToken);

                var peopleData = peoplytypeData.Select(x => new
                {
                    Code = x.Code,
                    Cnt = x.cnt,
                    EventDate = x.EventDate,
                    DepartmentId = x.DepartmentId
                }).ToList();

                var uniquePeopleTypes = peopleData.Select(x => new {x.EventDate, x.DepartmentId }).Distinct();

                foreach (var item in uniqueDepartments)
                {
                    var newData = new GetOnsiteEmployeesDataDepartments
                    {
                        Name = item.Name,
                        DateData = depData.Where(x => x.DepartmentId == item.DepartmentId).Select(c => new GetOnsiteEmployeesDataDepartmentsDate {
                            Cnt = c.Cnt,
                            Date = c.EventDate,
                            ChildKey = $"{c.DepartmentId}-{c.EventDate}"
                        }).ToList()
                       
                    };

                    returnDataDepartment.Add(newData);  


                }


                foreach (var item in uniquePeopleTypes)
                {

                    var currDepartment = uniqueDepartments.Where(x => x.DepartmentId == item.DepartmentId).FirstOrDefault();
                    var newData = new GetOnsiteEmployeesDataPeopleType
                    {
                        ParentKey = $"{item.DepartmentId}-{item.EventDate}",
                        Name = currDepartment?.Name,
                        DateData = peopleData.Where(x => x.DepartmentId == item.DepartmentId  && x.EventDate == item.EventDate).Select(c => new GetOnsiteEmployeesDataPeopleTypeDate
                        {
                            Cnt = c.Cnt,
                            PeopleTypeName = c.Code
                        }).ToList()

                    };

                    returnDataPeopleType.Add(newData);

                
                }

                returnData.Departments = returnDataDepartment;
                returnData.PeopleTypes = returnDataPeopleType;


            }


            return returnData;


            
        }



        public async Task<List<dynamic>> GetPeopTypeData(List<dynamic> departmentIds, CancellationToken cancellationToken)
        {
            DateTime endDate = DateTime.Today.AddDays(1);
            DateTime startDate = DateTime.Today.AddDays(-1);
            if (departmentIds.Count > 0)
            {

                List<int> intDepartmentIds = departmentIds.ConvertAll<int>(d => Convert.ToInt32(d));

                string departmentIdCondition = string.Join(",", intDepartmentIds);


                string dataQuery = @$"SELECT 
                                    COUNT(es.EmployeeId) AS cnt,
                                    es.EventDate,
                                    pt.Code,
                                    e.DepartmentId
                        FROM EmployeeStatus es
                        LEFT JOIN Employee e ON es.EmployeeId = e.Id
                        LEFT JOIN PeopleType pt ON e.PeopleTypeId = pt.Id
                        WHERE es.EventDate >= '{startDate.ToString("yyyy-MM-dd")}' AND es.EventDate <= '{endDate.ToString("yyyy-MM-dd")}' AND es.RoomId IS NOT NULL 
                        AND e.DepartmentId IN ({departmentIdCondition})
                            GROUP BY pt.code, es.EventDate, e.DepartmentId";

                try
                {
                    var rawData = await GetRawQueryData<dynamic>(dataQuery, cancellationToken);
                    return rawData;
                }
                catch (BadRequestException ex)
                {
                    return new List<dynamic>();
                }

            }
            return new List<dynamic>();
        }


        private async Task<List<dynamic>> GetDepartmentData(CancellationToken cancellationToken)
        {
            DateTime endDate = DateTime.Today.AddDays(1);
            DateTime startDate = DateTime.Today.AddDays(-1);
            var dataQuery = @$"
                        SELECT 
                            COUNT(es.EmployeeId) AS cnt,
                            d.Name,
                            es.EventDate,
                            d.ToplevelId DepartmentId
                        FROM EmployeeStatus es
                        LEFT JOIN Employee e ON es.EmployeeId = e.Id
                        LEFT JOIN DashboardDepartmentData d ON e.DepartmentId = d.Id
                        WHERE es.EventDate >= '{startDate.ToString("yyyy-MM-dd")}' AND es.EventDate <= '{endDate.ToString("yyyy-MM-dd")}' AND es.RoomId IS NOT NULL  and depId is not null
                        GROUP BY d.Name, es.EventDate, d.ToplevelId";


            try
            {
                var rawData = await GetRawQueryData<dynamic>(dataQuery, cancellationToken);
                return rawData;
            }
            catch (BadRequestException ex)
            {
                throw new BadRequestException("Failed to retrieve  data");
            }

        }

        #endregion  


        #region EmployeeRegister




        public async Task<List<GetEmployeeRegisterDataResponse>> GetEmployeeRegisterData(GetEmployeeRegisterDataRequest request, CancellationToken cancellationToken)
        {
            DateTime date = DateTime.Today.AddDays(-7);
            if (request.startDate.HasValue)
            { 
                date = request.startDate.Value;
            }


            string dataQuery = @$"SELECT Count(*) cnt , 'Add' Description from Employee e WHERE e.DateCreated >= '{date.ToString("yyyy-MM-dd")}'
                                union all
                                SELECT Count(*), 'DeActive' Description from EmployeeAudit  e WHERE e.DateCreated >= '{date.ToString("yyyy-MM-dd")}'
                                AND e.OldValues like '%""Active"":1%' AND e.NewValues like '%""Active"":0%' AND e.Type = 'Update'
                                union all
                                SELECT Count(*), 'ReActive' Description from EmployeeAudit  e WHERE e.DateCreated >= '{date.ToString("yyyy-MM-dd")}'
                                AND e.OldValues like '%""Active"":0%' AND e.NewValues like '%""Active"":1%' AND e.Type = 'Update'";


            try
            {
                var rawData = await GetRawQueryData<dynamic>(dataQuery, cancellationToken);
                var returnData = rawData.Select(d => new GetEmployeeRegisterDataResponse
                {
                    Description = d.Description,
                    Cnt = d.cnt
                }).ToList();

                return returnData;
            }
            catch (BadRequestException ex)
            {
                throw new BadRequestException("Failed to retrieve data");
            }


        }

        #endregion

        #region PackMealData


        public async Task<List<GetPackMealDataResponse>> GetPackMealData(GetPackMealDataRequest request, CancellationToken cancellationToken)
        {

            string dataQuery = @"SELECT  gd.Description, Count(gm.EmployeeId) cnt FROM GroupMembers gm
                                    left JOIN GroupDetail gd ON gm.GroupDetailId = gd.id
                                    left JOIN Employee e ON gm.EmployeeId = e.Id AND e.Active = 1
                                     WHERE gm.GroupMasterId 
                                    IN (SELECT gm.id from GroupMaster gm WHERE gm.Description = 'Pack Meal')
                                    GROUP BY gm.GroupDetailId, gd.Description
                                    ORDER BY cnt desc";



            try
            {
                var rawData = await GetRawQueryData<dynamic>(dataQuery, cancellationToken);
                var returnData = rawData.Select(d => new GetPackMealDataResponse
                {
                    Description = d.Description,
                    Cnt = d.cnt
                }).ToList();

                return returnData;
            }
            catch (BadRequestException ex)
            {
                throw new BadRequestException("Failed to retrieve pack meal data");
            }


        }

        #endregion

        #region ExecuteRawQuery
        private async Task<List<T>> GetRawQueryData<T>(string query, CancellationToken cancellationToken)
        {
            if (Context.Database.GetDbConnection() is SqlConnection sqlConnection)
            {
                try
                {
                    if (sqlConnection.State == ConnectionState.Closed)
                    {
                        await sqlConnection.OpenAsync(cancellationToken);
                    }

                    using (var command = sqlConnection.CreateCommand())
                    {
                        command.CommandText = query;
                        command.CommandTimeout = 300;

                        using (var result = await command.ExecuteReaderAsync(cancellationToken))
                        {
                            var resultList = new List<T>();
                            while (await result.ReadAsync(cancellationToken))
                            {
                                var d = (IDictionary<string, object>)new ExpandoObject();
                                for (int i = 0; i < result.FieldCount; i++)
                                {
                                    d.Add(result.GetName(i), result.IsDBNull(i) ? null : result.GetValue(i));
                                }

                                resultList.Add(JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(d)));
                            }
                            return resultList;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new BadRequestException("An error occurred while executing the query.");
                }
                finally
                {
                    if (sqlConnection.State == ConnectionState.Open)
                    {
                        await sqlConnection.CloseAsync();
                    }
                }
            }
            else
            {
                throw new BadRequestException("Database connection is not of type SqlConnection.");
            }
        }
    }
    #endregion





}