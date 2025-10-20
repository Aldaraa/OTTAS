using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.DashboardSystemAdminFeature.GeOnsiteEmployeesData;
using tas.Application.Features.DashboardSystemAdminFeature.GetCampPOBData;
using tas.Application.Features.DashboardSystemAdminFeature.GetCampUtilizationData;
using tas.Application.Features.DashboardSystemAdminFeature.GetEmployeeTransportData;
using tas.Application.Features.DashboardSystemAdminFeature.GetPeopleTypeAndDepartment;
using tas.Application.Features.DashboardSystemAdminFeature.GetStatData;
using tas.Application.Features.DashboardTransportAdminFeature.GetDomesticData;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Entities;
using tas.Persistence.Context;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace tas.Persistence.Repositories
{


    public class DashboardSystemAdminRepository : BaseRepository<Employee>, IDashboardSystemAdminRepository
    {
        public DashboardSystemAdminRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
        }

        #region GetStatData
        public async Task<List<GetStatDataResponse>> GetStatData(GetStatDataRequest request, CancellationToken cancellationToken) 
        {

           var returnData = await GetStatDataFetch(cancellationToken);

            return returnData;



        }


        private async Task<List<GetStatDataResponse>> GetStatDataFetch( CancellationToken cancellationToken)
        {
            string skippLocationIds = "1, 9";

            string query = @$"
                    SELECT Count(*) Count, 'Total Profile' Code, 'Total active profile' Description FROM Employee e
                    WHERE e.Active = 1
                    UNION all
                   SELECT COUNT(*) cnt, l.Code Code, l.Description Description FROM Employee e
                    left JOIN Location l ON e.LocationId = l.Id

                     WHERE e.Active = 1 and l.Id not in({skippLocationIds}) GROUP BY e.LocationId, l.Code, l.Description 
                     ORDER BY COUNT(*) desc
                            ";

            return await GetRawQueryData<GetStatDataResponse>(query, cancellationToken);
        }


        #endregion



        #region OnSiteDataAnalyze

        public async Task<GeOnsiteEmployeesDataResponse> GeOnsiteEmployeesData(GeOnsiteEmployeesDataRequest request, CancellationToken cancellationToken)
        {
            var returnData = new GeOnsiteEmployeesDataResponse();
            returnData.GenderData = await GeOnsiteEmployeesDataGender(request.startDate, cancellationToken);
            returnData.DepartmentData = new List<GeOnsiteEmployeesDataDepartment>(); // await GeOnsiteEmployeesDataDepartment(request.startDate, cancellationToken);
            returnData.ShiftData =await GeOnsiteEmployeesDataShift(request.startDate, cancellationToken); 
            returnData.CampData =await GeOnsiteEmployeesDataCamp(request.startDate, cancellationToken); 
            returnData.Peopletype =await GeOnsiteEmployeesDataPeopletype(request.startDate, cancellationToken); 


            return returnData;


        }



        private async Task<List<GeOnsiteEmployeesDataGender>> GeOnsiteEmployeesDataGender(DateTime? startDate, CancellationToken cancellationToken)
        {
            DateTime sDate = DateTime.Now;
            if (startDate.HasValue) { 
                sDate = startDate.Value;    
            }
            var startdatestring = sDate.ToString("yyyy");

            string query = @$" SELECT 
                                FORMAT(EventDate, 'yyyy-MM-dd') AS Date,
                                COUNT(DISTINCT EmployeeId) AS NumberOfEmployees,
                                CASE WHEN e.gender = 1 then 'Male' ELSE 'Female' end AS 'Gender'
                            FROM 
                                EmployeeStatus ES
                            JOIN 
                                Employee E ON ES.EmployeeId = E.Id
                            WHERE 
                                es.RoomId is NOT NULL AND  YEAR(EventDate) ={startdatestring}
                                AND es.EventDate < GETDATE()
                            GROUP BY 
                                 FORMAT(EventDate, 'yyyy-MM-dd'),  CASE WHEN e.gender = 1 then 'Male' ELSE 'Female' end
                     ";

            return await GetRawQueryData<GeOnsiteEmployeesDataGender>(query, cancellationToken);
        }



        private async Task<List<GeOnsiteEmployeesDataShift>> GeOnsiteEmployeesDataShift(DateTime? startDate, CancellationToken cancellationToken)
        {
            DateTime sDate = DateTime.Now;
            if (startDate.HasValue)
            {
                sDate = startDate.Value;
            }
            var startdatestring = sDate.ToString("yyyy");

            string query = @$"SELECT 
               FORMAT(EventDate, 'yyyy-MM-dd') AS Date,
                                COUNT(DISTINCT EmployeeId) AS NumberOfEmployees,
                                s.Code
                            FROM 
                                EmployeeStatus ES
                            JOIN 
                                Shift s ON ES.ShiftId =s.Id
                            WHERE 
                                es.RoomId is NOT NULL AND  YEAR(EventDate) = {startdatestring}
                                AND es.EventDate < GETDATE() AND s.Code IN ('NS', 'DS')
                            GROUP BY 
                                 FORMAT(EventDate, 'yyyy-MM-dd'), s.Code
                            ORDER BY 
                          FORMAT(EventDate, 'yyyy-MM-dd')
                                ";

            return await GetRawQueryData<GeOnsiteEmployeesDataShift>(query, cancellationToken);
        }


        private async Task<List<GeOnsiteEmployeesDataCamp>> GeOnsiteEmployeesDataCamp(DateTime? startDate, CancellationToken cancellationToken)
        {
            DateTime sDate = DateTime.Now;
            if (startDate.HasValue)
            {
                sDate = startDate.Value;
            }
            var startdatestring = sDate.ToString("yyyy");

            string query = @$"SELECT 
               FORMAT(EventDate, 'yyyy-MM-dd') AS Date,
                            COUNT(DISTINCT EmployeeId) AS NumberOfEmployees,
                            c.Description Camp
                        FROM 
                            EmployeeStatus ES
                        JOIN 
                            Room r ON ES.RoomId = r.Id
                        left JOIN Camp c ON r.CampId = c.Id 
                        WHERE 
                            es.RoomId is NOT NULL AND  YEAR(EventDate) = {startdatestring}
                            AND es.EventDate < GETDATE()
                        GROUP BY 
                            FORMAT(EventDate, 'yyyy-MM-dd'), c.Description
                            ORDER BY 
                          FORMAT(EventDate, 'yyyy-MM-dd')
                        ";

            return await GetRawQueryData<GeOnsiteEmployeesDataCamp>(query, cancellationToken);
        }


        private async Task<List<GeOnsiteEmployeesDataPeopletype>> GeOnsiteEmployeesDataPeopletype(DateTime? startDate, CancellationToken cancellationToken)
        {
            DateTime sDate = DateTime.Now;
            if (startDate.HasValue)
            {
                sDate = startDate.Value;
            }
            var startdatestring = sDate.ToString("yyyy");

            string query = @$"SELECT 
               FORMAT(EventDate, 'yyyy-MM-dd') AS Date,
                            COUNT(DISTINCT EmployeeId) AS NumberOfEmployees,
                            pt.Code
                        FROM 
                            EmployeeStatus ES
                        JOIN 
                            Employee e ON ES.EmployeeId = e.Id
                        left JOIN PeopleType pt ON e.PeopleTypeId = pt.Id
                        WHERE 
                            es.RoomId is NOT NULL AND  YEAR(EventDate) ={startdatestring}
                            AND es.EventDate < GETDATE()
                        and
                         pt.Code IN ('KHB DW', 'Expat', 'NAT')
                        OR pt.Code LIKE 'Non.S%'
                        GROUP BY 
                          FORMAT(EventDate, 'yyyy-MM-dd'), pt.Code

                        ORDER BY 
                                    FORMAT(EventDate, 'yyyy-MM-dd');
                                ";

            return await GetRawQueryData<GeOnsiteEmployeesDataPeopletype>(query, cancellationToken);
        }
        #endregion




        #region RawQuery 

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
        #endregion


        #region CampPOBData

        public async Task<List<GetCampPOBDataResponse>> GetCampPOBData(GetCampPOBDataRequest request, CancellationToken cancellationToken)
        {
            if (request.weekly)
            {
                DateTime currentDate = DateTime.Now;
                DateTime weekstartDate = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday);
                if (currentDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    weekstartDate = weekstartDate.AddDays(-7);
                }

                // Calculate the next Sunday
                DateTime weekEndDate = currentDate.AddDays(DayOfWeek.Sunday - currentDate.DayOfWeek);
                if (currentDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    weekEndDate = currentDate.AddDays(7 - (int)currentDate.DayOfWeek);
                }


                return await GetPOBData(weekstartDate, weekEndDate, cancellationToken);

            }
            else {

                DateTime currentDate = DateTime.Now;
                DateTime startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                return await GetPOBData(startDate, endDate, cancellationToken);


            }
        }


        private async Task<List<GetCampPOBDataResponse>> GetPOBData(DateTime startDate, DateTime endDate, CancellationToken cancellationToken) 
        {
            string skipCampIds = "7";

            string query = @$"SELECT 
                                    c.Description Camp, 
                                    roomData.RoomType, 
                                    AVG(roomData.cnt) AS Count 
                                FROM 
                                    Camp c
                                LEFT JOIN (
                                    SELECT 
                                        c.id campId, 
                                        rt.Description AS RoomType, 
                                        COUNT(*) AS cnt 
                                    FROM 
                                        EmployeeStatus es 
                                    LEFT JOIN 
                                        Room r ON es.RoomId = r.Id
                                    LEFT JOIN 
                                        Camp c ON r.CampId = c.Id
                                    LEFT JOIN 
                                        RoomType rt ON r.RoomTypeId = rt.Id
                                    WHERE 
                                        es.RoomId IS NOT NULL 
                                        AND es.EventDate >= '{startDate.ToString("yyyy-MM-dd")}' 
                                        AND es.EventDate <= '{endDate.ToString("yyyy-MM-dd")}'
                                    GROUP BY 
                                        c.Id, 
                                        rt.Description, 
                                        es.EventDate
                                ) roomData ON c.Id = roomData.campId
                                WHERE roomData.RoomType is not NULL and c.Id not in({skipCampIds})
                                GROUP BY 
                                    c.Description, 
                                    roomData.RoomType";

            return await GetRawQueryData<GetCampPOBDataResponse>(query, cancellationToken);
        }






        #endregion


        #region PeopleTypeAndDepartments

        public async Task<GetPeopleTypeAndDepartmentResponse> GetPeopleTypeAndDepartment(GetPeopleTypeAndDepartmentRequest request, CancellationToken cancellationToken)
        {


            var departmentData = await GetDepartmentEmployees(cancellationToken);

            var returnData = new GetPeopleTypeAndDepartmentResponse
            {
                DepartmentEmployees =await ModifyDepartmentEmployees(departmentData, cancellationToken),
                PeopleTypeEmployees = await GetPeopleTypeEmployees(cancellationToken)
            };

            return returnData;
        }


        private async Task<List<DepartmentEmployees>> ModifyDepartmentEmployees(List<DepartmentEmployees> employeeData, CancellationToken cancellationToken)
        {
            foreach (var item in employeeData)
            {
                item.PeopleTypeData = await GetDepartmentPeopleTypeEmployees(item.Id, cancellationToken);
            }

            return employeeData;    
        }

        private async Task<List<DepartmentEmployeesPeopleTypes>> GetDepartmentPeopleTypeEmployees(int DepartmentId, CancellationToken cancellationToken)
        {

            string query = @$"WITH RecursiveDepartments AS (
                            SELECT d.Id, d.ParentDepartmentId
                            FROM DashboardDepartmentData d
                            WHERE d.Id = {DepartmentId}
    
                            UNION ALL
    
                            -- Recursive member: all child departments
                            SELECT ddd.Id, ddd.ParentDepartmentId
                            FROM DashboardDepartmentData ddd
                            INNER JOIN RecursiveDepartments rd ON ddd.ParentDepartmentId = rd.Id
                        )
                        SELECT 
                            {DepartmentId} AS DepartmentId,  
                            ISNULL(pt.Code, 'Not Registered') AS Description, 
                            COUNT(*) AS Count 
                        FROM Employee e
                        LEFT JOIN PeopleType pt ON e.PeopleTypeId = pt.Id
                        WHERE e.Active = 1 
                          AND (e.DepartmentId = {DepartmentId} OR e.DepartmentId IN (SELECT Id FROM RecursiveDepartments))
                        /*  AND e.PeopleTypeId IS NOT NULL */
                        GROUP BY pt.Code
                        ORDER BY COUNT(*) DESC";

            return await GetRawQueryData<DepartmentEmployeesPeopleTypes>(query, cancellationToken);
        }


        private async Task<List<PeopleTypeEmployees>> GetPeopleTypeEmployees(CancellationToken cancellationToken)
        {

            string query = @$"SELECT ISNULL(pt.Code, 'Not Registered') Description, COUNT(*) Count FROM Employee e
                              left JOIN PeopleType pt ON e.PeopleTypeId = pt.Id
                              WHERE e.Active = 1
                              GROUP BY pt.Code
                              ORDER BY COUNT(*) desc;
                            ";

            return await GetRawQueryData<PeopleTypeEmployees>(query, cancellationToken);
        }


        private async Task<List<DepartmentEmployees>> GetDepartmentEmployees(CancellationToken cancellationToken)
        {


            string query = @$"SELECT d.TopLevelId Id, d.Name Description,  Count(*) Count FROM Employee e 
                        LEFT JOIN DashboardDepartmentData    d ON e.DepartmentId = d.Id
                        WHERE e.Active = 1 AND e.DepartmentId is NOT NULL
                        GROUP BY  d.TopLevelId, d.Name
                        ORDER BY count(*) DESC;";

            return await GetRawQueryData<DepartmentEmployees>(query, cancellationToken);
        }






        #endregion


        #region CampUtilization

        public async Task<List<GetCampUtilizationDataResponse>> GetCampUtilizationData(GetCampUtilizationDataRequest request, CancellationToken cancellationToken)
        {
            if (request.type == "Weekly")
            {
                DateTime currentDate = DateTime.Now;
                DateTime weekstartDate = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday);
                if (currentDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    weekstartDate = weekstartDate.AddDays(-7);
                }

                // Calculate the next Sunday
                DateTime weekEndDate = currentDate.AddDays(DayOfWeek.Sunday - currentDate.DayOfWeek);
                if (currentDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    weekEndDate = currentDate.AddDays(7 - (int)currentDate.DayOfWeek);
                }


                return await GetUtilizationData(weekstartDate, weekEndDate, cancellationToken);

            }
            if (request.type == "Monthly")
            {

                DateTime currentDate = DateTime.Now;
                DateTime startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                return await GetUtilizationData(startDate, endDate, cancellationToken);


            }
            else {
                return await GetUtilizationData(DateTime.Today, DateTime.Today, cancellationToken);
            }
        }

        private async Task<List<GetCampUtilizationDataResponse>> GetUtilizationData(DateTime startDate, DateTime endDate, CancellationToken cancellationToken) 
        {
            string query = @$"SELECT c.Id CampId, c.Description Camp, RoomData.RoomCount, bedData.BedCount, CapacityData.averageEmployeeCount Occup, (CapacityData.averageEmployeeCount * 100) / bedData.BedCount Utilization from Camp c 
                              left JOIN (SELECT Count(*) RoomCount, r.CampId FROM Room r 
                                  GROUP BY r.CampId
                                  ) RoomData
                             on c.Id = RoomData.CampId

                              left JOIN (SELECT sum(r.BedCount) BedCount, r.CampId FROM Room r 
                                  GROUP BY r.CampId
                                  ) bedData
                             on c.Id = bedData.CampId

                              LEFT JOIN ( SELECT 
                                c.Id AS campId, 
                                AVG(empCount) AS averageEmployeeCount
                            FROM (
                                SELECT 
                                    r1.CampId AS campId, 
                                    COUNT(*) AS empCount
                                FROM 
                                    EmployeeStatus es 
                                LEFT JOIN 
                                    Room r1 ON es.RoomId = r1.Id
                                WHERE 
                                    es.EventDate >= '{startDate.ToString("yyyy-MM-dd")}' 
                                    AND es.EventDate <= '{endDate.ToString("yyyy-MM-dd")}' 
                                    AND es.RoomId IS NOT NULL
                                GROUP BY 
                                    r1.CampId, 
                                    es.EventDate
                            ) AS dailyCounts
                            LEFT JOIN Camp c ON dailyCounts.campId = c.Id
                            GROUP BY 
                                c.Id) CapacityData
                                ON c.Id = CapacityData.campId 
                                WHERE c.Id NOT IN (7)";

            return await GetRawQueryData<GetCampUtilizationDataResponse>(query, cancellationToken);

        }


        #endregion





        public async Task<GetEmployeeTransportDataResponse> GetEmployeeTransportData(GetEmployeeTransportDataRequest request, CancellationToken cancellationToken)
        {
            DateTime currentDate = request.CurrentDate;
            DateTime weekstartDate = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday);
            var returnData = new GetEmployeeTransportDataResponse();

            if (currentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                weekstartDate = weekstartDate.AddDays(-7);
            }

            // Calculate the next Sunday
            DateTime weekEndDate = currentDate.AddDays(DayOfWeek.Sunday - currentDate.DayOfWeek);
            if (currentDate.DayOfWeek != DayOfWeek.Sunday)
            {
                weekEndDate = currentDate.AddDays(7 - (int)currentDate.DayOfWeek);
            }

            var validDirections = new[] { "IN", "OUT" };


            //var transportdata =await(from transport in Context.Transport.AsNoTracking().Where(x => x.EventDate.Value.Date >= weekstartDate && x.EventDate <= weekEndDate && validDirections.Contains(x.Direction))
                                

            //                ).ToListAsync();

            var transportdata = await Context.Transport.AsNoTracking()
                .Where(x => x.EventDate.Value.Date >= weekstartDate && x.EventDate <= weekEndDate && validDirections.Contains(x.Direction))
                .ToListAsync();


            var startDate = weekstartDate;
            returnData.InData = new List<GetEmployeeTransportDataDirectionData>();
            returnData.OutData = new List<GetEmployeeTransportDataDirectionData>();

            while (startDate <= weekEndDate)
            {

                var inDataTask =await GetDirectionDateData(transportdata, startDate.Date, "IN");
                var outDataTask =await GetDirectionDateData(transportdata, startDate.Date, "OUT");

                returnData.InData.Add(inDataTask);
                returnData.OutData.Add(outDataTask);

                startDate = startDate.AddDays(1);
            }

            return returnData;





        }


        private async Task<GetEmployeeTransportDataDirectionData> GetDirectionDateData(List<Transport> transportData, DateTime eventDate, string direction) 
        {
            var empcount = transportData.Where(x => x.Direction == direction && x.EventDate.Value.Date == eventDate.Date).Count();
            var returnData = new GetEmployeeTransportDataDirectionData
            {
                Count = empcount,
                Direction = direction,
                EventDate = eventDate,
                Drilldown = $"{direction}-{eventDate.ToString("yyyy-MM-dd")}"
            };

            returnData.LocationData = await GetLocationDateData(transportData, eventDate.Date, direction);

            return returnData;
        }


        private async Task<List<GetEmployeeTransportDataCommutBase>> GetLocationDateData(List<Transport> transportData, DateTime eventDate, string direction)
        {
            var empIds = transportData.Where(x => x.Direction == direction && x.EventDate.Value.Date == eventDate.Date).Select(x => x.EmployeeId).ToList();

            var locationData = await (from employee in Context.Employee.AsNoTracking()
                                      where empIds.Contains(employee.Id)
                                      join location in Context.Location.AsNoTracking()
                                      on employee.LocationId equals location.Id
                                      group employee by new { location.Description, location.Id } into locationGroup
                                      select new GetEmployeeTransportDataCommutBase
                                      {
                                          Id = $"{direction}-{eventDate.ToString("yyyy-MM-dd")}",
                                          EventDate = eventDate,
                                          Location = locationGroup.Key.Description,
                                          LocationId = locationGroup.Key.Id,
                                          
                                          Drilldown = $"{direction}-{eventDate.ToString("yyyy-MM-dd")}-{locationGroup.Key.Description}",
                                          Direction = direction,
                                          Count = locationGroup.Count()
                                      }).ToListAsync();

            foreach (var emp in locationData)
            {
                emp.StateData = await GetStateDateData(transportData, eventDate.Date, direction, emp.LocationId.Value, emp.Drilldown);

            }

            return locationData;


        }


        private async Task<List<GetEmployeeTransportDataState>> GetStateDateData(List<Transport> transportData, DateTime eventDate, string direction, int locationId, string drilldown)
        {
            var empIds = transportData.Where(x => x.Direction == direction && x.EventDate.Value.Date == eventDate.Date).Select(x => x.EmployeeId).ToList();

            var statedata = await (from employee in Context.Employee.AsNoTracking().Where(x=> x.LocationId == locationId)
                                      where empIds.Contains(employee.Id)
                                      join state in Context.State.AsNoTracking()
                                      on employee.StateId equals state.Id
                                      group employee by state.Description into stateGroup
                                      select new GetEmployeeTransportDataState
                                      {
                                          Id =drilldown,
                                          EventDate = eventDate,
                                          StateDescr = stateGroup.Key,
                                          Count = stateGroup.Count()
                                      }).ToListAsync();

            return statedata;


        }
    }

}