using Application.Common.Exceptions;
using Application.Features.OtinfoFeature.CheckTransport;
using Application.Features.OtinfoFeature.JobInfo;
using Application.Features.OtinfoFeature.ManualSent;
using Application.Features.TransportFeature.TransportInfo;
using Application.Repositories;
using Application.Service;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Persistence.Context;
using Persistence.HostedService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Persistence.HostedService.JobHostedService;

namespace Persistence.Repositories
{

    public class TransportInfoRepository : ITransportInfoRepository
    {
        protected readonly DataContext _context;
        ILogger <TransportInfoRepository> _logger;
        JobHostedService _hostedService;

        private const string OTINFO_API_KEY = "f3320226d1f9704fd3152ae0b1bdfa286d3f1d3be2be5fd0c86a309938952e78d6bb0c828b5a3a2cc5852c5fd4e4682ed71320dbdd3e3d8ff240caf2bac50518";
        private const string OTINFO_API_SEND_URL = "https://smartot.mn/tasapi/import";
        private const string OTINFO_API_CHECKTRANSPORT_URL = "https://smartot.mn/tasapi/check";
        private const string OTINFO_API_EMPTYDATA_URL = "https://smartot.mn/tasapi/empty";



        IConfiguration _configuration;

        public TransportInfoRepository(DataContext context, ILogger<TransportInfoRepository> logger, JobHostedService hostedService, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _hostedService = hostedService;
            _configuration = configuration;
        }



        public async Task<List<TransportInfoResponse>> EmployeeTransportInfo() 
        {
            try
            {
                var query = @"WITH RankedFlights AS (
                        SELECT 
                            e.SAPID,  
                            t.EventDate, 
                            t.Direction, 
                            ts.Description AS ScheduleDescription, 
                            tm.Code AS TransportMode, 
                            r.Number AS RoomNumber,
                            c.code Carrier,
                            ROW_NUMBER() OVER (PARTITION BY e.SAPID ORDER BY t.EventDate) AS FlightRank
                        FROM 
                            Transport t
                        LEFT JOIN 
                            TransportSchedule ts ON t.ScheduleId = ts.id
                        LEFT JOIN 
                            ActiveTransport at ON t.ActiveTransportId = at.Id
                        LEFT JOIN 
                            Carrier c ON at.CarrierId = c.Id
                        LEFT JOIN 
                            TransportMode tm ON at.TransportModeId = tm.Id
                        LEFT JOIN 
                            EmployeeStatus es ON CONVERT(DATE, es.EventDate) = CONVERT(DATE,t.EventDate) AND t.EmployeeId = es.EmployeeId
                        LEFT JOIN 
                            Room r ON es.RoomId = r.Id
                        LEFT JOIN 
                            Employee e ON t.EmployeeId = e.Id
                        WHERE 
                            t.EventDate >= GETDATE() AND e.SAPID is not NULL
                    )
                    SELECT 
                        0 No,
                        SAPID, 
                        EventDate, 
                        Direction, 
                        ScheduleDescription, 
                        TransportMode, 
                        Carrier,
                        RoomNumber
                    FROM 
                        RankedFlights
                    WHERE 
                        FlightRank <= 6
                    ORDER BY 
                        SAPID, EventDate"
                ;



                return await GetRawQueryData<TransportInfoResponse>(query, new CancellationToken());

                //return transportInfoList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new List<TransportInfoResponse>();
            }
           
        }


        #region JobSchedule



        public async Task SendTransportData(CancellationToken cancellationToken)
        {

            try
            {

                await SendEmptyData();
                _logger.LogError("OTINFO TRANSPORT DATA SENT-----BEGIN----------");

                var httpClient = new HttpClient();
                var dataToSend = (await EmployeeTransportInfo()).ToList();

                int batchSize = _configuration.GetSection("AppSettings:OTINFO:Batchsize").Get<int>(); ;


                int totalBatches = (int)Math.Ceiling((double)dataToSend.Count / batchSize);
                for (int i = 0; i < totalBatches; i++)
                {
                    var batch = dataToSend.Skip(i * batchSize).Take(batchSize).ToList();
                    var content = new MultipartFormDataContent
                    {
                        { new StringContent(OTINFO_API_KEY), "apikey" },
                        { new StringContent(JsonConvert.SerializeObject(batch), Encoding.UTF8, "application/json"), "data" }
                    };

                    HttpResponseMessage response = await httpClient.PostAsync(OTINFO_API_SEND_URL, content, cancellationToken);

                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    _logger.LogError($"Batch {i + 1}/{totalBatches} sent. API response: {responseBody}");

                    Console.WriteLine($"Batch {i + 1}/{totalBatches} sent. API response: {responseBody}");

                }
                _logger.LogError("OTINFO TRANSPORT DATA SENT-----END----------");
            }
            catch (Exception ex)
            {
                _logger.LogError("OTINFO TRANSPORT DATA SENT-----ERROR----------" + ex.ToString());
            }




        }



        #endregion


        #region ManualSent



