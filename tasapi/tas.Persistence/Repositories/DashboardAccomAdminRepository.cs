using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.DashboardAccomAdminFeature.GetCampInfo;
using tas.Application.Features.DashboardAccomAdminFeature.GetCampUsageInfo;
using tas.Application.Features.DashboardAccomAdminFeature.GetNonSiteInfo;
using tas.Application.Features.DashboardAccomAdminFeature.GetNoRoomInfo;
using tas.Application.Features.DashboardAccomAdminFeature.GetOccupantsInfo;
using tas.Application.Features.DashboardAccomAdminFeature.GetPobInfo;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class DashboardAccomAdminRepository : BaseRepository<Employee>, IDashboardAccomAdminRepository
    {
        public DashboardAccomAdminRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {

        }

        #region OccupantInfo
        public async Task<GetOccupantsInfoResponse> GetOccupantsInfo(GetOccupantsInfoRequest request, CancellationToken cancellationToken)
        {
            DateTime currentDate = DateTime.Now;


            if (request.CurrentDate.HasValue)
            {
                currentDate = request.CurrentDate.Value;
            }

            DateTime previousMonday = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday);
            if (currentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                previousMonday = previousMonday.AddDays(-7);
            }

            // Calculate the next Sunday
            DateTime nextSunday = currentDate.AddDays(DayOfWeek.Sunday - currentDate.DayOfWeek);
            if (currentDate.DayOfWeek != DayOfWeek.Sunday)
            {
                nextSunday = currentDate.AddDays(7 - (int)currentDate.DayOfWeek);
            }


            var returnDate = new GetOccupantsInfoResponse();
            returnDate.StartDate = previousMonday;
            returnDate.EndDate = nextSunday;
            returnDate.AllData = await GetWeekOccupantsInfoCount(cancellationToken);
            returnDate.WeekNumber = GetWeekNumber(currentDate);
            returnDate.ByBedCountData = await GetTodayOccupantsInfoByBedCount( cancellationToken);
            return returnDate;
        }


        private async Task<List<GetOccupantsInfoCount>> GetWeekOccupantsInfoCount(CancellationToken cancellationToken)
        {
            var startdatestring = DateHelper.GetWeekBoundaries(DateTime.Today).PreviousMonday.ToString("yyyy-MM-dd");
            var enddatestring = DateHelper.GetWeekBoundaries(DateTime.Today).NextSunday.ToString("yyyy-MM-dd");



            string query = @$"SELECT 
                            COUNT(es.EmployeeId) AS count, 
                            CASE 
                                WHEN pt.Code= 'Visitor' OR pt.Code= 'Non.S' THEN 'Visitor'
                                ELSE pt.Code
                            END AS Code,
                                CONVERT(DATE, es.EventDate) AS EventDate
                        FROM EmployeeStatus es
                        LEFT JOIN Employee e ON es.EmployeeId = e.Id
                        LEFT JOIN PeopleType pt ON e.PeopleTypeId = pt.Id
                        WHERE CONVERT(DATE, es.EventDate) >= '{startdatestring}' 
                                        AND CONVERT(DATE, es.EventDate) <= '{enddatestring}'
                          AND es.RoomId NOT IN (SELECT Id FROM Room r WHERE r.VirtualRoom > 0)
                        GROUP BY 
                            CASE 
                                WHEN pt.Code= 'Visitor' OR pt.Code= 'Non.S' THEN 'Visitor'
                                ELSE pt.Code
                            END,
                            CONVERT(DATE, es.EventDate)
                        ORDER BY count DESC;";

            var returndata = await GetRawQueryData<GetOccupantsInfoCount>(query, cancellationToken);
            return returndata;

        }


        private async Task<List<GetOccupantsInfoByBedCount>> GetTodayOccupantsInfoByBedCount(CancellationToken cancellationToken)
        {
            var todaydatestring = DateTime.Today.ToString("yyyy-MM-dd");


            var query = @$"
                    WITH RoomOccupancy AS (
                        SELECT
                            r.BedCount,
                            r.Id AS RoomId,
                            COUNT(es.EmployeeId) AS OccupantCount
                        FROM
                            Room r
                        LEFT JOIN
                            EmployeeStatus es ON r.Id = es.RoomId AND CONVERT(DATE, es.EventDate) = '{todaydatestring}'  AND RoomId NOT in  (SELECT Id FROM Room r WHERE r.VirtualRoom > 0)
                        GROUP BY
                            r.BedCount,
                            r.Id
                    ),
                    OccupantCategories AS (
                        SELECT
                            BedCount,
                            OccupantCount,
                            CASE
                                WHEN OccupantCount = 1 THEN '1 Occupant'
                                WHEN OccupantCount = 2 THEN '2 Occupants'
                                WHEN OccupantCount = 3 THEN '3 Occupants'
                                WHEN OccupantCount = 4 THEN '4 Occupants'
                                ELSE CAST(OccupantCount AS VARCHAR) + ' Occupants'
                            END AS OccupantType
                        FROM
                            RoomOccupancy
                            WHERE OccupantCount > 0
                    )
                    SELECT
                        CASE
                           
                            WHEN BedCount = 1 THEN '1 bed'
                            WHEN BedCount = 2 THEN '2 bed'
                            WHEN BedCount = 3 THEN '3 bed'
                            ELSE '0 bed'
                        END AS RoomType,
                        OccupantType,
                        COUNT(*) AS RoomCount,
                        SUM(OccupantCount) AS TotalOccupants
                    FROM
                        OccupantCategories


                    GROUP BY
                     BedCount,
                        OccupantType
    
                    ORDER BY
                        BedCount,
                        OccupantType";

            var returndata = await GetRawQueryData<GetOccupantsInfoByBedCount>(query, cancellationToken);
            return returndata;

        }


        #endregion







        #region POB

        public async Task<GetPobInfoResponse> GetPobInfo(GetPobInfoRequest request, CancellationToken cancellationToken)
        {
            DateTime currentDate = DateTime.Now;


            if (request.CurrentDate.HasValue)
            {
                currentDate = request.CurrentDate.Value;
            }

            DateTime previousMonday = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday);
            if (currentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                previousMonday = previousMonday.AddDays(-7);
            }

            // Calculate the next Sunday
            DateTime nextSunday = currentDate.AddDays(DayOfWeek.Sunday - currentDate.DayOfWeek);
            if (currentDate.DayOfWeek != DayOfWeek.Sunday)
            {
                nextSunday = currentDate.AddDays(7 - (int)currentDate.DayOfWeek);
            }


            var returnDate = new GetPobInfoResponse();
            returnDate.StartDate = previousMonday;
            returnDate.EndDate = nextSunday;
            returnDate.PobDates = await GetPobInfoDates(returnDate.StartDate, returnDate.EndDate, cancellationToken);
            returnDate.WeekNumber = GetWeekNumber(currentDate);

            var avgPob = await GetAveragePOB(returnDate.StartDate, returnDate.EndDate);
            returnDate.POB = Convert.ToInt32(avgPob);
            return returnDate;
        }

        private async Task<List<GetPobInfoDates>> GetPobInfoDates(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            var startdatestring = startDate.ToString("yyyy-MM-dd");
            var enddatestring = endDate.ToString("yyyy-MM-dd");


            string query = @$"SELECT
                        CONVERT(DATE, es.EventDate) AS EventDate,
                        COUNT(es.Id) AS OnsiteEmployees
                    FROM
                        EmployeeStatus es
                    LEFT JOIN
                        Employee e ON es.EmployeeId = e.Id
                    WHERE
                        CONVERT(DATE, es.EventDate) BETWEEN '{startdatestring}' AND '{enddatestring}'
                        AND es.RoomId IS NOT NULL 
                        and es.RoomId not in (select Id from Room where virtualRoom > 0)
                        
                    GROUP BY
                        CONVERT(DATE, es.EventDate)";

            var returndata = await GetRawQueryData<GetPobInfoDates>(query, cancellationToken);
            return returndata;

        }

        private async Task<double> GetAveragePOB(DateTime startDate, DateTime endDate)
        {

           var virtualRoomIds =await  Context.Room.AsNoTracking().Where(c => c.VirtualRoom > 0).Select(x=> x.Id).ToListAsync();

            var query = await Context.EmployeeStatus
                .Where(es => es.EventDate >= startDate && es.EventDate <= endDate && es.RoomId != null && !virtualRoomIds.Contains(es.RoomId.Value))
                .GroupBy(es => es.EventDate.Value.Date)
                .Select(g => new
                {
                    EventDate = g.Key,
                    OnsiteEmployees = g.Count()
                })
                .ToListAsync();

            var averageOnsiteEmployees = query.Average(x => x.OnsiteEmployees);

            return averageOnsiteEmployees;
        }

        #endregion

        #region NoRoomInfo


        public async Task<GetNoRoomInfoResponse> GetNoRoomInfo(GetNoRoomInfoRequest request, CancellationToken cancellationToken)
        {
            DateTime currentDate = DateTime.Now;

            if (request.CurrentDate.HasValue)
            {
                currentDate = request.CurrentDate.Value;
            }

            DateTime previousMonday = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday);
            if (currentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                previousMonday = previousMonday.AddDays(-7);
            }

            // Calculate the next Sunday
            DateTime nextSunday = currentDate.AddDays(DayOfWeek.Sunday - currentDate.DayOfWeek);
            if (currentDate.DayOfWeek != DayOfWeek.Sunday)
            {
                nextSunday = currentDate.AddDays(7 - (int)currentDate.DayOfWeek);
            }


            var returnDate = new GetNoRoomInfoResponse();
            returnDate.StartDate = previousMonday;
            returnDate.EndDate = nextSunday;
            returnDate.WeekNumber = GetWeekNumber(currentDate);
            returnDate.PeopleType = await GetNoRoomInfoEmployeesPeopleType(returnDate.StartDate, returnDate.EndDate, cancellationToken);
            returnDate.Gender = await GetNoRoomInfoEmployeesGender(returnDate.StartDate, returnDate.EndDate, cancellationToken);
            returnDate.RoomType = await GetNoRoomInfoEmployeesRoomType(returnDate.StartDate, returnDate.EndDate, cancellationToken);


            return returnDate;




        }

        private async Task<List<GetNoRoomInfoEmployeesRoomType>> GetNoRoomInfoEmployeesRoomType(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            var startdatestring = startDate.ToString("yyyy-MM-dd");
            var enddatestring = endDate.ToString("yyyy-MM-dd");

            string query = @$"
                            SELECT
                                'Senior' RoomType,
                                COUNT(es.Id) AS OnsiteEmployees
                            FROM
                                EmployeeStatus es
                            INNER JOIN
                                Employee e ON es.EmployeeId = e.Id AND e.RoomId IS NULL
                            LEFT JOIN
                                Room r ON es.RoomId = r.Id
                            WHERE
                                CONVERT(DATE, es.EventDate) = '{enddatestring}'
                                AND es.RoomId IS NOT NULL
                                AND r.BedCount = 1
                               AND es.RoomId NOT IN (SELECT Id FROM Room WHERE VirtualRoom > 0)
                    union ALL 
                        SELECT
                            'Junior' RoomType,
                            COUNT(es.Id) AS OnsiteEmployees
                        FROM
                            EmployeeStatus es
                        INNER JOIN
                            Employee e ON es.EmployeeId = e.Id AND e.RoomId IS NULL
                        LEFT JOIN
                            Room r ON es.RoomId = r.Id
                        WHERE
                            CONVERT(DATE, es.EventDate) = '{enddatestring}'
                            AND es.RoomId IS NOT NULL
                            AND r.BedCount > 1
                            AND es.RoomId NOT IN (SELECT Id FROM Room WHERE VirtualRoom > 0)";

            var returndata = await GetRawQueryData<GetNoRoomInfoEmployeesRoomType>(query, cancellationToken);
            return returndata;

        }



        private async Task<List<GetNoRoomInfoEmployeesPeopleType>> GetNoRoomInfoEmployeesPeopleType(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            var enddatestring = endDate.ToString("yyyy-MM-dd");
            string query = @$"SELECT Count(*) OnsiteEmployees , pt.Code PeopleType FROM Employee e 
                   LEFT JOIN
                            PeopleType pt ON e.PeopleTypeId = pt.Id
                   right join EmployeeStatus es ON e.Id = es.EmployeeId AND es.RoomId is NOT NULL 
                        AND es.RoomId NOT IN (SELECT Id FROM Room WHERE VirtualRoom > 0)
                   AND es.EventDate = '{enddatestring}'
                         where e.RoomId IS NULL
                         AND e.Active = 1
                         AND pt.Code = 'Nat'
                    
  
                    GROUP BY pt.Code
                    UNION all

                    SELECT Count(*) OnsiteEmployees, pt.Code PeopleType FROM Employee e 
                                       LEFT JOIN  PeopleType pt ON e.PeopleTypeId = pt.Id
                                                                   right join EmployeeStatus es ON e.Id = es.EmployeeId AND es.RoomId is NOT NULL 
                                                                           AND es.RoomId NOT IN (SELECT Id FROM Room WHERE VirtualRoom > 0)
                                       AND es.EventDate =  '{enddatestring}'
                                             where e.RoomId IS NULL
                                             AND e.Active = 1
                                             AND pt.Code = 'Expat'
                    GROUP BY pt.Code";

            var returndata = await GetRawQueryData<GetNoRoomInfoEmployeesPeopleType>(query, cancellationToken);
            return returndata;

        }


        private async Task<List<GetNoRoomInfoEmployeesGender>> GetNoRoomInfoEmployeesGender(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            var enddatestring = endDate.ToString("yyyy-MM-dd");
            string query = @$"SELECT
                            CASE WHEN e.gender = 1 THEN 'Male' ELSE 'Female' END AS Gender,
                            COUNT(*) AS OnsiteEmployees
                        FROM
                            Employee e
                        JOIN (
                            SELECT DISTINCT EmployeeId
                            FROM EmployeeStatus es
                            WHERE es.RoomId IS NOT NULL 

                            AND es.EventDate = '{enddatestring}'
                            AND es.RoomId NOT IN (SELECT Id FROM Room WHERE VirtualRoom > 0)
                        ) esdata ON e.Id = esdata.EmployeeId
                            where e.RoomId is null
                        GROUP BY
                            CASE WHEN e.gender = 1 THEN 'Male' ELSE 'Female' END;";

            var returndata = await GetRawQueryData<GetNoRoomInfoEmployeesGender>(query, cancellationToken);
            return returndata;

        }

        #endregion




        #region OnsiteEmployee

        public async Task<GetNonSiteInfoResponse> GetNonSiteInfo(GetNonSiteInfoRequest request, CancellationToken cancellationToken)
        {
            DateTime currentDate = DateTime.Now;


            if (request.CurrentDate.HasValue) { 
                currentDate = request.CurrentDate.Value;    
            }

            DateTime previousMonday = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday);
            if (currentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                previousMonday = previousMonday.AddDays(-7);
            }

            // Calculate the next Sunday
            DateTime nextSunday = currentDate.AddDays(DayOfWeek.Sunday - currentDate.DayOfWeek);
            if (currentDate.DayOfWeek != DayOfWeek.Sunday)
            {
                nextSunday = currentDate.AddDays(7 - (int)currentDate.DayOfWeek);
            }


            var returnDate = new GetNonSiteInfoResponse();
            returnDate.StartDate = previousMonday;   
            returnDate.EndDate = nextSunday;
            returnDate.Employees = await GetNonSiteInfoEmployees(returnDate.StartDate, returnDate.EndDate, cancellationToken);
            returnDate.WeekNumber = GetWeekNumber(currentDate);
            return returnDate;  



        }

        private async Task<List<GetNonSiteInfoEmployees>> GetNonSiteInfoEmployees(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            var startdatestring = startDate.ToString("yyyy-MM-dd");
            var enddatestring = endDate.ToString("yyyy-MM-dd");


            string query = @$"    SELECT
                            'Non.s' PeopleType,
                            COUNT(es.Id) AS OnSiteEmployee
                        FROM
                            EmployeeStatus es
                        LEFT JOIN
                            Employee e ON es.EmployeeId = e.Id
                        LEFT JOIN
                            PeopleType pt ON e.PeopleTypeId = pt.Id
                        WHERE
                            CONVERT(DATE, es.EventDate) BETWEEN '{startdatestring}' AND '{enddatestring}'
                            AND pt.Code like 'Non.s%'
                            AND es.RoomId is NOT NULL

                    union ALL

                        SELECT
                            'Visitor' AS PeopleType,
                            COUNT(es.Id) AS OnSiteEmployee
                        FROM
                            EmployeeStatus es
                        LEFT JOIN
                            Employee e ON es.EmployeeId = e.Id
                        LEFT JOIN
                            PeopleType pt ON e.PeopleTypeId = pt.Id
                        WHERE
                            CONVERT(DATE, es.EventDate) BETWEEN '{startdatestring}' AND '{enddatestring}'
                            AND pt.Code =  'Visitor'
                            AND es.RoomId is NOT NULL";

            var returndata = await GetRawQueryData<GetNonSiteInfoEmployees>(query, cancellationToken);
            return returndata;

        }

        private int GetWeekNumber(DateTime date)
        {
            // Get the current culture
            CultureInfo currentCulture = CultureInfo.CurrentCulture;

            // Get the calendar from the current culture
            Calendar calendar = currentCulture.Calendar;

            // Specify which day is the first day of the week
            CalendarWeekRule weekRule = currentCulture.DateTimeFormat.CalendarWeekRule;
            DayOfWeek firstDayOfWeek = currentCulture.DateTimeFormat.FirstDayOfWeek;

            // Get the week number for the specified date
            int weekNumber = calendar.GetWeekOfYear(date, weekRule, firstDayOfWeek);

            return weekNumber;
        }


        #endregion


        #region UseageInfo
        public async Task<GetCampUsageInfoResponse> GetCampUsageInfo(GetCampUsageInfoRequest request, CancellationToken cancellationToken)
        {
            DateTime currentDate = DateTime.Today;
            if (request.CurrentDate.HasValue) { 
                currentDate = request.CurrentDate.Value;
            }

            var returnData = new GetCampUsageInfoResponse();
            returnData.Camp = await GetCampUsageInfoCamp(currentDate, cancellationToken);
            returnData.Senior = await GetCampUsageInfoSenior(currentDate, cancellationToken);
            returnData.Gender = await GetCampUsageInfoGender(currentDate, cancellationToken);

            return returnData;

        }


        private async Task<List<GetCampUsageInfoCamp>> GetCampUsageInfoCamp(DateTime currentDate, CancellationToken cancellationToken)
        {
            var datestring = currentDate.ToString("yyyy-MM-dd");
            string query = @$"WITH RoomStats AS (
                        SELECT 
                            c.Id AS CampId,
                            c.Code AS Camp,
                            COUNT(r.Id) AS ActualRoom,
                            NULLIF(SUM(r.BedCount), 0) AS ActualBed
                        FROM 
                            Camp c
                        LEFT JOIN 
                            Room r ON c.Id = r.CampId
                        GROUP BY 
                            c.Code, c.Id
                    ),
                    AssignedStats AS (
                        SELECT
                            c.Id AS CampId,
                            COUNT(e.Id) AS AssignedPeople
                        FROM
                            Employee e
                        LEFT JOIN 
                            Room r ON e.RoomId = r.Id
                        LEFT JOIN 
                            Camp c ON r.CampId = c.Id
                        GROUP BY
                            c.Id
                    ),
                    OnSiteStats AS (
                        SELECT
                            c.Id AS CampId,
                            COUNT(es.Id) AS OnSitePeople
                        FROM
                            EmployeeStatus es
                        LEFT JOIN 
                            Room r ON es.RoomId = r.Id
                        LEFT JOIN 
                            Camp c ON r.CampId = c.Id
                        WHERE
                            CONVERT(DATE, es.EventDate) = '{datestring}'
                        GROUP BY
                            c.Id
                    ),
                    VacantRoomStats AS (
                        SELECT
                            c.Id AS CampId,
                            COUNT(r.Id) AS VacantRoomNoOwner
                        FROM
                            Room r
                        LEFT JOIN
                            Employee e ON r.Id = e.RoomId
                        LEFT JOIN 
                            Camp c ON r.CampId = c.Id
                        WHERE
                            e.Id IS NULL
                        GROUP BY
                            c.Id
                    ),
                    VacantBedStats AS (
                        SELECT
                            c.Id AS CampId,
                            COUNT(b.Id) AS VacantBedNoOwner
                        FROM
                            Bed b
                        LEFT JOIN
                            EmployeeStatus es ON b.Id = es.BedId
                        LEFT JOIN 
                            Room r ON b.RoomId = r.Id
                        LEFT JOIN 
                            Camp c ON r.CampId = c.Id
                        WHERE
                            es.Id IS NULL
                        GROUP BY
                            c.Id
                    )
                    SELECT
                        rs.Camp,
                        rs.ActualRoom,
                        rs.ActualBed,
                        ISNULL(asg.AssignedPeople, 0) AS AssignedPeople,
                        ISNULL(ons.OnSitePeople, 0) AS OnSitePeople,
                        CASE WHEN rs.ActualBed > 0 THEN ROUND((ISNULL(ons.OnSitePeople, 0) * 100.0) / rs.ActualBed, 2) ELSE 0 END AS UsagePercentage,
                        rs.ActualBed - ISNULL(ons.OnSitePeople, 0) AS AvailableBedOnSite,
                        ISNULL(vrs.VacantRoomNoOwner, 0) AS VacantRoomNoOwner,
                        ISNULL(vbs.VacantBedNoOwner, 0) AS VacantBedNoOwner
                    FROM
                        RoomStats rs
                    LEFT JOIN 
                        AssignedStats asg ON rs.CampId = asg.CampId
                    LEFT JOIN 
                        OnSiteStats ons ON rs.CampId = ons.CampId
                    LEFT JOIN 
                        VacantRoomStats vrs ON rs.CampId = vrs.CampId
                    LEFT JOIN 
                        VacantBedStats vbs ON rs.CampId = vbs.CampId
                    ORDER BY
                        rs.Camp;
                    ";


            return await GetRawQueryData<GetCampUsageInfoCamp>(query, cancellationToken);
        }

        private async Task<List<GetCampUsageInfoSenior>> GetCampUsageInfoSenior(DateTime currentDate, CancellationToken cancellationToken)
        {
            var datestring = currentDate.ToString("yyyy-MM-dd");

            string query = @$"WITH RoomStats AS (
                                    SELECT 
                                        CASE 
                                            WHEN r.BedCount = 1 THEN 'Senior'
                                            WHEN r.BedCount > 1 THEN 'Junior'
                                        END AS Category,
                                        COUNT(r.Id) AS ActualRoom,
                                        SUM(r.BedCount) AS ActualBed
                                    FROM 
                                        Room r
                                    GROUP BY 
                                        CASE 
                                            WHEN r.BedCount = 1 THEN 'Senior'
                                            WHEN r.BedCount > 1 THEN 'Junior'
                                        END
                                ),
                                AssignedStats AS (
                                    SELECT
                                        CASE 
                                            WHEN r.BedCount = 1 THEN 'Senior'
                                            WHEN r.BedCount > 1 THEN 'Junior'
                                        END AS Category,
                                        COUNT(e.Id) AS AssignedPeople
                                    FROM
                                        Employee e
                                    LEFT JOIN 
                                        Room r ON e.RoomId = r.Id
                                    GROUP BY
                                        CASE 
                                            WHEN r.BedCount = 1 THEN 'Senior'
                                            WHEN r.BedCount > 1 THEN 'Junior'
                                        END
                                ),
                                OnSiteStats AS (
                                    SELECT
                                        CASE 
                                            WHEN r.BedCount = 1 THEN 'Senior'
                                            WHEN r.BedCount > 1 THEN 'Junior'
                                        END AS Category,
                                        COUNT(es.Id) AS OnSitePeople
                                    FROM
                                        EmployeeStatus es
                                    LEFT JOIN 
                                        Room r ON es.RoomId = r.Id
                                    WHERE
                                        CONVERT(DATE, es.EventDate) = '{datestring}'
                                    GROUP BY
                                        CASE 
                                            WHEN r.BedCount = 1 THEN 'Senior'
                                            WHEN r.BedCount > 1 THEN 'Junior'
                                        END
                                ),
                                VacantRoomStats AS (
                                    SELECT
                                        CASE 
                                            WHEN r.BedCount = 1 THEN 'Senior'
                                            WHEN r.BedCount > 1 THEN 'Junior'
                                        END AS Category,
                                        COUNT(r.Id) AS VacantRoomNoOwner
                                    FROM
                                        Room r
                                    LEFT JOIN
                                        Employee e ON r.Id = e.RoomId
                                    WHERE
                                        e.Id IS NULL
                                    GROUP BY
                                        CASE 
                                            WHEN r.BedCount = 1 THEN 'Senior'
                                            WHEN r.BedCount > 1 THEN 'Junior'
                                        END
                                ),
                                VacantBedStats AS (
                                    SELECT
                                        CASE 
                                            WHEN r.BedCount = 1 THEN 'Senior'
                                            WHEN r.BedCount > 1 THEN 'Junior'
                                        END AS Category,
                                        COUNT(b.Id) AS VacantBedNoOwner
                                    FROM
                                        Bed b
                                    LEFT JOIN
                                        EmployeeStatus es ON b.Id = es.BedId
                                    LEFT JOIN 
                                        Room r ON b.RoomId = r.Id
                                    WHERE
                                        es.Id IS NULL
                                    GROUP BY
                                        CASE 
                                            WHEN r.BedCount = 1 THEN 'Senior'
                                            WHEN r.BedCount > 1 THEN 'Junior'
                                        END
                                )
                                SELECT
                                    rs.Category,
                                    rs.ActualRoom,
                                    rs.ActualBed,
                                    ISNULL(asg.AssignedPeople, 0) AS AssignedPeople,
                                    ISNULL(ons.OnSitePeople, 0) AS OnSitePeople,
                                    CASE WHEN rs.ActualBed > 0 THEN ROUND((ISNULL(ons.OnSitePeople, 0) * 100.0) / rs.ActualBed, 2) ELSE 0 END AS UsagePercentage,
                                    rs.ActualBed - ISNULL(ons.OnSitePeople, 0) AS AvailableBedOnSite,
                                    ISNULL(vrs.VacantRoomNoOwner, 0) AS VacantRoomNoOwner,
                                    ISNULL(vbs.VacantBedNoOwner, 0) AS VacantBedNoOwner
                                FROM
                                    RoomStats rs
                                LEFT JOIN 
                                    AssignedStats asg ON rs.Category = asg.Category
                                LEFT JOIN 
                                    OnSiteStats ons ON rs.Category = ons.Category
                                LEFT JOIN 
                                    VacantRoomStats vrs ON rs.Category = vrs.Category
                                LEFT JOIN 
                                    VacantBedStats vbs ON rs.Category = vbs.Category

                                UNION ALL

                                SELECT
                                    'Total' AS Category,
                                    SUM(rs.ActualRoom) AS ActualRoom,
                                    SUM(rs.ActualBed) AS ActualBed,
                                    SUM(ISNULL(asg.AssignedPeople, 0)) AS AssignedPeople,
                                    SUM(ISNULL(ons.OnSitePeople, 0)) AS OnSitePeople,
                                    CASE WHEN SUM(rs.ActualBed) > 0 THEN ROUND((SUM(ISNULL(ons.OnSitePeople, 0)) * 100.0) / SUM(rs.ActualBed), 2) ELSE 0 END AS UsagePercentage,
                                    SUM(rs.ActualBed) - SUM(ISNULL(ons.OnSitePeople, 0)) AS AvailableBedOnSite,
                                    SUM(ISNULL(vrs.VacantRoomNoOwner, 0)) AS VacantRoomNoOwner,
                                    SUM(ISNULL(vbs.VacantBedNoOwner, 0)) AS VacantBedNoOwner
                                FROM
                                    RoomStats rs
                                LEFT JOIN 
                                    AssignedStats asg ON rs.Category = asg.Category
                                LEFT JOIN 
                                    OnSiteStats ons ON rs.Category = ons.Category
                                LEFT JOIN 
                                    VacantRoomStats vrs ON rs.Category = vrs.Category
                                LEFT JOIN 
                                    VacantBedStats vbs ON rs.Category = vbs.Category
                                ORDER BY
                                    Category";

            var returndata = await GetRawQueryData<GetCampUsageInfoSenior>(query, cancellationToken);
            return returndata.Where(x=> x.Category != null).ToList();

        }

        private async Task<List<GetCampUsageInfoGender>> GetCampUsageInfoGender(DateTime currentDate, CancellationToken cancellationToken)
        {
            var datestring = currentDate.ToString("yyyy-MM-dd");

            string query = @$"WITH RoomStats AS (
                                        SELECT 
                                            CASE 
                                                WHEN e.Gender = 1 THEN 'Male'
                                                ELSE 'Female'
                                            END AS Gender,
                                            COUNT(DISTINCT r.Id) AS ActualRoom,
                                            SUM(r.BedCount) AS ActualBed
                                        FROM 
                                            Room r
                                        LEFT JOIN 
                                            Employee e ON r.Id = e.RoomId
                                        GROUP BY 
                                            CASE 
                                                WHEN e.Gender = 1 THEN 'Male'
                                                ELSE 'Female'
                                            END
                                    ),
                                    AssignedStats AS (
                                        SELECT
                                            CASE 
                                                WHEN e.Gender = 1 THEN 'Male'
                                                ELSE 'Female'
                                            END AS Gender,
                                            COUNT(e.Id) AS AssignedPeople
                                        FROM
                                            Employee e
                                        LEFT JOIN 
                                            Room r ON e.RoomId = r.Id
                                        GROUP BY
                                            CASE 
                                                WHEN e.Gender = 1 THEN 'Male'
                                                ELSE 'Female'
                                            END
                                    ),
                                    OnSiteStats AS (
                                        SELECT
                                            CASE 
                                                WHEN e.Gender = 1 THEN 'Male'
                                                ELSE 'Female'
                                            END AS Gender,
                                            COUNT(es.Id) AS OnSitePeople
                                        FROM
                                            EmployeeStatus es
                                        LEFT JOIN 
                                            Employee e ON es.EmployeeId = e.Id
                                        LEFT JOIN 
                                            Room r ON es.RoomId = r.Id
                                        WHERE
                                            CONVERT(DATE, es.EventDate) = '{datestring}'
                                        GROUP BY
                                            CASE 
                                                WHEN e.Gender = 1 THEN 'Male'
                                                ELSE 'Female'
                                            END
                                    ),
                                    VacantRoomStats AS (
                                        SELECT
                                            CASE 
                                                WHEN e.Gender = 1 THEN 'Male'
                                                ELSE 'Female'
                                            END AS Gender,
                                            COUNT(DISTINCT r.Id) AS VacantRoomNoOwner
                                        FROM
                                            Room r
                                        LEFT JOIN
                                            Employee e ON r.Id = e.RoomId
                                        WHERE
                                            e.Id IS NULL
                                        GROUP BY
                                            CASE 
                                                WHEN e.Gender = 1 THEN 'Male'
                                                ELSE 'Female'
                                            END
                                    ),
                                    VacantBedStats AS (
                                        SELECT
                                            CASE 
                                                WHEN e.Gender = 1 THEN 'Male'
                                                ELSE 'Female'
                                            END AS Gender,
                                            COUNT(b.Id) AS VacantBedNoOwner
                                        FROM
                                            Bed b
                                        LEFT JOIN
                                            EmployeeStatus es ON b.Id = es.BedId
                                        LEFT JOIN 
                                            Employee e ON es.EmployeeId = e.Id
                                        WHERE
                                            es.Id IS NULL
                                        GROUP BY
                                            CASE 
                                                WHEN e.Gender = 1 THEN 'Male'
                                                ELSE 'Female'
                                            END
                                    )
                                    SELECT
                                        rs.Gender AS Gender,
                                        rs.ActualRoom,
                                        rs.ActualBed,
                                        ISNULL(asg.AssignedPeople, 0) AS AssignedPeople,
                                        ISNULL(ons.OnSitePeople, 0) AS OnSitePeople,
                                        CASE WHEN rs.ActualBed > 0 THEN ROUND((ISNULL(ons.OnSitePeople, 0) * 100.0) / rs.ActualBed, 2) ELSE 0 END AS UsagePercentage,
                                        rs.ActualBed - ISNULL(ons.OnSitePeople, 0) AS AvailableBedOnSite,
                                        ISNULL(vrs.VacantRoomNoOwner, 0) AS VacantRoomNoOwner,
                                        ISNULL(vbs.VacantBedNoOwner, 0) AS VacantBedNoOwner
                                    FROM
                                        RoomStats rs
                                    LEFT JOIN 
                                        AssignedStats asg ON rs.Gender = asg.Gender
                                    LEFT JOIN 
                                        OnSiteStats ons ON rs.Gender = ons.Gender
                                    LEFT JOIN 
                                        VacantRoomStats vrs ON rs.Gender = vrs.Gender
                                    LEFT JOIN 
                                        VacantBedStats vbs ON rs.Gender = vbs.Gender

                                    UNION ALL

                                    SELECT
                                        'Total' AS Gender,
                                        SUM(rs.ActualRoom) AS ActualRoom,
                                        SUM(rs.ActualBed) AS ActualBed,
                                        SUM(ISNULL(asg.AssignedPeople, 0)) AS AssignedPeople,
                                        SUM(ISNULL(ons.OnSitePeople, 0)) AS OnSitePeople,
                                        CASE WHEN SUM(rs.ActualBed) > 0 THEN ROUND((SUM(ISNULL(ons.OnSitePeople, 0)) * 100.0) / SUM(rs.ActualBed), 2) ELSE 0 END AS UsagePercentage,
                                        SUM(rs.ActualBed) - SUM(ISNULL(ons.OnSitePeople, 0)) AS AvailableBedOnSite,
                                        SUM(ISNULL(vrs.VacantRoomNoOwner, 0)) AS VacantRoomNoOwner,
                                        SUM(ISNULL(vbs.VacantBedNoOwner, 0)) AS VacantBedNoOwner
                                    FROM
                                        RoomStats rs
                                    LEFT JOIN 
                                        AssignedStats asg ON rs.Gender = asg.Gender
                                    LEFT JOIN 
                                        OnSiteStats ons ON rs.Gender = ons.Gender
                                    LEFT JOIN 
                                        VacantRoomStats vrs ON rs.Gender = vrs.Gender
                                    LEFT JOIN 
                                        VacantBedStats vbs ON rs.Gender = vbs.Gender
                                    ORDER BY
                                        Gender;
                                    ";

            var returndata = await GetRawQueryData<GetCampUsageInfoGender>(query, cancellationToken);
            return returndata;

        }




        #endregion


        #region CampInfo



        public async Task<GetCampInfoResponse> GetCampInfo(GetCampInfoRequest request, CancellationToken cancellationToken) 
        {
            var returnData = new GetCampInfoResponse();

            returnData.CampInfoCampData = await GetCampInfoAsync(cancellationToken);

            var RoomData = new List<GetCampInfoCampRoomData>();



           var camps =await Context.Camp.AsNoTracking().Where(x => x.Active == 1).ToListAsync();

            foreach (var item in camps)
            {

                var CampRoomBedData = await GetCampRoomBedData(item.Id, cancellationToken);
                if (CampRoomBedData.Count > 0)
                {
                    var newRecord = new GetCampInfoCampRoomData();
                    newRecord.Camp = item.Code;
                    newRecord.RoomAndBed = await GetCampRoomBedData(item.Id, cancellationToken);
                    RoomData.Add(newRecord);
                }

            }

            returnData.CampInfoCampBedRoomData = RoomData;
            returnData.CampInfoRoomTypeData = await GetCampInfoTotalRoomTypeData(cancellationToken);

            return returnData;

        }

        private async Task<List<GetCampInfoCampRoomRoomBedData>> GetCampRoomBedData(int campId, CancellationToken cancellationToken)
        {
            string query = @$"
                            WITH RoomStats AS (
                                SELECT 
                                    c.Code AS Camp,
                                    r.BedCount,
                                    COUNT(r.Id) AS ActualRoomQty,
                                    SUM(r.BedCount) AS ActualBedQty
                                FROM 
                                    Room r
                                LEFT JOIN 
                                    Camp c ON r.CampId = c.Id
                                    WHERE r.CampId = {campId}
                                GROUP BY 
                                    c.Code, r.BedCount
                            ),
                            OwnerStats AS (
                                SELECT
                                    c.Code AS Camp,
                                    r.BedCount,
                                    COUNT(e.Id) AS OwnerCount
                                FROM
                                    Employee e
                                LEFT JOIN 
                                    Room r ON e.RoomId = r.Id
                                LEFT JOIN 
                                    Camp c ON r.CampId = c.Id
                                  WHERE r.CampId = {campId}
                                GROUP BY
                                    c.Code, r.BedCount
                            ),
                            OnSiteStats AS (
                                SELECT
                                    c.Code AS Camp,
                                    r.BedCount,
                                    COUNT(es.Id) AS OnSiteCount
                                FROM
                                    EmployeeStatus es
                                LEFT JOIN 
                                    Room r ON es.RoomId = r.Id
                                LEFT JOIN 
                                    Camp c ON r.CampId = c.Id
                                WHERE
                                    CONVERT(DATE, es.EventDate) = CONVERT(DATE, GETDATE())
                                    AND r.CampId = {campId}
                                GROUP BY
                                    c.Code, r.BedCount
                            )
                            SELECT
                                rs.Camp,
                                CAST(rs.BedCount AS VARCHAR) + ' bed room' AS RoomType,
                                rs.ActualRoomQty AS RoomQTY,
                                rs.ActualBedQty AS BedQTY,
                                ISNULL(os.OwnerCount, 0) AS Owner,
                                ISNULL(ons.OnSiteCount, 0) AS OnSite
                            FROM
                                RoomStats rs
                            LEFT JOIN 
                                OwnerStats os ON rs.Camp = os.Camp AND rs.BedCount = os.BedCount
                            LEFT JOIN 
                                OnSiteStats ons ON rs.Camp = ons.Camp AND rs.BedCount = ons.BedCount
                            ORDER BY
                                rs.Camp, rs.BedCount";

            return await GetRawQueryData<GetCampInfoCampRoomRoomBedData>(query, cancellationToken);
        }

        private async Task<List<GetCampInfoCampData>> GetCampInfoAsync(CancellationToken cancellationToken)
        {
            string query = @"
                    SELECT 
                    c.Code AS Camp,
                    COUNT(DISTINCT r.Id) AS RoomQTY,
                    ISNULL(SUM(r.BedCount), 0) AS BedQTY
                FROM 
                    dbo.Camp c
                LEFT JOIN 
                    dbo.Room r ON c.Id = r.CampId
                GROUP BY 
                    c.Code
                ORDER BY 
                    c.Code;";

            var data = await GetRawQueryData<GetCampInfoCampData>(query, cancellationToken);
            return data.Where(x=> x.RoomQTY > 0 && x.BedQTY > 0).ToList();
        }


        private async Task<List<GetCampInfoTotalRoomTypeData>> GetCampInfoTotalRoomTypeData(CancellationToken cancellationToken)
        {
            string query = @"
                   WITH RoomStats AS (
                        SELECT 
                            r.BedCount,
                            COUNT(r.Id) AS TotalRoomQTY,
                            SUM(r.BedCount) AS TotalBedQTY
                        FROM 
                            Room r
                        WHERE
                            r.BedCount <= 3
                        GROUP BY 
                            r.BedCount
                    ),
                    OwnedStats AS (
                        SELECT
                            r.BedCount,
                            COUNT(e.Id) AS OwnedCount
                        FROM
                            Employee e
                        LEFT JOIN 
                            Room r ON e.RoomId = r.Id
                        WHERE
                            r.BedCount <= 3
                        GROUP BY
                            r.BedCount
                    ),
                    OnSiteStats AS (
                        SELECT
                            r.BedCount,
                            COUNT(es.Id) AS OnsiteCount
                        FROM
                            EmployeeStatus es
                        LEFT JOIN 
                            Room r ON es.RoomId = r.Id
                        WHERE
                            CONVERT(DATE, es.EventDate) = CONVERT(DATE, GETDATE()) AND r.BedCount <= 3
                        GROUP BY
                            r.BedCount
                    )
                    SELECT
                        CAST(RoomStats.BedCount AS VARCHAR) + ' bed' AS RoomType,
                        RoomStats.TotalRoomQTY AS TotalRoomQTY,
                        RoomStats.TotalBedQTY AS TotalBedQTY,
                        ISNULL(OwnedStats.OwnedCount, 0) AS Owned,
                        ISNULL(OnSiteStats.OnsiteCount, 0) AS Onsite
                    FROM
                        RoomStats
                    LEFT JOIN 
                        OwnedStats ON RoomStats.BedCount = OwnedStats.BedCount
                    LEFT JOIN 
                        OnSiteStats ON RoomStats.BedCount = OnSiteStats.BedCount
                    ";

            return await GetRawQueryData<GetCampInfoTotalRoomTypeData>(query, cancellationToken);
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