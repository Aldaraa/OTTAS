using Application.Features.HotDeskFeature.EmployeeInfo;
using Application.Features.HotDeskFeature.EmployeeSend;
using Application.Features.HotDeskFeature.EmployeeStatusInfo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Features.HotDeskFeature.EmployeeStatusSend;
using Application.Features.HotDeskFeature.EmployeeStatusInfoById;

namespace Persistence.Repositories
{
    public partial class HotDeskRepository
    {

        #region EmployeeStatusInfoList


        public async Task<List<EmployeeStatusInfoResponse>> EmployeeStatusInfo()
        {
            try
            {
                var query = @"SELECT es.Id, es.EmployeeId UserId, es.EventDate, s.Code ShiftCode, s.OnSite, c.Code ShiftColor FROM EmployeeStatus es
                                left JOIN Shift s ON es.ShiftId = s.Id
                                inner JOIN Employee e ON es.EmployeeId = e.Id AND e.ADAccount is not NULL
                                left join Color c ON s.ColorId = c.Id
                                WHERE CAST(es.EventDate as DATE) >= GETDATE();";


                var data = await GetRawQueryData<EmployeeStatusInfoResponse>(query, new CancellationToken());
                return data;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new List<EmployeeStatusInfoResponse>();
            }

        }

        #endregion


        public async Task<List<EmployeeStatusInfoByIdResponse>> EmployeeStatusInfoById(EmployeeStatusInfoByIdRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var query = @$"SELECT es.Id, es.EmployeeId UserId, es.EventDate, s.Code ShiftCode, s.OnSite, c.Code ShiftColor FROM EmployeeStatus es
                                left JOIN Shift s ON es.ShiftId = s.Id
                                inner JOIN Employee e ON es.EmployeeId = e.Id AND e.ADAccount is not NULL
                                left join Color c ON s.ColorId = c.Id
                                WHERE CAST(es.EventDate as DATE) >= GETDATE() and es.EmployeeId = {request.employeeId};";


                var data = await GetRawQueryData<EmployeeStatusInfoByIdResponse>(query, new CancellationToken());
                return data;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new List<EmployeeStatusInfoByIdResponse>();
            }
        }

        public async Task EmployeeStatusSendData(EmployeeStatusSendRequest request, CancellationToken cancellationToken)
        {
            await SendEmployeeStatusEmptyData(@"/api/EmployeeStatusInfo");
            var employeeList = await EmployeeStatusInfo();
            var dataToSend = employeeList.ToList();
            int batchSize = _configuration.GetSection("AppSettings:HOTDESK:Batchsize").Get<int>();
            if (batchSize < 100)
            {
                batchSize = 1000;
            }
            int totalBatches = (int)Math.Ceiling((double)dataToSend.Count / batchSize);

            string domain = _configuration.GetSection("AppSettings:HOTDESK:domain").Get<string>();
            string username = _configuration.GetSection("AppSettings:HOTDESK:username").Get<string>();
            string password = _configuration.GetSection("AppSettings:HOTDESK:password").Get<string>();
            string hotdeskmainapiurl = _configuration.GetSection("AppSettings:HOTDESK:hotdeksapiurl").Get<string>();


            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(username, password, domain)
            };

            using var httpClient = new HttpClient(handler); // Use 'using' to dispose HttpClient
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var sendCount = 0;

            for (int i = 0; i < totalBatches; i++)
            {
                var batch = dataToSend.Skip(i * batchSize).Take(batchSize).ToList();
                var payload = new { EmployeeStatuses = batch }; // Wrap batch in "Employees" array
                var content = new StringContent(
                    JsonConvert.SerializeObject(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                try
                {

                    var root = "/api/EmployeeStatusInfo";
                    var baseUrl = hotdeskmainapiurl.TrimEnd('/');
                    var path = root.TrimStart('/');
                    var url = $"{baseUrl}/{path}";

                    HttpResponseMessage response = await httpClient.PostAsync(url, content, cancellationToken);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Batch {i + 1}/{totalBatches} sent. API response: {responseBody}");
                    Console.WriteLine($"Batch {i + 1}/{totalBatches} sent. API response: {responseBody}");
                    sendCount += batch.Count;

                    Console.WriteLine($"=================================COUNT ==================== {sendCount}==========================");
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, $"Failed to send batch {i + 1}/{totalBatches}");
                    throw; // Or handle gracefully based on requirements
                }
            }


            _logger.LogInformation("EMPLOYEESTATUS DATA SENT-----END----------");


        }


        #region EmptySendData




        private async Task SendEmployeeStatusEmptyData(string root)
        {

            try
            {

                string domain = _configuration.GetSection("AppSettings:HOTDESK:domain").Get<string>();
                string username = _configuration.GetSection("AppSettings:HOTDESK:username").Get<string>();
                string password = _configuration.GetSection("AppSettings:HOTDESK:password").Get<string>();
                string hotdeskmainapiurl = _configuration.GetSection("AppSettings:HOTDESK:hotdeksapiurl").Get<string>();


                var handler = new HttpClientHandler
                {
                    UseDefaultCredentials = true,
                    Credentials = new NetworkCredential(username, password, domain)
                };

                var baseUrl = hotdeskmainapiurl.TrimEnd('/');
                var path = root.TrimStart('/');
                var url = $"{baseUrl}/{path}";

                var httpClient = new HttpClient(handler);
                HttpResponseMessage response = await httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogError("HOTDESK EMPLOYEESTATUS EMPTY DATA SENT-----SUCCESS----------" + responseBody);


            }
            catch (Exception ex)
            {
                _logger.LogError("HOTDESK EMPLOYEESTATUS EMPTY DATA SENT-----ERROR----------" + ex.ToString());
            }
        }

        #endregion
    }
}
