using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysVersionFeature.CreateSysVersion;
using tas.Application.Features.SysVersionFeature.GetSysVersion;
using tas.Application.Features.SysVersionHistoryFeature.GetSysVersionHistory;
using tas.Application.Features.SysVersionNoteFeature.GetSysVersionNote;
using tas.Application.Repositories;
using tas.Domain.Common;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public class SysVersionRepository : BaseRepository<SysVersion>, ISysVersionRepository
    {
        private readonly IConfiguration _configuration;         
        private readonly HTTPUserRepository _HTTPUserRepository;
        public SysVersionRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
        }


        public async Task CreateVersion(CreateSysVersionRequest request, CancellationToken cancellationToken)
        { 
            var newversion = new SysVersion();
            newversion.Version = request.Version;
            newversion.ReleaseNote = request.ReleaseNote;
            newversion.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
            newversion.ReleaseDate =DateTime.Now;
            Context.SysVersion.Add(newversion);
        }


        public async Task<GetSysVersionResponse> GeVersion(GetSysVersionRequest request, CancellationToken cancellationToken)
        {
            await InitSystemVersion(cancellationToken);
            var returnData = new GetSysVersionResponse();
            var currentVersion =await Context.SysVersion.AsNoTracking().OrderByDescending(c => c.ReleaseDate).FirstOrDefaultAsync();
            if (currentVersion != null)
            {
                returnData.Id = currentVersion.Id;
                returnData.Version = currentVersion.Version;
                returnData.ReleaseDate = currentVersion.ReleaseDate;
                    
            }
            else {
                returnData.Id = 0;
                returnData.Version = "0.0.0";
                returnData.ReleaseDate = DateTime.Today;
            }
            return returnData;
        }


        public async Task<GetSysVersionNoteResponse> GeVersionNote(GetSysVersionNoteRequest request, CancellationToken cancellationToken)
        {
            var returnData = new GetSysVersionNoteResponse();
            var currentVersion = await Context.SysVersion.Where(x=> x.Id == request.Id).FirstOrDefaultAsync();
            if (currentVersion != null)
            {
                returnData.Id = currentVersion.Id;
                returnData.Version = currentVersion.Version;
                returnData.ReleaseDate = currentVersion.ReleaseDate;
                returnData.ReleaseNote = currentVersion.ReleaseNote;

            }
            else
            {
                returnData.Id = 0;
                returnData.Version = "0.0.0";
                returnData.ReleaseDate = DateTime.Today;
                returnData.ReleaseNote = "Version unable";
            }
            return returnData;
        }

        public async Task<List<GetSysVersionHistoryResponse>> GeVersionHistory(GetSysVersionHistoryRequest request, CancellationToken cancellationToken)
        {
            var returnData = await Context.SysVersion.OrderByDescending(x=> x.ReleaseDate).Take(3).Select(x => new GetSysVersionHistoryResponse
            {
                Id = x.Id,
                ReleaseDate = x.ReleaseDate,
                Version = x.Version
            }).ToListAsync();


            return returnData;
        }


        private async Task InitSystemVersion(CancellationToken cancellationToken)
        {
            

            var newversion = new SysVersion();
            newversion.Version = VersionInfo.Version;
            newversion.ReleaseNote = VersionInfo.VersionString;
            newversion.ReleaseDate = DateTime.Now;
            newversion.Active = 1;
            newversion.DateCreated = DateTime.Now;


            var currentVersion = await Context.SysVersion.AsNoTracking()
                 .Where(x => x.Version == newversion.Version)
                 .FirstOrDefaultAsync(cancellationToken);

            if (currentVersion == null)
            {
                Context.SysVersion.Add(newversion);
                await Context.SaveChangesAsync();

            }


        }



    }


}