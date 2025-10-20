using Application.Features.HotDeskFeature.DepartmentSend;
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

namespace Persistence.Repositories
{
    public partial class HotDeskRepository
    {

        public async Task DepartmentSendData(DepartmentSendRequest request, CancellationToken cancellationToken)
        {
            await SendDepartmentEmptyData(@"/api/DepartmentInfo");
            var employeeList = await DepartmentInfo();
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
                var payload = new { Departments = batch }; // Wrap batch in "Employees" array
                var content = new StringContent(
                    JsonConvert.SerializeObject(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                try
                {
                    var root = "/api/DepartmentInfo";
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

            await SendDepartmentDeactiveCleanup(@"/api/DepartmentInfo/deactivecleanup");
            _logger.LogInformation("OTDESK DEPARTMENT DATA SENT-----END----------");


        }




        #region EmptySendData

        private async Task SendDepartmentEmptyData(string root)
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

                using var httpClient = new HttpClient(handler);

                var baseUrl = hotdeskmainapiurl.TrimEnd('/');
                var path = root.TrimStart('/');
                var url = $"{baseUrl}/{path}";

                HttpResponseMessage response = await httpClient.DeleteAsync(url);

                _logger.LogInformation("DELETE request sent to {Url}", url);

                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("OTDESK DEPARTMENT EMPTY DATA SENT SUCCESS. Response={Response}", responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OTDESK DEPARTMENT EMPTY DATA SENT ERROR");
            }
        }

        #endregion


        #region DeActiveCleanup


        private async Task SendDepartmentDeactiveCleanup(string root = @"/api/DepartmentInfo/deactivecleanup")
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

                _logger.LogError("OTDESK DEPARMENT DEACTIVE CLEANUP REQUEST SENT-----SUCCESS----------" + responseBody);


            }
            catch (Exception ex)
            {
                _logger.LogError("OTDESK DEPARMENT DEACTIVE CLEANUP REQUEST SENT-----ERROR----------" + ex.ToString());
            }
        }

        #endregion

    }
}
