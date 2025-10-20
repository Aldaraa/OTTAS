using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysResponseTimeFeature.DeleteSysResponseTime;
using tas.Application.Repositories;
using tas.Application.Utils;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public partial class SysResponseTimeRepository : BaseRepository<SysResponseTime>, ISysResponseTimeRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        public SysResponseTimeRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
        }


        public async Task DeleteOldData(DeleteSysResponseTimeRequest request, CancellationToken cancellationToken)
        {
            var threshold = DateTime.UtcNow.AddHours(-72);
            var oldRecords =await Context.SysResponseTime
                .Where(r => r.DateDeleted == null && r.DateCreated <= threshold)
            .ToListAsync();
            Context.SysResponseTime.RemoveRange(oldRecords);
        }
    }


}