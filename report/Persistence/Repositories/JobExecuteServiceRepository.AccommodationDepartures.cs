using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public partial class JobExecuteServiceRepository
    {
        private async Task<string> AccommodationDepartures(List<ReportCol> FieldNames, string reportName, List<ReportParam> reportParams)
        {
            string fields = "";
            foreach (var field in FieldNames)
            {

                fields += $", {field.FieldName} AS {field.FieldCaption}";
            }

            var employeeId = await GetUserId();


            //var query = @$"SELECT ProfileData.Id 'Person #', CONCAT(ProfileData.Firstname, ' ', ProfileData.Lastname ) Name,
            //                RoomType.Description AS RoomType, Room.Number AS Room, Camp.Description AS Camp,
            //                d.Name Department,

            //                ActiveTransport.Code AS Flight ,Schedule.ETA ArrivalTime, 
            //              CASE WHEN ProfileData.RoomId = Room.Id THEN 'Yes' 
            //              ELSE 'No' 
            //               END AS RoomOwner,
            //              DepartureTransport.EventDate DepartureDate ,
            //              DepartureTransport.ETA DepartureTime,
            //              Employer.Description Employer,
            //              Position.Description Position,
            //              r.Number RoomNumber,
            //              ProfileData.SAPID,
            //              case WHEN ProfileData.gender = 1 THEN 'Male' ELSE
            //              'Female' END AS 'Gender',
            //              ProfileData.NRN,
            //               n.Description Nationality,
            //              PeopleType.Code ResourceType,
            //              s.Code Shift,
            //              ProfileData.PersonalMobile
            //              {fields} 
            //              FROM Transport Transport 
            //              LEFT join Employee ProfileData 
            //               ON Transport.EmployeeId = ProfileData.Id
            //              LEFT JOIN Position Position 
            //              ON ProfileData.PositionId = Position.Id 
            //              LEFT JOIN Employer Employer
            //              ON ProfileData.employerId = Employer.Id 
            //              LEFT JOIN PeopleType PeopleType 
            //              ON ProfileData.PeopleTypeId = PeopleType.Id 
            //              LEFT JOIN ReportDeparmentData d ON ProfileData.DepartmentId = d.Id

            //            left JOIN TransportSchedule Schedule ON Transport.ScheduleId = Schedule.id 
            //            left JOIN ActiveTransport ActiveTransport ON Schedule.ActiveTransportId = ActiveTransport.Id 
            //            left JOIN EmployeeStatus RoomData ON RoomData.EmployeeId = Transport.EmployeeId AND CAST(RoomData.EventDate AS DATE) = '[CURRENTDATE]' 
            //            left JOIN Room Room ON RoomData.RoomId = Room.Id 
            //            left join Nationality n ON ProfileData.NationalityId = n.Id
            //            left JOIN RoomType RoomType ON Room.RoomTypeId = RoomType.Id 
            //            left join ReportProfileData Profile on ProfileData.Id = Profile.PersonNo
            //            [CAMPQUERY] 

            //            left JOIN(SELECT t1.EmployeeId, MIN(t1.EventDate) AS EventDate, ts.ETA FROM  Transport t1
            //                      left JOIN TransportSchedule ts ON t1.ScheduleId = ts.id
            //            WHERE CAST(t1.EventDate AS DATE) = '[CURRENTDATE]' 
            //                AND t1.Direction = 'OUT' GROUP BY t1.EmployeeId, ts.ETA) DepartureTransport 
            //            ON Transport.EmployeeId = DepartureTransport.EmployeeId 
            //            left join Shift s ON RoomData.ShiftId = s.Id 
            //            left join Room r ON ProfileData.RoomId = r.Id

            //            WHERE CAST(transport.EventDate AS DATE) = '[CURRENTDATE]' AND transport.Direction = 'IN' 
            //            ORDER BY ProfileData.Firstname;";

            string query = @$"SELECT ProfileData.Id 'Person #', CONCAT(ProfileData.Firstname, ' ', ProfileData.Lastname ) Name,
                                                    Bed.Description BedNo,
                                                     Room.Number AS Room,
                                                    RoomType.Description AS RoomType, Camp.Description AS Camp,
                                                    d.Name Department,
                            
                                                    ActiveTransport.Code AS Flight ,Schedule.ETD DepartureTime, 
                                                  CASE WHEN ProfileData.RoomId = Room.Id THEN 'Yes' 
                                                  ELSE 'No' 
                                                   END AS RoomOwner,
                                                  Employer.Description Employer,
                                                  Position.Description Position,
                                                  r.Number RoomNumber,
                                                  ProfileData.SAPID,
                                                  case WHEN ProfileData.gender = 1 THEN 'Male' ELSE
                                                  'Female' END AS 'Gender',
                                                   n.Description Nationality,
                                                  PeopleType.Code ResourceType
                                                    {fields} 
                                                  FROM Transport Transport 
                                                  LEFT join Employee ProfileData 
                                                   ON Transport.EmployeeId = ProfileData.Id
                                                  LEFT JOIN Position Position 
                                                  ON ProfileData.PositionId = Position.Id 
                                                  LEFT JOIN Employer Employer
                                                  ON ProfileData.employerId = Employer.Id 
                                                  LEFT JOIN PeopleType PeopleType 
                                                  ON ProfileData.PeopleTypeId = PeopleType.Id 
                                                  LEFT JOIN ReportDeparmentData d ON ProfileData.DepartmentId = d.Id
                          
                                                left JOIN TransportSchedule Schedule ON Transport.ScheduleId = Schedule.id 
                                                left JOIN ActiveTransport ActiveTransport ON Schedule.ActiveTransportId = ActiveTransport.Id 
                                                left JOIN EmployeeStatus RoomData ON RoomData.EmployeeId = Transport.EmployeeId AND CAST(RoomData.EventDate AS DATE) = CAST(DATEADD(DAY, -1, '[CURRENTDATE]') as DATE) 
                                                left JOIN Room Room ON RoomData.RoomId = Room.Id 
                                                left JOIN Bed ON RoomData.BedId = Bed.Id
                                                left join Nationality n ON ProfileData.NationalityId = n.Id
                                                left JOIN RoomType RoomType ON Room.RoomTypeId = RoomType.Id 
                                            /*    left join ReportProfileData Profile on ProfileData.Id = Profile.PersonNo*/
                                                inner join GetReportProfileData({employeeId}) Profile on ProfileData.Id = Profile.PersonNo

                                                 [CAMPQUERY] 

                                                left join Shift s ON RoomData.ShiftId = s.Id 
                                                left join Room r ON ProfileData.RoomId = r.Id

                                                WHERE CAST(transport.EventDate AS DATE) = '[CURRENTDATE]' AND transport.Direction = 'OUT' 
                                                AND Room.Id IS NOT NULL
                                                ORDER BY ProfileData.Firstname;";



            string whereCondition = "";
            foreach (var item in reportParams)
            {




                var ItemData = await GetParametervalue(item);
                if (ItemData.FieldName == "CampId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        query = query.Replace("[CAMPQUERY]", $" RIGHT JOIN(SELECT Id, c.Description  from Camp c WHERE id IN  {item.FieldValue} ) Camp  ON Room.CampId = Camp.Id");
                    }
                    else
                    {
                        query = query.Replace("[CAMPQUERY]", " left JOIN Camp Camp ON Room.CampId = Camp.Id ");
                    }
                }
                if (ItemData.FieldName == "CurrentDate")
                {

                    if (DateTime.TryParseExact(item.FieldValueCaption, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                    {
                        query = query.Replace("[CURRENTDATE]", result.ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        DateTime currentDdate = DateTime.Today.AddDays(1);
                        query = query.Replace("[CURRENTDATE]", currentDdate.ToString("yyyy-MM-dd"));
                    }


                }
            }

            
            query = query + " " + whereCondition;
            query = query.Replace("[CAMPQUERY]", " left JOIN Camp Camp ON Room.CampId = Camp.Id ");


            return await SaveDynamicListToExcelFile(query, @"\Assets\GeneratedFiles\", reportName, reportParams);
        }
    }
}