        public async Task ManualSentData(ManualSentRequest request, CancellationToken cancellationToken)
        {
            try
            {

                await SendEmptyData();
                _logger.LogError("OTINFO TRANSPORT DATA MANUAL SENT-----BEGIN----------");

                var httpClient = new HttpClient();
                var dataToSend = request.testmode ? (await EmployeeTransportInfo()).ToList() : (await EmployeeTransportInfo()).ToList();

                int batchSize =request.batchsize;


                int totalBatches = (int)Math.Ceiling((double)dataToSend.Count / batchSize);
                for (int i = 0; i < totalBatches; i++)
                {
                    var batch = dataToSend.Skip(i * batchSize).Take(batchSize).ToList();
                    var content = new MultipartFormDataContent
                    {
                        { new StringContent(OTINFO_API_KEY), "apikey" },
                        { new StringContent(JsonConvert.SerializeObject(batch), Encoding.UTF8, "application/json"), "data" }
                    };

                    HttpResponseMessage response = await httpClient.PostAsync(OTINFO_API_SEND_URL, content, cancellationToken);

                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    _logger.LogError($"Batch {i + 1}/{totalBatches} sent. API response: {responseBody}");

                    Console.WriteLine($"Batch {i + 1}/{totalBatches} sent. API response: {responseBody}");

                }
                _logger.LogError("OTINFO TRANSPORT DATA MANUAL SENT-----END----------");
            }
            catch (Exception ex)
            {
                _logger.LogError("OTINFO TRANSPORT DATA MANUAL SENT-----ERROR----------" + ex.ToString());
            }




        }


        #endregion


        #region EmptySendData


        private async Task SendEmptyData()
        {

            try
            {
                var httpClient = new HttpClient();
                var content = new MultipartFormDataContent
                    {
                        { new StringContent(OTINFO_API_KEY), "apikey" }
                    };

                HttpResponseMessage response = await httpClient.PostAsync(OTINFO_API_EMPTYDATA_URL, content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogError("OTINFO TRANSPORT EMPTY DATA SENT-----SUCCESS----------" + responseBody);


            }
            catch (Exception ex)
            {
                _logger.LogError("OTINFO TRANSPORT EMPTY DATA SENT-----ERROR----------" + ex.ToString());
            }
        }

        #endregion


        #region CheckData

        public async Task<string> CheckTransport(CheckTransportRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var httpClient = new HttpClient();


                    var content = new MultipartFormDataContent
                    {
                        { new StringContent(OTINFO_API_KEY), "apikey" },
                        { new StringContent(request.SAPID), "sap" }
                    };

                    HttpResponseMessage response = await httpClient.PostAsync(OTINFO_API_CHECKTRANSPORT_URL, content, cancellationToken);

                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();


                    return responseBody;
            }
            catch (Exception ex)
            {
                _logger.LogError("OTINFO TRANSPORT DATA MANUAL SENT-----ERROR----------" + ex.ToString());

                return ex.Message;
            }



        }


        #endregion


        #region JobInfo

        public async Task<List<JobInfoResponse>> JobInfo(JobInfoRequest request, CancellationToken cancellationToken)
        {
            var jobs =await _hostedService.GetRunningJobsInfo();
            return jobs;
        }

        #endregion


        #region RawQueryExecute

        private async Task<List<T>> GetRawQueryData<T>(string query, CancellationToken cancellationToken)
        {
            if (_context.Database.GetDbConnection() is SqlConnection sqlConnection)
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
                             int rowNumber = 0;
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


        public async Task LoadData()
        {
            try
            {

                //Тодорхой минут бүр синк хийх шаардлагатай => OTINFO APP
                var otinfoEmployeeDataSyncTimeQuery = _configuration.GetSection("AppSettings:OTINFO:Schedule:IntervalMinute").Get<int>();
                await _hostedService.ScheduleEmployeeDataSyncJob(otinfoEmployeeDataSyncTimeQuery);


                //Өдөр болгоны тодорхой цагуудад 2025-09-26 Double => OTDESK SYSTEM

                var otdeskEmployeeSyncTimequery = _configuration.GetSection("AppSettings:HOTDESK:Schedule:EmployeeSyncScheduleTime").Get<string>();
                await _hostedService.ScheduleEmployeeSyncJob(otdeskEmployeeSyncTimequery == null ? "0 30 01 * * ?" : otdeskEmployeeSyncTimequery);


                var otdeskEmployeeStatusSyncTimequery = _configuration.GetSection("AppSettings:HOTDESK:Schedule:EmployeeStatusSyncScheduleTime").Get<string>();
                await _hostedService.ScheduleEmployeeStatusSyncJob(otdeskEmployeeStatusSyncTimequery == null ? "0 40 01 * * ?" : otdeskEmployeeStatusSyncTimequery);

                var otdeskdepartmentSyncTimequery = _configuration.GetSection("AppSettings:HOTDESK:Schedule:DepartmentSyncScheduleTime").Get<string>();
                await _hostedService.ScheduleDepartmentSyncJob(otdeskdepartmentSyncTimequery == null ? "0 0 02 * * ?" : otdeskdepartmentSyncTimequery);


            }
            catch (Exception ex)
            {
                _logger.LogError("OTINFO TRANSPORT GET INTERVAL MINUTE-----ERROR----------" + ex.ToString()); 
            }

        }



    }

}
