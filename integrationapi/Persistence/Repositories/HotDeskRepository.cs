
using Application.Common.Exceptions;
using Application.Features.HotDeskFeature.DepartmentInfo;
using Application.Features.HotDeskFeature.DepartmentSend;
using Application.Features.HotDeskFeature.EmployeeInfo;
using Application.Features.HotDeskFeature.EmployeeSend;
using Application.Features.TransportFeature.TransportInfo;
using Application.Repositories;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Persistence.Context;
using Persistence.HostedService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{


    public partial class HotDeskRepository : IHotDeskRepository
    {
        protected readonly DataContext _context;
        ILogger<HotDeskRepository> _logger;
        JobHostedService _hostedService;

        IConfiguration _configuration;

        //  private const string HOTDESK_API_KEY = "f3320226d1f9704fd3152ae0b1bdfa286d3f1d3be2be5fd0c86a309938952e78d6bb0c828b5a3a2cc5852c5fd4e4682ed71320dbdd3e3d8ff240caf2bac50518";
        //private const string HOTDESK_API_SEND_URL = "https://localhost:7121/api/EmployeeInfo";
        //private const string HOTDESK_API_EMPTYDATA_URL = "https://localhost:7121/api/EmployeeInfo";

        //   private const string HOTDESK_API_SEND_URL = "http://hotdeskapi.fractal.local/api/EmployeeInfo";
        //  private const string HOTDESK_API_EMPTYDATA_URL = "http://hotdeskapi.fractal.local/api/EmployeeInfo";

     //   private const string HOTDESK_MAIN_API =   "http://hotdeskapi.fractal.local"; // "https://localhost:7121"; // 

        public HotDeskRepository(DataContext context, ILogger<HotDeskRepository> logger, JobHostedService hostedService, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _hostedService = hostedService;
            _configuration = configuration;
        }

      

        #region DepartmentList


        public async Task<List<DepartmentInfoResponse>> DepartmentInfo()
        {
            try
            {
                var query = @"SELECT Id DepartmentId, Name, ParentDepartmentId FROM Department where Active = 1";


                var data = await GetRawQueryData<DepartmentInfoResponse>(query, new CancellationToken());
                return data;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new List<DepartmentInfoResponse>();
            }

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


   




    }
}
