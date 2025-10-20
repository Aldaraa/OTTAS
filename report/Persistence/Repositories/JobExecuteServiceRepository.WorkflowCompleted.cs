using OfficeOpenXml;
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

        public async Task<string> WorkflowCompletedQueryModfy(List<ReportCol> FieldNames, string reportName, List<ReportParam> reportParams)
        {
            var startDate = DateTime.Today;
            var endDate = DateTime.Today;
            string fields = "";
            var employeeId = await GetUserId();
            foreach (var field in FieldNames)
            {
                    fields += $",{field.FieldName} AS '{field.FieldCaption}'";
               

            }
            var query = @$"SELECT 
                            CASE rd.DocumentTag
                                WHEN 'ADD' THEN 'New Booking'
                                WHEN 'REMOVE' THEN 'Remove'
                                WHEN 'RESCHEDULE' THEN 'Reschedule'
                                ELSE rd.DocumentTag
                            END AS 'DocumentType', 
                                ResourceData.Id 'TAS #',
                                Concat(ResourceData.Firstname, ' ', ResourceData.Lastname) ResourceName,
                                travelinfo.Camp,
                                travelinfo.Department,
                                travelinfo.Employer,
                                travelinfo.TransportMode,
                                rd.id 'Request #',
                                rd.DateCreated 'RequestedOn',
                                rd.CompletedDate 'ProcessedOn',
                                travelinfo.FirstTravelDate,
                                travelinfo.FirstTransportCode,
                                travelinfo.FirstTransportDescription,
                                travelinfo.FirstTransportDirection,
                                travelinfo.LastTravelDate,
                                travelinfo.LastTransportCode,
                                travelinfo.LastTransportDescription,
                                travelinfo.LastTransportDirection,
                                Concat(requester.Firstname, ' ', requester.Lastname) Requester,
                                travelinfo.Reason Comment
                                {fields}
                                
                             FROM RequestDocument rd
                             left JOIN Employee ResourceData ON rd.EmployeeId = ResourceData.id 
                             left JOIN (
                                SELECT travelData.DocumentId, travelData.EmployeeId, travelData.inScheduleId, travelData.outScheduleId, travelData.RoomId,
                                  travelData.DepartmentId, travelData.CostCodeId, travelData.EmployerId,
                                  d.Name Department,
                                  employer.Description Employer,
                                  costcode.Number CostCodeNumber,
                                  costcode.Description CostCodeDescription,
                                  firsttransport.EventDate FirstTravelDate,
                                  firstransportSchedule.Code FirstTransportCode,
                                  firstransportSchedule.Description FirstTransportDescription,
                                  firsttransport.Direction FirstTransportDirection,
    
                                  lasttransport.EventDate LastTravelDate,
                                  lasttransportSchedule.Code LastTransportCode,
                                  lasttransportSchedule.Description LastTransportDescription,
                                  lasttransport.Direction LastTransportDirection,
                                  camp.Description Camp,
                                  tm.Code TransportMode,
                                  camp.Id CampId,
                                  travelData.Reason
   
                                  FROM (
                                      SELECT rsta.DocumentId, rsta.EmployeeId, rsta.inScheduleId, rsta.outScheduleId, rsta.DepartmentId, rsta.CostCodeId, rsta.EmployerId, rsta.RoomId, rsta.Reason  
                                      FROM RequestSiteTravelAdd rsta 
                                      WHERE rsta.DocumentId IN (select Id from RequestDocument
                        							 WHERE CurrentAction = 'Completed' AND
                        									DateCreated >= '[STARTDATE]' AND
                        									DateCreated <=  '[ENDDATE]'  AND 
                        									DocumentType = 'Site Travel')
                                      UNION ALL 
                                      SELECT rstr.DocumentId, rstr.EmployeeId, rstr.FirstScheduleId, rstr.LastScheduleId, e.DepartmentId, e.CostCodeId, e.employerId, e.RoomId, rstr.Reason  FROM RequestSiteTravelRemove rstr
                                      left JOIN Employee e ON rstr.EmployeeId = e.Id
                                      WHERE rstr.DocumentId IN (select Id from RequestDocument
              												 WHERE CurrentAction = 'Completed' AND
              														DateCreated >= '[STARTDATE]' AND
              														DateCreated <=  '[ENDDATE]'  AND 
              														DocumentType = 'Site Travel' )
                                                    UNION ALL 
                                      SELECT  rstr.DocumentId, rstr.EmployeeId, rstr.ExistingScheduleId, rstr.ReScheduleId, e.DepartmentId, e.CostCodeId, e.employerId, rstr.RoomId, rstr.Reason from RequestSiteTravelReschedule rstr
                                      left JOIN Employee e  ON rstr.EmployeeId = e.Id
                                      WHERE rstr.DocumentId IN  (select Id from RequestDocument
                              							 WHERE CurrentAction = 'Completed' AND
                              									DateCreated >= '[STARTDATE]' AND
                              									DateCreated <=  '[ENDDATE]'  AND 
                              									DocumentType = 'Site Travel' )) as travelData
                                      left JOIN ReportDeparmentData d on travelData.DepartmentId = d.Id
                                      left JOIN Employer employer ON travelData.EmployerId = employer.Id
                                      left JOIN CostCodes costcode ON travelData.CostCodeId = costcode.Id
          
                                      left JOIN Transport firsttransport ON travelData.inScheduleId = firsttransport.ScheduleId AND travelData.EmployeeId = firsttransport.EmployeeId
                                      left JOIN Transport lasttransport ON travelData.outScheduleId = lasttransport.ScheduleId AND travelData.EmployeeId = lasttransport.EmployeeId
                                      left JOIN TransportSchedule firstransportSchedule ON firsttransport.ScheduleId = firstransportSchedule.id
                                      left JOIN TransportSchedule lasttransportSchedule ON lasttransport.ScheduleId = lasttransportSchedule.id
                                      LEFT JOIN Room room ON travelData.RoomId = room.Id
                                      left join Camp camp on room.CampId = camp.Id
                                      left join ActiveTransport at ON firstransportSchedule.ActiveTransportId = at.Id
                                      left JOIN TransportMode tm ON at.TransportModeId = tm.Id

                             ) travelinfo
                              ON rd.id = travelinfo.DocumentId AND rd.EmployeeId = travelinfo.EmployeeId
                              
                            LEFT JOIN Employee requester
                              ON requester.Id = rd.UserIdCreated
                            left JOIN Employee Resource ON rd.EmployeeId = Resource.Id
                            LEFT JOIN CostCodes ProfileCost ON ResourceData.CostCodeId = ProfileCost.Id
                            LEFT JOIN PeopleType PeopleType ON requester.PeopleTypeId = PeopleType.Id
                           /* INNER join ReportProfileData Profile on ResourceData.Id = Profile.PersonNo*/
                            inner join GetReportProfileData({employeeId}) Profile on ResourceData.Id = Profile.PersonNo

                            WHERE rd.CurrentAction = 'Completed' AND
									rd.DateCreated >= '[STARTDATE]' AND
									rd.DateCreated <=  '[ENDDATE]'  AND 
									rd.DocumentType = 'Site Travel'
                             [WHERECONDITION]";

            string whereCondition = "";

            foreach (var item in reportParams)
            {

                var ItemData = await GetParametervalue(item);
                if (ItemData.FieldName == "EmployerId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                            whereCondition += $" AND Resource.employerId IN {item.FieldValue}";
                        
                    }


                }
                if (ItemData.FieldName == "DepartmentId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                            whereCondition += $" AND Resource.DepartmentId IN {item.FieldValue}";
                       
                    }
                }

                if (ItemData.FieldName == "CampId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                            whereCondition += $" AND travelinfo.CampId IN {item.FieldValue}";
                       
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


            return await SaveDynamicListToExcelFile(query, @"\Assets\GeneratedFiles\", "Workflow Completed site travel", reportParams);


        }

        #endregion
    }
}
