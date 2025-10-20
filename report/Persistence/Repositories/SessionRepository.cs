using Application.Common.Exceptions;
using Application.Repositories;
using Domain.CustomModel;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Persistence.Context;
using Persistence.HostedService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class SessionRepository :  BaseRepository<ReportJob>, ISessionRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;


        public SessionRepository(DataContext context, IMemoryCache cache, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(context, configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _cache = cache;
        }


        public async Task AddSessionAsync(SessionInfo session)
        {
            var sessions = await GetAllSessionsAsync();
            if (sessions != null)
            {
                sessions.Add(session);
                _cache.Set("Sessions", sessions);
            }
            else
            {
                sessions = new List<SessionInfo>();
                sessions.Add(session);
                _cache.Set("Sessions", sessions);
            }

        }

        public async Task<SessionInfo?> GetSessionAsync(int sessionId)
        {
            var sessions = await GetAllSessionsAsync();
            return sessions.FirstOrDefault(s => s.SessionId == sessionId);
        }

        public async Task<List<SessionInfo>> GetAllSessionsAsync()
        {

            var outData =new List<SessionInfo>();

            if (_cache.TryGetValue($"Sessions", out outData))
            {
                return outData;
            }
            else {
                return outData;
            }


        }

        public async Task<List<SessionInfo>> GetSessionsByKillIdAsync(int killId)
        {
            var sessions = await GetAllSessionsAsync();
            return sessions.Where(s => s.KillId == killId).ToList();
        }


        public async Task  RemoveSessionsByKillIdLocal(int killId)
        {
            var sessions = await GetAllSessionsAsync();
            var currentSession = sessions.Where(c => c.KillId == killId).FirstOrDefault();
            if (currentSession != null)
            {
                sessions.RemoveAll(s => s.KillId == killId);
                _cache.Set("Sessions", sessions);
            }
        }


        public async Task RemoveSessionsByKillIdAsync(int killId, string connectionString)
        {
            var sessions = await GetAllSessionsAsync();
            var currentSession = sessions.Where(c => c.KillId == killId).FirstOrDefault();
            if (currentSession != null)
            {

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = $"KILL {currentSession.SessionId}";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            int rowsAffected = await command.ExecuteNonQueryAsync();
                            if (rowsAffected > 0)
                            {
   
                                sessions.RemoveAll(s => s.KillId == killId);
                                _cache.Set("Sessions", sessions);

                            }
                            else
                            {
                                sessions.RemoveAll(s => s.KillId == killId);
                                _cache.Set("Sessions", sessions);
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
             //       throw new BadRequestException("SQL Error: " + ex.Message);
                }
                catch (Exception ex)
                {
                 //   throw new BadRequestException("Session error " + ex.Message);
                }
            }
            else
            {
                throw new BadRequestException("Session already finished");
            }
        }

        
        

    }
}
