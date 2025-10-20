using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public partial class JobExecuteServiceRepository
    {
        #region Workflow_request

        public async Task<string> TransportDetailsQueryModfy(List<ReportCol> FieldNames, string reportName, List<ReportParam> reportParams)
        {
            var startDate = DateTime.Today;
            var endDate = DateTime.Today;

            string fields = "";
            foreach (var field in FieldNames)
            {
                    fields += $", {field.FieldName} AS {field.FieldCaption}";
                
            }

            var employeeId = await GetUserId();


            var query = @$"WITH TransportData AS (
                            SELECT
                                t.ScheduleId,
                                t.ActiveTransportId, 
                                t.EventDate, 
                                t.EmployeeId, 
                                t.Direction, 
                                LEAD(t.EventDate) OVER (PARTITION BY t.EmployeeId ORDER BY t.EventDate) AS NextEventDate,
                                LAG(t.EventDate) OVER (PARTITION BY t.EmployeeId ORDER BY t.EventDate) AS PreviousEventDate
                            FROM Transport t 
                                WHERE t.EventDate >= '[STARTDATE]' 
                                  AND t.EventDate <= '[ENDDATE]'
                                    
                        ),
                        OutDays AS (
                            SELECT 

                                ScheduleId,
                                ActiveTransportId, 
                                EventDate AS EventDate,
                                EmployeeId,
                                Direction,
                                CASE 
                                    WHEN Direction = 'OUT' AND PreviousEventDate IS NULL THEN DATEDIFF(DAY, '[STARTDATE]', EventDate)
                                    WHEN Direction = 'OUT' AND PreviousEventDate IS NOT NULL THEN DATEDIFF(DAY, PreviousEventDate, EventDate)
                                    WHEN Direction = 'OUT' AND NextEventDate IS NULL THEN DATEDIFF(DAY, EventDate, '[ENDDATE]')
                                    WHEN Direction = 'IN' AND NextEventDate IS NULL THEN DATEDIFF(DAY, EventDate, '[ENDDATE]')
                                    ELSE NULL
                                END AS Duration
                            FROM TransportData
                        )
                        SELECT
	                        t.EventDate AS 'Date', 

                                                        CASE 
                                                            WHEN  d.Name IS NOT NULL 
                                                                THEN d.Name
                                                            ELSE Profile.Department 
                                                        END AS Department,


                                                        CASE 
                                                            WHEN  employer.Description IS NOT NULL 
                                                                THEN employer.Description
                                                            ELSE Profile.Employer 
                                                        END AS Employer,
                                                        ProfileData.Id AS 'PersonId',
                                                        CONCAT(ProfileData.Firstname, ' ', ProfileData.Lastname) AS Person,
                                                        at.Code AS Transport,
                                                        ProfileData.SAPID,
                                                        t.Direction,
                                                        tm.Code AS Mode,
                                                        ts.Description,
                                                        fgm.Description AS TransportGroup,
                                                        CASE 
                                                            WHEN cc.Number IS NOT NULL 
                                                                THEN CONCAT(cc.Number, ' ', cc.Description) 
                                                            ELSE Profile.Costcode 
                                                        END AS CostCentre,
                                                        p.Description AS Position,
                                                        r.Description AS Roster,
                                                        l.Description AS CommutBase,
                                                        Room.Number AS 'Room #',
                                                        rt.Description AS RoomType,
                                                        camp.Description AS Camp,
                                                        pt.Code AS PeopleType,
                                                        s.Code AS Shift,
                                                        t.Duration OnSiteDays
                                                       {fields}
	
                        FROM OutDays t
		                         LEFT JOIN Employee ProfileData ON t.EmployeeId = ProfileData.Id
			                        LEFT JOIN TransportSchedule ts ON t.ScheduleId = ts.id
			                        LEFT JOIN ActiveTransport at ON t.ActiveTransportId = at.Id
			                        LEFT JOIN TransportMode tm ON at.TransportModeId = tm.Id
			                        LEFT JOIN Location l ON at.fromLocationId = l.Id
			                        LEFT JOIN PeopleType pt ON ProfileData.PeopleTypeId = pt.Id
			                        LEFT JOIN EmployeeStatus es ON t.EmployeeId = es.EmployeeId AND CAST(es.EventDate AS DATE) = CAST(t.EventDate AS DATE)
			                        LEFT JOIN ReportDeparmentData d ON es.DepId = d.Id
			                        LEFT JOIN Employer employer ON employer.Id = es.EmployerId
			                        LEFT JOIN CostCodes cc ON es.CostCodeId = cc.Id
			                        LEFT JOIN FlightGroupMaster fgm ON ProfileData.FlightGroupMasterId = fgm.Id
			                        LEFT JOIN Position p ON ProfileData.PositionId = p.Id
			                        LEFT JOIN Roster r ON ProfileData.RosterId = r.Id
			                        LEFT JOIN Room room ON room.Id = es.RoomId
			                        LEFT JOIN RoomType rt ON rt.Id = room.RoomTypeId
			                        LEFT JOIN Camp camp ON room.CampId = camp.Id
			                        LEFT JOIN Shift s ON es.ShiftId = s.Id
                                    LEFT JOIN ReportDeparmentData ProfileDepartment ON ProfileData.DepartmentId = ProfileDepartment.Id
                                     inner join GetReportProfileData({employeeId}) Profile on ProfileData.Id = Profile.PersonNo
                                    [WHERECONDITION]
                        ORDER BY  t.EmployeeId, t.EventDate;";

            string whereCondition = "";

            foreach (var item in reportParams)
            {

                var ItemData = await GetParametervalue(item);
                if (ItemData.FieldName == "EmployerId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        whereCondition += $" AND ProfileData.employerId IN {item.FieldValue}";

                    }


                }
                if (ItemData.FieldName == "DepartmentId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        whereCondition += $" AND ProfileData.DepartmentId IN {item.FieldValue}";

                    }
                }

                if (ItemData.FieldName == "CampId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        whereCondition += $" AND camp.Id IN {item.FieldValue}";

                    }
                }
                if (ItemData.FieldName == "GroupMasterId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {

                        whereCondition += $" AND ProfileData.Id IN (SELECT EmployeeId FROM GroupMembers gm WHERE gm.GroupDetailId IN  {item.FieldValue})";

                    }
                }
                if (ItemData.FieldName == "LocationId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {

                        whereCondition += $" AND l.id IN  {item.FieldValue}";

                    }
                }
                if (ItemData.FieldName == "StartDate")
                {

                    if (DateTime.TryParseExact(item.FieldValueCaption, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                    {
                        startDate = result;
                    }

                }
                if (ItemData.FieldName == "EndDate")
                {

                    if (DateTime.TryParseExact(item.FieldValueCaption, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                    {
                        endDate = result;
                    }

                }

            }



            query = query.Replace("[WHERECONDITION]", whereCondition);

            query = query.Replace("[STARTDATE]", $"{startDate.ToString("yyyy-MM-dd")}").Replace("[ENDDATE]", $"{endDate.ToString("yyyy-MM-dd")}");


            return await SaveDynamicListToExcelFile(query, @"\Assets\GeneratedFiles\", "TAS Transport Details", reportParams);

        }

        #endregion


    }

}

