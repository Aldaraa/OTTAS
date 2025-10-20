using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.DashboardAccomAdminFeature.GetCampInfo;
using tas.Application.Features.DashboardTransportAdminFeature.GetDomesticData;
using tas.Application.Features.DashboardTransportAdminFeature.GetInternationalTravelData;
using tas.Application.Features.DashboardTransportAdminFeature.GetRosterData;
using tas.Application.Features.DashboardTransportAdminFeature.GetTransportGroupData;
using tas.Application.Features.DashboardTransportAdminFeature.GetTransportGroupEmployeeData;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Entities;
using tas.Persistence.Context;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace tas.Persistence.Repositories
{
    public class DashboardTransportAdminRepository : BaseRepository<Employee>, IDashboardTransportAdminRepository
    {
        public DashboardTransportAdminRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {

        }


        #region DomesticData

        public async Task<GetDomesticDataResponse> GetDomesticData(GetDomesticDataRequest request, CancellationToken cancellationToken) 
        {
            var returnData = new GetDomesticDataResponse();
            returnData.AircraftData =await GetDomesticAircraft(request.startDate, cancellationToken);
            returnData.Document = await GetDomesticDocument(request.startDate, cancellationToken);
            returnData.DateTransport =await GetDomesticDateTransport(request.startDate, cancellationToken);
            returnData.WeekUtil = await GetDomesticWeekUtil(request.startDate, cancellationToken);
            returnData.NoShow =await GetDomesticNoShow(request.startDate, cancellationToken);




            return returnData;

        }



        private async Task<List<GetDomesticNoShow>> GetDomesticNoShow(DateTime? currentDate, CancellationToken cancellationToken)
        {
            var startdatestring = DateHelper.GetWeekBoundaries(currentDate).PreviousMonday.ToString("yyyy-MM-dd");
            var enddatestring = DateHelper.GetWeekBoundaries(currentDate).NextSunday.ToString("yyyy-MM-dd");

            string query = @$"SELECT DATEPART(WEEK, EventDate) AS WeekNumber, count(*) AS Count, 'GoShow' Type from TransportGoShow tgs
                            WHERE EventDate BETWEEN '{startdatestring}' AND '{enddatestring}'
                                                        GROUP BY
                                                            DATEPART(WEEK, EventDate)
                                                            union ALL
                            SELECT DATEPART(WEEK, EventDate) AS WeekNumber, count(*) AS Count, 'NoShow' Type from TransportNoShow tgs
                            WHERE EventDate BETWEEN '{startdatestring}' AND '{enddatestring}'
                                                        GROUP BY
                                                            DATEPART(WEEK, EventDate)";

            return await GetRawQueryData<GetDomesticNoShow>(query, cancellationToken);
        }


        private async Task<List<GetDomesticWeekUtil>> GetDomesticWeekUtil(DateTime? currentDate, CancellationToken cancellationToken)
        {

           
            var startdatestring = DateHelper.GetWeekBoundaries(currentDate).PreviousMonday.ToString("yyyy-MM-dd");
            var enddatestring = DateHelper.GetWeekBoundaries(currentDate).NextSunday.ToString("yyyy-MM-dd");
            string query = @$"  SELECT COUNT(*) AS Passengers,
                                 c.Description AS Carrier
                          FROM Transport t
                          INNER JOIN ActiveTransport at ON t.ActiveTransportId = at.Id
                          INNER JOIN Carrier c ON at.CarrierId = c.Id
                          INNER JOIN TransportMode tm ON at.TransportModeId = tm.Id AND tm.Code = 'Airplane'
  
                          WHERE t.EventDateTime BETWEEN '{startdatestring}' AND '{enddatestring}'
                          GROUP BY c.Description";

            return await GetRawQueryData<GetDomesticWeekUtil>(query, cancellationToken);
        }

        //GetDomesticWeekUtil


        private async Task<GetDomesticAircraft> GetDomesticAircraft(DateTime? currentDate, CancellationToken cancellationToken)
        {
            var startdatestring = DateHelper.GetWeekBoundaries(currentDate).PreviousMonday.ToString("yyyy-MM-dd");
            var enddatestring = DateHelper.GetWeekBoundaries(currentDate).NextSunday.ToString("yyyy-MM-dd");


            var returnData =new GetDomesticAircraft();

            returnData.Airplane = await GetAircraftData("Airplane", startdatestring, enddatestring, cancellationToken);
            returnData.Bus = await GetAircraftData("Bus", startdatestring, enddatestring, cancellationToken);
            returnData.Drive = await GetAircraftData("Drive", startdatestring, enddatestring, cancellationToken);

            return returnData;

        }



        public async Task<List<GetDomesticAircraftWeekInfo>> GetDomesticAircraftWeekInfo(string startDateString, string endDateString, int scheduleId, string direction, string transportMode, CancellationToken cancellationToken)
        {
            var query = @$"SELECT
                              ISNULL(at.AircraftCode, 'Not Registered') Code, 
                              COUNT(*) AS Count, 
                              DATENAME(weekday, ts.EventDate) AS DayName,
                              ts.Direction
                            FROM Transport ts
                            LEFT JOIN ActiveTransport at 
                              ON ts.ActiveTransportId = at.Id
                            inner JOIN TransportMode tm ON at.TransportModeId = tm.Id  AND tm.Code = '{transportMode}'
                            WHERE ts.EventDate >= '{startDateString}' 
                              AND ts.EventDate <= '{endDateString}'
                              and ts.Direction = '{direction}'
                              AND ts.ScheduleId = {scheduleId}
                            GROUP BY at.AircraftCode, 
                                     ts.ScheduleId,
                                     ts.Direction,
                                     DATENAME(weekday, ts.EventDate);
                            "
            ;

            return await GetRawQueryData<GetDomesticAircraftWeekInfo>(query, cancellationToken);
        }


        public async Task<List<GetDomesticAircraftData>> GetAircraftData(string transportMode, string startDateString, string endDateString, CancellationToken cancellationToken)
        {
            var query = $@" SELECT 
                        ISNULL(at.AircraftCode, 'Not Registered') Code, 
                        ts.Id ScheduleId
                     FROM TransportSchedule ts 
                     inner JOIN (
                    SELECT t.ScheduleId FROM Transport t WHERE t.EventDate >= '{startDateString}' and t.EventDate <= '{endDateString}'
                    GROUP BY t.ScheduleId) tt 
                    ON ts.id = tt.ScheduleId
                     LEFT JOIN ActiveTransport at 
                       ON ts.ActiveTransportId = at.Id
                     inner JOIN TransportMode tm ON at.TransportModeId = tm.Id  AND tm.Code = '{transportMode}'
                     WHERE ts.EventDate >= '{startDateString}' 
                       AND ts.EventDate <= '{endDateString}'
                       and at.Direction IN ('IN', 'OUT')";

            var craftData = await GetRawQueryData<GetDomesticAircraftData>(query, cancellationToken);

            foreach (var item in craftData)
            {
                item.WeekINInfo = await GetDomesticAircraftWeekInfo(startDateString, endDateString, item.ScheduleId.Value, "IN", transportMode, cancellationToken);
                item.WeekOUTInfo = await GetDomesticAircraftWeekInfo(startDateString, endDateString, item.ScheduleId.Value, "OUT", transportMode, cancellationToken);

            }

            return craftData;
        }





        private async Task<List<GetDomesticDateTransport>> GetDomesticDateTransport(DateTime? currentDate, CancellationToken cancellationToken)
        {
            
                var startdatestring = DateHelper.GetWeekBoundaries(currentDate).PreviousMonday.ToString("yyyy-MM-dd");
                var enddatestring = DateHelper.GetWeekBoundaries(currentDate).NextSunday.ToString("yyyy-MM-dd");

                string query = @$"SELECT  ISNULL(at.AircraftCode, 'Not Registered') Code, 
                               SUM(CASE WHEN at.Direction = 'IN' THEN 1 ELSE 0 END) AS 'IN',
                               SUM(CASE WHEN at.Direction = 'OUT' THEN 1 ELSE 0 END) AS 'OUT'
                        FROM TransportSchedule ts
                        LEFT JOIN ActiveTransport at ON ts.ActiveTransportId = at.Id
                        LEFT JOIN Transport t ON t.ScheduleId = ts.id
                        WHERE CONVERT(DATE, ts.EventDate) = CONVERT(DATE, GETDATE())
                        AND at.Direction IN ('IN', 'OUT')
                        GROUP BY at.AircraftCode
                        ORDER BY 'IN' DESC, 'OUT' desc";

            return await GetRawQueryData<GetDomesticDateTransport>(query, cancellationToken);
        }



        private async Task<List<GetDomesticDocument>> GetDomesticDocument(DateTime? currentDate,  CancellationToken cancellationToken)
        {
            var startdatestring = DateHelper.GetWeekBoundaries(currentDate).PreviousMonday.ToString("yyyy-MM-dd");
            var enddatestring = DateHelper.GetWeekBoundaries(currentDate).NextSunday.ToString("yyyy-MM-dd");

            string query = @$"SELECT Count(*) Count, rdh.CurrentAction Action  from RequestDocumentHistory rdh WHERE rdh.DocumentId IN (select Id FROM RequestDocument rd
                         WHERE rd.DocumentType = 'Site Travel')
                         AND rdh.CurrentAction IN ('Completed', 'Approved', 'Declined')
                        AND rdh.DateCreated >= '{startdatestring}' AND rdh.DateCreated <= '{enddatestring}' 
                        GROUP BY rdh.CurrentAction";

            return await GetRawQueryData<GetDomesticDocument>(query, cancellationToken);
        }






        #endregion


        #region InternationalTravel

        public async Task<GetInternationalTravelDataResponse> GetInternationalTravelData(GetInternationalTravelDataRequest request, CancellationToken cancellationToken)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = startDate.AddDays(30);


            if (request.startDate.HasValue)
            {
                startDate = request.startDate.Value;
            }

            if (request.endDate.HasValue)
            {
                endDate = request.endDate.Value;
            }

            var returnData = new GetInternationalTravelDataResponse();
            returnData.TravelPurpose = await GetInternationalTravelDataTravelPurpose(cancellationToken, startDate, endDate);
            returnData.TravelAgent = await GetInternationalTravelDataTravelAgent(cancellationToken, startDate, endDate);
            returnData.Hotel = await GetInternationalTravelDataTravelHotel(cancellationToken, startDate, endDate);
            returnData.Documents = await GetInternationalTravelDataDocument(cancellationToken, startDate, endDate);





            return returnData;
        }

        private async Task<List<GetInternationalTravelDataTravelPurpose>> GetInternationalTravelDataTravelPurpose( CancellationToken cancellationToken, DateTime startDate, DateTime endDate)
        {
            var startdatestring = startDate.ToString("yyyy-MM-dd");
            var enddatestring = endDate.ToString("yyyy-MM-dd");

            string query = @$"SELECT rtp.Description TravelPurpose, COUNT(*) count FROM RequestNonSiteTravel rnst
                            left join RequestTravelPurpose rtp
                            ON rnst.RequestTravelPurposeId = rtp.Id
                            WHERE rnst.DocumentId IN (SELECT id from RequestDocument rd WHERE rd.CurrentAction = 'Completed')
                            AND rnst.DateCreated >= '{startdatestring}' AND rnst.DateCreated <= '{enddatestring}'
                            GROUP BY rtp.Description";

            return await GetRawQueryData<GetInternationalTravelDataTravelPurpose>(query, cancellationToken);
        }


        private async Task<List<GetInternationalTravelDataTravelAgent>> GetInternationalTravelDataTravelAgent(CancellationToken cancellationToken, DateTime startDate, DateTime endDate)
        {
            var startdatestring = startDate.ToString("yyyy-MM-dd");
            var enddatestring = endDate.ToString("yyyy-MM-dd");

            string query = $@"SELECT rta.Description Agent, COUNT(*) Count FROM RequestNonSiteTravel rnst
                            left join RequestTravelAgent rta ON rnst.RequestTravelAgentId = rta.Id
                            WHERE rnst.DocumentId IN (SELECT id from RequestDocument rd WHERE rd.CurrentAction = 'Completed')
                            AND rnst.DateCreated >= '{startdatestring}' AND rnst.DateCreated <= '{enddatestring}'
                            GROUP BY rta.Description";

            return await GetRawQueryData<GetInternationalTravelDataTravelAgent>(query, cancellationToken);
        }


        private async Task<List<GetInternationalTravelDataHotel>> GetInternationalTravelDataTravelHotel(CancellationToken cancellationToken, DateTime startDate, DateTime endDate)
        {
            var startdatestring = startDate.ToString("yyyy-MM-dd");
            var enddatestring = endDate.ToString("yyyy-MM-dd");

            string query = @$"SELECT rnst.Hotel, COUNT(*) Count FROM RequestNonSiteTravelAccommodation  rnst
                                WHERE rnst.DocumentId IN (SELECT id from RequestDocument rd WHERE rd.CurrentAction = 'Completed')
                                AND rnst.DateCreated >= '{startdatestring}' AND rnst.DateCreated <= '{enddatestring}'
                                GROUP BY rnst.Hotel";

            return await GetRawQueryData<GetInternationalTravelDataHotel>(query, cancellationToken);
        }


        private async Task<List<GetInternationalTravelDataDocument>> GetInternationalTravelDataDocument(CancellationToken cancellationToken, DateTime startDate, DateTime endDate)
        {
            var startdatestring = startDate.ToString("yyyy-MM-dd");
            var enddatestring = endDate.ToString("yyyy-MM-dd");

            string query = @$"SELECT 
                                Count(*) Count, 
                                CASE 
                                    WHEN rdh.CurrentAction = 'Approved' THEN 'Pending'
                                    ELSE rdh.CurrentAction 
                                END AS Action
                            FROM RequestDocumentHistory rdh
                            WHERE rdh.DocumentId IN (
                                SELECT Id 
                                FROM RequestDocument rd
                                WHERE rd.DocumentType = 'Non Site Travel'
                            )
                            AND rdh.CurrentAction IN ('Completed', 'Approved', 'Declined')
                            AND rdh.DateCreated >= '{startdatestring}' 
                            AND rdh.DateCreated <= '{enddatestring}'
                            GROUP BY 
                                CASE 
                                    WHEN rdh.CurrentAction = 'Approved' THEN 'Pending'
                                    ELSE rdh.CurrentAction 
                                END;";

            return await GetRawQueryData<GetInternationalTravelDataDocument>(query, cancellationToken);
        }


        #endregion



        #region RosterData

        public async Task<GetRosterDataResponse> GetRosterData(GetRosterDataRequest request, CancellationToken cancellationToken) 
        {

            var returnData = new GetRosterDataResponse();
            DateTime startDate = DateTime.Today.AddDays(-7);
            DateTime endDate = DateTime.Today;


            switch (request.routeType)
            {
                case "WEEKLY":
                   startDate = DateHelper.GetWeekBoundaries(request.currentDate).PreviousMonday;
                   endDate = DateHelper.GetWeekBoundaries(request.currentDate).NextSunday;
                    break;
                case "MONTHLY":
                    startDate = DateHelper.GetMonthBoundaries(request.currentDate).FirstDay;
                    endDate = DateHelper.GetMonthBoundaries(request.currentDate).LastDay;
                    break;
                case "QUARTLY":
                    startDate = DateHelper.GetQuarterlyBoundaries(request.currentDate).FirstDay;
                    endDate = DateHelper.GetQuarterlyBoundaries(request.currentDate).LastDay;
                    break;
                case "YEARLY":
                    startDate = DateHelper.GetYearBoundaries(request.currentDate).FirstDay;
                    endDate = DateHelper.GetYearBoundaries(request.currentDate).LastDay;
                    break;
                default:
                    break;
            }



            var  RosterData = await GetRosterGroupData(startDate, endDate, cancellationToken);

            foreach (var item in RosterData)
            {
                item.Details = await GetRosterDetailData(item.Drilldown, startDate, endDate, cancellationToken);


            }

            returnData.RosterData = RosterData;
          //  returnData.FlightGroupData =await GetFlightGroupData(startDate, endDate, request.departmentIds, cancellationToken);

            return returnData;


        }


        private async Task<List<GetTransportGroupDataResponse>> GetFlightGroupData(DateTime startDate, DateTime endDate, List<int>depIds,  CancellationToken cancellationToken)
        {
            var startdatestring = startDate.ToString("yyyy-MM-dd");
            var enddatestring = endDate.ToString("yyyy-MM-dd");

            if (depIds.Count == 0)
            {
                string query = @$"SELECT 
                           ISNULL(r.Id, 0) Id,
                        COUNT(*) AS Count,
                       isnull(r.Description, 'Not registered') Description,
                        DATENAME(WEEKDAY, t.EventDate) DayName
                    FROM 
                        Transport t
                    LEFT JOIN 
                        Employee e ON t.EmployeeId = e.Id
                    LEFT JOIN 
                        FlightGroupMaster  r ON e.FlightGroupMasterId = r.Id
                    WHERE 
                        t.EventDate >= '{startdatestring}' 
                        AND t.EventDate <= '{enddatestring}'
                        AND t.Direction = 'IN'
                    GROUP BY 
                         ISNULL(r.Description, 'Not registered'),
                    DATENAME(WEEKDAY, t.EventDate),
                        ISNULL(r.Id, 0)";
                return await GetRawQueryData<GetTransportGroupDataResponse>(query, cancellationToken);
            }
            else {

                var childDepartmemntIds  = await DepartmentHierarchyIds(depIds);

                string departmentParam = string.Join(", ", childDepartmemntIds);

                string query = @$"SELECT 
                           ISNULL(r.Id, 0) Id,
                        COUNT(*) AS Count,
                       isnull(r.Description, 'Not registered') Description,
                        DATENAME(WEEKDAY, t.EventDate) DayName
                    FROM 
                        Transport t
                    LEFT JOIN 
                        Employee e ON t.EmployeeId = e.Id
                    LEFT JOIN 
                        FlightGroupMaster  r ON e.FlightGroupMasterId = r.Id
                    WHERE 
                        t.EventDate >= '{startdatestring}' 
                        AND t.EventDate <= '{enddatestring}'
                        AND t.Direction = 'IN'
                        AND e.DepartmentId IN ({departmentParam})
                    GROUP BY 
                         ISNULL(r.Description, 'Not registered'),
                    DATENAME(WEEKDAY, t.EventDate),
                        ISNULL(r.Id, 0)";
                return await GetRawQueryData<GetTransportGroupDataResponse>(query, cancellationToken);
            }

        }


        private async Task<List<GetRosterGroupData>> GetRosterGroupData(DateTime startDate, DateTime endDate,  CancellationToken cancellationToken)
        {
            var startdatestring = startDate.ToString("yyyy-MM-dd");
            var enddatestring = endDate.ToString("yyyy-MM-dd");

            string query = @$"
                    SELECT 
                        COUNT(*) AS Count,
                        rg.Description,
                        rg.id 'Drilldown'
                    FROM 
                        Transport t
                    LEFT JOIN 
                        Employee e ON t.EmployeeId = e.Id
                    LEFT JOIN 
                        Roster r ON e.RosterId = r.Id
                    left JOIN RosterGroup rg ON r.RosterGroupId = rg.Id
                    WHERE 
                        t.EventDate >= '{startdatestring}' 
                        AND t.EventDate <= '{enddatestring}'
                        AND t.Direction = 'IN'
                        AND r.Name IS NOT NULL
                    GROUP BY 
                        rg.Description, rg.id";

            return await GetRawQueryData<GetRosterGroupData>(query, cancellationToken);
        }

        private async Task<List<GetRosterGroupDetail>> GetRosterDetailData(int? RosterGroupId, DateTime starDate, DateTime endDate, CancellationToken cancellationToken)
        {
            var startdatestring = starDate.ToString("yyyy-MM-dd");
            var enddatestring = endDate.ToString("yyyy-MM-dd");

            string query = @$"SELECT COUNT(*) AS Count,
                        r.RosterGroupId Id,    
                        r.name
                    FROM 
                        Transport t
                    LEFT JOIN 
                        Employee e ON t.EmployeeId = e.Id
                    LEFT JOIN 
                        Roster r ON e.RosterId = r.Id AND r.RosterGroupId = {RosterGroupId}
                    WHERE 
                        t.EventDate >= '{startdatestring}' 
                        AND t.EventDate <= '{enddatestring}'
                        AND t.Direction = 'IN'
                        AND r.Name IS NOT NULL
                    GROUP BY 
                        r.name, r.RosterGroupId";

            return await GetRawQueryData<GetRosterGroupDetail>(query, cancellationToken);




        }

        private async Task<List<int>> DepartmentHierarchyIds(List<int> departmentIds)
        {


            return await GetRawQueryData(GetDepartmentHierarchyQuery(departmentIds));



        }


        private async Task<List<int>> GetRawQueryData(string query)
        {
            if (Context.Database.GetDbConnection() is SqlConnection sqlConnection)
            {
                try
                {
                    if (sqlConnection.State == ConnectionState.Closed)
                    {
                        await sqlConnection.OpenAsync();
                    }

                    using (var command = sqlConnection.CreateCommand())
                    {
                        command.CommandText = query;
                        command.CommandTimeout = 300;

                        using (var result = await command.ExecuteReaderAsync())
                        {
                            var resultList = new List<int>();
                            while (await result.ReadAsync())
                            {
                                // Assuming the integer value is in the first column
                                if (!result.IsDBNull(0))
                                {
                                    resultList.Add(result.GetInt32(0));
                                }
                            }
                            return resultList;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new BadRequestException("data load failed");
                }
                catch (Exception ex)
                {
                    throw new BadRequestException("data load failed");
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



    #region TransportGroupData

    public async Task<List<GetTransportGroupDataResponse>> GetTransportGroupData(GetTransportGroupDataRequest request, CancellationToken cancellationToken)
        {
            var returnData = new List<GetTransportGroupDataResponse>();
            DateTime startDate = DateTime.Today.AddDays(-7);
            DateTime endDate = DateTime.Today;


            if (request.currentDate.HasValue)
            {
                startDate = DateHelper.GetWeekBoundaries(request.currentDate).PreviousMonday;
                endDate = DateHelper.GetWeekBoundaries(request.currentDate).NextSunday;
            }
            else {
                startDate = DateHelper.GetWeekBoundaries(DateTime.Today).PreviousMonday;
                endDate = DateHelper.GetWeekBoundaries(DateTime.Today).NextSunday;
            }


            returnData = await GetFlightGroupData(startDate, endDate, request.departmentIds, cancellationToken);

            return returnData;

        }

        #endregion


        #region EmployeeData

        public async Task<List<GetTransportGroupEmployeeDataResponse>> GetTransportGroupEmployeeData(GetTransportGroupEmployeeDataRequest request, CancellationToken cancellationToken)
        {

            DateTime effectiveDate = request.currentDate ?? DateTime.Today;
            DateTime startOfWeek = DateHelper.GetWeekBoundaries(effectiveDate).PreviousMonday;
            DateTime dayNumDate = DateTime.Today;
            if (Enum.TryParse(request.Daynum, true, out DayOfWeek dayOfWeek))
            {
                int dayOffset = ((int)dayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
                dayNumDate = startOfWeek.AddDays(dayOffset);
            }

            var returnData = await GetTransportGroupEmployeeData(request.groupMasterId, dayNumDate, cancellationToken);
            return returnData;

        }


        private string GetDepartmentHierarchyQuery(List<int> Ids)
        {



            string departmentParam = string.Join(", ", Ids);

            return @$"WITH DepartmentHierarchy AS (
                        SELECT 
                            Id,
                            ParentDepartmentId
                        FROM 
                            dbo.Department
                        WHERE 
                            Id  IN ({departmentParam})
                        UNION ALL
                        SELECT 
                            d.Id,
                            d.ParentDepartmentId
                        FROM 
                            dbo.Department d
                        INNER JOIN 
                            DepartmentHierarchy dh ON dh.Id = d.ParentDepartmentId
                    )
                    SELECT 
                        Id
                    FROM 
                        DepartmentHierarchy";
        }


        private async Task<List<GetTransportGroupEmployeeDataResponse>> GetTransportGroupEmployeeData(int FlightGroupMasterId, DateTime currentDate, CancellationToken cancellationToken)
        {
            var currentdatestring = currentDate.ToString("yyyy-MM-dd");
            string query = "";

            if (FlightGroupMasterId == 0)
            {
                query = @$" SELECT t.Id, e.Id EmployeeId, Concat(e.Firstname, ' ', e.Lastname) FullName, e.SAPID, d.Name DepartmentName, e1.Description EmployerName, t.EventDate, at.Code ActiveTransportCode, ts.Description ScheduleCode, t.Status  FROM Transport t
                    INNER join Employee e ON t.EmployeeId = e.Id 
                    left join ActiveTransport at ON t.ActiveTransportId = at.Id
                    left join TransportSchedule ts ON t.ScheduleId = ts.id
                    left JOIN Department d ON e.DepartmentId = d.Id
                    left JOIN Employer e1 ON e.employerId = e1.Id

                     WHERE t.EventDate = '{currentdatestring}' and t.Direction = 'IN'
                     AND e.FlightGroupMasterId is null
                     ORDER BY e.Firstname";
            }
            else {
                query = @$" SELECT t.Id, e.Id EmployeeId, Concat(e.Firstname, ' ', e.Lastname) FullName, e.SAPID, d.Name DepartmentName, e1.Description EmployerName, t.EventDate, at.Code ActiveTransportCode, ts.Description ScheduleCode, t.Status  FROM Transport t
                    INNER join Employee e ON t.EmployeeId = e.Id 
                    left join ActiveTransport at ON t.ActiveTransportId = at.Id
                    left join TransportSchedule ts ON t.ScheduleId = ts.id
                    left JOIN Department d ON e.DepartmentId = d.Id
                    left JOIN Employer e1 ON e.employerId = e1.Id

                     WHERE t.EventDate = '{currentdatestring}' and t.Direction = 'IN'
                     AND e.FlightGroupMasterId = {FlightGroupMasterId}
                     ORDER BY e.Firstname";
            }




            return await GetRawQueryData<GetTransportGroupEmployeeDataResponse>(query, cancellationToken);




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
    }

}