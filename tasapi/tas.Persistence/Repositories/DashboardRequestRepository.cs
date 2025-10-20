using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.DashboardRequestFeature.GetDocumentDashboard;
using tas.Application.Features.DashboardSystemAdminFeature.GeOnsiteEmployeesData;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class DashboardRequestRepository : BaseRepository<Employee>, IDashboardRequestRepository
    {
        public DashboardRequestRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
        }

        
        public async Task<GetDocumentDashboardResponse> GetDocumentDashboard(GetDocumentDashboardRequest request, CancellationToken cancellationToken)
        {
            var returnData = new GetDocumentDashboardResponse();
            var DocumentDashboardMonthData = new List<GetDocumentDashboardMonthData>();
            var DocumentDashboardRequestData = new List<GetDocumentDashboardRequestData>();

            DateTime today = DateTime.Today;

            DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var monthRequest = await Context.RequestDocument.Where(x => x.DateCreated >= startOfMonth && x.DateCreated <= endOfMonth).ToListAsync();
            var documentTypeList = new List<string>();
            documentTypeList.Add(RequestDocumentType.NonSiteTravel);
            documentTypeList.Add(RequestDocumentType.SiteTravel);
            documentTypeList.Add(RequestDocumentType.DeMobilisation);
            documentTypeList.Add(RequestDocumentType.ProfileChanges);

            DateTime currentDate = startOfMonth;
            while (currentDate <= endOfMonth)
            {
                foreach (var item in documentTypeList)
                {
                    var monthreqestCurrentData = monthRequest.Where(x => x.DocumentType == item && x.DateCreated.Value.Date.Day == currentDate.Day).Count();
                    var newRecord = new GetDocumentDashboardMonthData
                    {
                        day = currentDate.Day,
                        value = monthreqestCurrentData,
                        documentType = item
                    };

                    DocumentDashboardMonthData.Add(newRecord);
                }

                currentDate = currentDate.AddDays(1);
            }

            var actonTypes = new List<string>();
            actonTypes.Add(RequestDocumentAction.Submitted);
            actonTypes.Add(RequestDocumentAction.Approved);
            actonTypes.Add(RequestDocumentAction.Cancelled);
            actonTypes.Add(RequestDocumentAction.Completed);
            actonTypes.Add(RequestDocumentAction.Declined);



            foreach (var item in documentTypeList)
            {
                foreach (var itemactions in actonTypes)
                {
                    var count = monthRequest.Where(x => x.DocumentType == item && x.CurrentAction == itemactions).Count();
                    var newRecord = new GetDocumentDashboardRequestData
                    {
                        CurrentStatus = itemactions,
                        count = count,
                        documentType = item
                    };

                    DocumentDashboardRequestData.Add(newRecord);
                }

            }



            returnData.monthData = DocumentDashboardMonthData;
            returnData.totalRequests = DocumentDashboardRequestData;
            returnData.SiteTravelDeclined = await GetDocumentDashboardSiteTravelDeclined(request.startdate, request.endDate, cancellationToken);
            returnData.PendingDocumentDaysAway = await GetDocumentDashboardPendingDayAway(request.startdate, request.endDate, cancellationToken);



            return returnData;
        }


        private async Task<List<GetDocumentDashboardSiteTravelDeclined>> GetDocumentDashboardSiteTravelDeclined(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken)
        {
            DateTime sDate = DateTime.Now.AddMonths(-1);
            DateTime eDate = DateTime.Today;
            if (startDate.HasValue)
            {
                sDate = startDate.Value;
            }

            if (endDate.HasValue)
            {
                eDate = endDate.Value;
            }

            var startdatestring = sDate.ToString("yyyy-MM-dd");
            var enddatestring = eDate.ToString("yyyy-MM-dd");


            var query = @$"WITH SoundexGroups AS (
                                            SELECT 
                                                SOUNDEX(ISNULL(Comment, 'NO COMMENT')) AS SoundexCode,
                                                ISNULL(Comment, 'NO COMMENT') AS Comment,
                                                COUNT(*) AS Count  
                                            FROM 
                                                RequestDocumentHistory rdh 
                                            WHERE 
                                                rdh.DocumentId IN (
                                                    SELECT Id 
                                                    FROM RequestDocument rd 
                                                    WHERE rd.CurrentAction = '{RequestDocumentAction.Declined}' 
                                                    AND rd.DocumentType = '{RequestDocumentType.SiteTravel}' 
                                                    AND rd.DateCreated >= '{startdatestring}' 
                                                    AND rd.DateCreated <= '{enddatestring}'
                                                )
                                                AND rdh.CurrentAction = '{RequestDocumentAction.Declined}'
                                            GROUP BY 
                                                SOUNDEX(ISNULL(Comment, 'NO COMMENT')), ISNULL(Comment, 'NO COMMENT')
                                        )
                                        SELECT 
                                            MIN(Comment) AS Comment,
                                            SUM(Count) AS Count
                                        FROM 
                                            SoundexGroups
                                        GROUP BY 
                                            SoundexCode
                                        ORDER BY 
                                            Count DESC";


            //string query = @$"SELECT ISNULL(Comment, 'NO COMMENT') Comment, COUNT(*) Count  from RequestDocumentHistory rdh WHERE rdh.DocumentId IN (
            //                SELECT Id from RequestDocument rd WHERE rd.CurrentAction = '{RequestDocumentAction.Declined}' AND rd.DocumentType = '{RequestDocumentType.SiteTravel}' AND rd.DateCreated >= '{startdatestring}' AND rd.DateCreated <= '{enddatestring}')
            //                AND rdh.CurrentAction = '{RequestDocumentAction.Declined}'
            //                GROUP BY rdh.Comment;
            //                ";

            return await GetRawQueryData<GetDocumentDashboardSiteTravelDeclined>(query, cancellationToken);
        }


        
        private async Task<List<GetDocumentDashboardPendingDayAway>> GetDocumentDashboardPendingDayAway(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken)
        {
            DateTime sDate = DateTime.Now.AddMonths(-1);
            DateTime eDate = DateTime.Today;
            if (startDate.HasValue)
            {
                sDate = startDate.Value;
            }

            if (endDate.HasValue)
            {
                eDate = endDate.Value;
            }

            var startdatestring = sDate.ToString("yyyy-MM-dd");
            var enddatestring = eDate.ToString("yyyy-MM-dd");


            string query = @$"SELECT rd.DocumentType, count(*) Count, DATEDIFF(DAY, rd.DaysAwayDate, GETDATE()) DaysAway FROM RequestDocument rd WHERE rd.DateCreated >= '{startdatestring}' AND rd.DateCreated <= '{enddatestring}'
                                AND rd.CurrentAction NOT IN('Completed', 'Cancelled')
                                GROUP BY rd.DocumentType, DATEDIFF(DAY, rd.DaysAwayDate, GETDATE())
                                order BY  DATEDIFF(DAY, rd.DaysAwayDate, GETDATE()) asc;
                            ";

            return await GetRawQueryData<GetDocumentDashboardPendingDayAway>(query, cancellationToken);
        }


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


