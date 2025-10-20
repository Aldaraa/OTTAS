using Application.Features.HotDeskFeature.EmployeeInfo;
using Application.Features.HotDeskFeature.EmployeeSend;
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
using Application.Features.HotDeskFeature.EmployeeInfoById;

namespace Persistence.Repositories
{
    public partial class HotDeskRepository
    {
        #region EmployeeList


        public async Task<List<EmployeeInfoResponse>> EmployeeInfo()
        {
            try
            {
                var query = @"SELECT e.Id EmployeeId, e.Lastname, e.Firstname, e.Email, e.SAPID, e.ADAccount, 
                                d.Name DepartmentName, e1.Description Employer, 
                                p.Description PositionName, e.PeopleTypeId, pt.Description PeopleTypeName, 
                                e.DepartmentId, e.Active FROM Employee e
                              left JOIN Department d ON e.DepartmentId = d.Id
                              left join Employer e1 ON e.employerId = e1.Id
                              left JOIN Position p ON e.PositionId = p.Id
                              left JOIN PeopleType pt ON e.PeopleTypeId = pt.Id
                                WHERE e.Active = 1 AND e.ADAccount is NOT NULL";


                var data = await GetRawQueryData<EmployeeInfoResponse>(query, new CancellationToken());
                return data;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new List<EmployeeInfoResponse>();
            }

        }

        #endregion


        #region EmployeeById


        public async Task<EmployeeInfoByIdResponse> EmployeeInfoById(EmployeeInfoByIdRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var query = @$"SELECT e.Id EmployeeId, e.Lastname, e.Firstname, e.Email, e.SAPID, e.ADAccount, 
                                d.Name DepartmentName, e1.Description Employer, 
                                p.Description PositionName, e.PeopleTypeId, pt.Description PeopleTypeName, 
                                e.DepartmentId, e.Active FROM Employee e
                              
                              left JOIN Department d ON e.DepartmentId = d.Id
                              left join Employer e1 ON e.employerId = e1.Id
                              left JOIN Position p ON e.PositionId = p.Id
                              left JOIN PeopleType pt ON e.PeopleTypeId = pt.Id
                              WHERE e.Id = {request.employeeId}";


                var data = await GetRawQueryData<EmployeeInfoByIdResponse>(query, cancellationToken);
                if (data.Count > 0)
                {
                    return data[0];
                }
                else {
                    return new EmployeeInfoByIdResponse();
                }
                

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new EmployeeInfoByIdResponse();
            }

        }

        #endregion


        public async Task EmployeeSendData(EmployeeSendRequest request, CancellationToken cancellationToken)
        {
            await SendEmployeeEmptyData(@"/api/EmployeeInfo");
            var employeeList = await EmployeeInfo();
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
                var payload = new { Employees = batch }; // Wrap batch in "Employees" array
                var content = new StringContent(
                    JsonConvert.SerializeObject(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                try
                {
                    var root = "/api/EmployeeInfo";
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

            await SendEmployeeDeactiveCleanup(@"/api/EmployeeInfo/deactivecleanup");
            _logger.LogInformation("EMPLOYEE DATA SENT-----END----------");
                

        }


        #region EmptySendData

        private async Task SendEmployeeEmptyData(string root)
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

                var httpClient = new HttpClient(handler);
                HttpResponseMessage response = await httpClient.DeleteAsync($"{hotdeskmainapiurl}{root}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogError("HOTDESK EMPLOYEE EMPTY DATA SENT-----SUCCESS----------" + responseBody);


            }
            catch (Exception ex)
            {
                _logger.LogError("HOTDESK EMPLOYEE EMPTY DATA SENT-----ERROR----------" + ex.ToString());
            }
        }

        #endregion

        #region DeActiveCleanup


        private async Task SendEmployeeDeactiveCleanup(string root= @"/api/EmployeeInfo/deactivecleanup")
        {

            try
            {
              //  deactivecleanup
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
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogError("HOTDESK EMPLOYEE  DEACTIVE CLEANUP REQUEST SENT-----SUCCESS----------" + responseBody);


            }
            catch (Exception ex)
            {
                _logger.LogError("HOTDESK EMPLOYEE  DEACTIVE CLEANUP REQUEST SENT-----ERROR----------" + ex.ToString());
            }
        }

        #endregion





    }
}
