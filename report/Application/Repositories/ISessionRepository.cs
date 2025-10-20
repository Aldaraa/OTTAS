using Domain.CustomModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface ISessionRepository
    {
        Task AddSessionAsync(SessionInfo session);
        Task<SessionInfo?> GetSessionAsync(int sessionId);
        Task<List<SessionInfo>> GetAllSessionsAsync();
        Task<List<SessionInfo>> GetSessionsByKillIdAsync(int killId);
        Task RemoveSessionsByKillIdAsync(int killId, string? connectionString);

        Task RemoveSessionsByKillIdLocal(int killId);
    }
}
