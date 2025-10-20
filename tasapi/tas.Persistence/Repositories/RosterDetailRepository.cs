using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.BedFeature.GetBed;
using tas.Application.Features.RosterDetailFeature.GetRosterDetail;
using tas.Application.Features.RosterDetailFeature.UpdateRosterDetail;
using tas.Application.Features.RosterDetailFeature.UpdateSeqRosterDetail;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class RosterDetailRepository : BaseRepository<RosterDetail>, IRosterDetailRepository
    {
        private readonly IConfiguration _configuration;

        private readonly HTTPUserRepository _hTTPUserRepository;
        public RosterDetailRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _hTTPUserRepository = hTTPUserRepository;
        }

        //GetbyRosterId

        public async Task<List<GetRosterDetailResponse?>> GetbyRosterId(int rosterId, CancellationToken cancellationToken)
        {

            var rosterDetails = await (from rd in Context.RosterDetail.AsNoTracking().Where(x=> x.RosterId == rosterId)
                                       join r in Context.Roster.AsNoTracking() on rd.RosterId equals r.Id into rosterGroup
                                       from r in rosterGroup.DefaultIfEmpty()

                                       join s in Context.Shift.AsNoTracking() on rd.ShiftId equals s.Id into shiftGroup
                                       from s in shiftGroup.DefaultIfEmpty()

                                       join c in Context.Color.AsNoTracking() on s.ColorId equals c.Id into colorGroup
                                       from c in colorGroup.DefaultIfEmpty()

                                       select new GetRosterDetailResponse
                                       {
                                           Id = rd.Id,
                                           ShiftId = rd.ShiftId,
                                           SeqNumber = rd.SeqNumber,
                                           RosterId = rd.RosterId,
                                           RosterName = r.Name ?? string.Empty,
                                           ShiftColor = c.Code ?? string.Empty,
                                           ShiftCode = s.Code ?? string.Empty,
                                           ShiftName = s.Description ?? string.Empty,
                                           OnSite = s.OnSite,
                                           DaysOn = rd.DaysOn,
                                           Active = rd.Active,
                                           DateCreated = rd.DateCreated,
                                           DateUpdated = rd.DateUpdated
                                       })
                           .OrderBy(x => x.SeqNumber)
                           .ToListAsync(cancellationToken);


            //var rosterDetails = await Context.RosterDetail.AsNoTracking()
            //         .Where(rd => rd.RosterId == rosterId)
            //         .Select( rd => new GetRosterDetailResponse
            //         {
            //             Id = rd.Id,
            //             ShiftId = rd.ShiftId,
            //             SeqNumber = rd.SeqNumber,
            //             RosterId = rd.RosterId,
            //             RosterName =Context.Roster.AsNoTracking().FirstOrDefault(r => r.Id == rd.RosterId).Name ?? string.Empty,
            //             ShiftColor = Context.Color.AsNoTracking().FirstOrDefault(x => x.Id == Context.Shift.FirstOrDefault(s => s.Id == rd.ShiftId).ColorId).Code ?? String.Empty,
            //             ShiftCode = Context.Shift.AsNoTracking().FirstOrDefault(s => s.Id == rd.ShiftId).Code ?? string.Empty,
            //             ShiftName = Context.Shift.AsNoTracking().FirstOrDefault(s => s.Id == rd.ShiftId).Description ?? string.Empty,
            //             OnSite = Context.Shift.AsNoTracking().FirstOrDefault(s => s.Id == rd.ShiftId).OnSite,
            //             DaysOn = rd.DaysOn,
            //             Active = rd.Active,
            //             DateCreated = rd.DateCreated,
            //             DateUpdated = rd.DateUpdated
            //         }).OrderBy(x => x.SeqNumber)
            //         .ToListAsync(cancellationToken);

            return rosterDetails;

        }

        public async Task UpdateReorderSequeensNumber(UpdateSeqRosterDetailRequest request, CancellationToken cancellationToken = default)
        {
            var rosterDetails = await Context.RosterDetail
                .Where(rd => rd.RosterId == request.RosterId)
                .OrderBy(rd => rd.SeqNumber)
                .ToListAsync(cancellationToken);

            int RowNum = 1;
            foreach (int RosterDetailId in request.RosterDetailIds)
            {
                var currentData =  rosterDetails.FirstOrDefault(x => x.Id == RosterDetailId);
                if (currentData != null) { 
                    currentData.SeqNumber = RowNum;
                    Context.RosterDetail.Update(currentData);
                }
                RowNum++;
            }
            await Task.CompletedTask;
        }

        public async Task UpdateData(UpdateRosterDetailRequest request, CancellationToken cancellationToken)
        {
            var currentDetail = await Context.RosterDetail.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (currentDetail != null) {
                currentDetail.ShiftId = request.ShiftId;
                currentDetail.DaysOn = request.DaysOn;
                currentDetail.DateUpdated = DateTime.Now;
                currentDetail.UserIdUpdated = _hTTPUserRepository.LogCurrentUser().Id;

                Context.RosterDetail.Update(currentDetail);

            }

          await  Task.CompletedTask;
        }



    }

}
