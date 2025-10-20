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

        public async Task<string> NonSiteTravelQueryModfy(List<ReportCol> FieldNames, string reportName, List<ReportParam> reportParams)
        {
            var startDate = DateTime.Today;
            var endDate = DateTime.Today;

            var employeeId = await GetUserId();

            var query = @"
                    SELECT 
                      CASE 
                        WHEN e.gender = 1 THEN 'Male'
                        ELSE 'Female' 
                      END AS Gender,
                      CONCAT(e.Firstname, ' ', e.Lastname) AS Traveller,
                      employer.Description AS Employer,
                      department.Name AS Department,
                      Concat(Manager.Firstname, ' ', Manager.Lastname) Manager,
                      CONCAT(cc.Code, cc.Number, ' ', cc.Description) AS CostCode,
                      rtp.Description AS PurposeOfTravel,
                      flightDirection.FlightDirectionCode AS TravelSummary,
                      costData.Cost Cost,
                      rta.Description AS TicketAgent,
                      travelDate.travelFirstDate FirstDate,
                      rnst.DocumentId AS 'Request #',
                      CASE WHEN e.PersonalMobile IS NULL OR e.PersonalMobile = '' THEN '########'
                      ELSE e.PersonalMobile
                      END   AS 'Contact #'
                    FROM 
                      RequestNonSiteTravel rnst
                    RIGHT JOIN 
                      (
                        SELECT 
                          id, 
                          rd.DateCreated, 
                          rd.EmployeeId, 
                          rd.UserIdCreated 
                        FROM 
                          RequestDocument rd
                        WHERE 
                          rd.CurrentAction = 'Completed' AND 
                          rd.DocumentType = 'Non Site Travel'
                          AND rd.id IN (SELECT DocumentId FROM (SELECT
                        rdh.DocumentId
                       ,MAX(rdh.DateCreated) AS DateCreated
                      FROM RequestDocumentHistory rdh
                      WHERE rdh.DateCreated >= '{STARTDATE}'
                      AND rdh.DateCreated <= '{ENDDATE}'
                      GROUP BY rdh.DocumentId) AS docData)
                      ) doc ON doc.id = rnst.DocumentId
  
                    LEFT JOIN 
                      Employee e ON doc.EmployeeId = e.Id
                    LEFT JOIN 
                      Employer employer ON e.employerId = employer.Id
                    LEFT JOIN 
                      Department department ON e.DepartmentId = department.Id
                    LEFT JOIN 
                      CostCodes cc ON e.CostCodeId = cc.Id
                    LEFT JOIN 
                      RequestTravelPurpose rtp ON rnst.RequestTravelPurposeId = rtp.Id
                    LEFT JOIN 
                      (
                        SELECT 
                          rnstf.DocumentId, 
                          STRING_AGG(CONCAT(ra.Code, '-', ra1.Code), '/') WITHIN GROUP (ORDER BY CONCAT(ra.Code, '-', ra1.Code)) AS FlightDirectionCode
                        FROM 
                          RequestNonSiteTravelFlight rnstf
                        LEFT JOIN 
                          RequestAirport ra ON ra.id = rnstf.DepartLocationId
                        LEFT JOIN 
                          RequestAirport ra1 ON ra1.id = rnstf.ArriveLocationId
                        GROUP BY 
                          rnstf.DocumentId
                      ) flightDirection ON rnst.DocumentId = flightDirection.DocumentId
                    LEFT JOIN 
                      RequestTravelAgent rta ON rnst.RequestTravelAgentId = rta.Id
                    left JOIN (SELECT min(TravelDate) travelFirstDate, DocumentId FROM RequestNonSiteTravelFlight WHERE DocumentId IN (
                    SELECT 
                        rd.Id
      
                        FROM 
                          RequestDocument rd
                        WHERE 
                          rd.CurrentAction = 'Completed' AND 
                          rd.DocumentType = 'Non Site Travel')
                    GROUP BY DocumentId
                    ) travelDate
                    ON travelDate.DocumentId = rnst.DocumentId
                    LEFT JOIN (
                          SELECT
                        rdh.DocumentId
                       ,MAX(rdh.DateCreated) AS DateCreated
                       ,rdh.ActionEmployeeId
                      FROM RequestDocumentHistory rdh
                      WHERE rdh.DateCreated >= '{STARTDATE}'
                      AND rdh.DateCreated <= '{ENDDATE}'
                      AND rdh.CurrentAction = 'Completed'
                      AND rdh.DocumentId IN (SELECT 
                        rd.Id
                        FROM 
                          RequestDocument rd
                        WHERE 
                          rd.CurrentAction = 'Completed' AND 
                          rd.DocumentType = 'Non Site Travel')
                      GROUP BY rdh.DocumentId
                              ,rdh.ActionEmployeeId
                    ) completedDoc 
                    ON rnst.DocumentId = completedDoc.DocumentId
                    left JOIN Employee Manager ON completedDoc.ActionEmployeeId = Manager.Id
                    left JOIN (SELECT Cost, DocumentId FROM RequestNonSiteTravelOption 
                    WHERE Selected = 1) costData
                    ON costData.DocumentId = rnst.DocumentId
                    ORDER BY travelDate.travelFirstDate";


            foreach (var item in reportParams)
            {

                var ItemData = await GetParametervalue(item);
      
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
            query = query.Replace("{STARTDATE}", $"{startDate.ToString("yyyy-MM-dd")}").Replace("{ENDDATE}", $"{endDate.ToString("yyyy-MM-dd")}");


            return await SaveDynamicListToExcelFile(query, @"\Assets\GeneratedFiles\", "Non Site Travel", reportParams);

        }

        #endregion


    }

}
