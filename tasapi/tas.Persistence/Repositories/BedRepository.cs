using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.BedFeature.GetAllBed;
using tas.Application.Features.BedFeature.GetBed;
using tas.Application.Features.RoomFeature.GetAllRoom;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class BedRepository : BaseRepository<Bed>, IBedRepository
    {
        private readonly IConfiguration _configuration;
        public BedRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
        }

        public async Task<List<GetAllBedResponse>> GetAllBed(int roomId, CancellationToken cancellationToken)
        {
            var result = await Context.Bed
                             .Join(Context.Room,
                                 bed => bed.RoomId,
                                 room => room.Id,
                                 (bed, room) => new { bed, room })
                             .Join(Context.Camp,
                               room => room.room.CampId,
                               camp => camp.Id,
                               (room, camp) => new GetAllBedResponse
                               {
                                   Id = room.bed.Id,
                                   Description = room.bed.Description,
                                   Active = room.bed.Active,
                                   CampName = camp.Description,
                                   RoomNumber = room.room.Number,
                                   RoomId = room.room.Id,
                                   DateCreated = room.bed.DateCreated,
                                   DateUpdated = room.bed.DateUpdated

                               }).ToListAsync(cancellationToken);

            return result.Where(x => x.RoomId == roomId).ToList();
        }


        public async Task<GetBedResponse?> Get(int Id, CancellationToken cancellationToken)
        {
            return await Context.Bed.Where(x => x.Id == Id)
                             .Join(Context.Room,
                                 bed => bed.RoomId,
                                 room => room.Id,
                                 (bed, room) => new { bed, room })
                             .Join(Context.Camp,
                               room => room.room.CampId,
                               camp => camp.Id,
                               (room, camp) => new GetBedResponse
                               {
                                   Id = room.bed.Id,
                                   Description = room.bed.Description,
                                   Active = room.bed.Active,
                                   CampName = camp.Description,
                                   RoomNumber = room.room.Number,
                                   DateCreated = room.bed.DateCreated,
                                   DateUpdated = room.bed.DateUpdated

                               }).FirstOrDefaultAsync(cancellationToken);
        }

        //public async Task<GetBedResponse> Get(int Id, CancellationToken cancellationToken)
        //{
        //   return await Context.Bed.Where(x=> x.Id == Id)
        //                     .Join(Context.Room,
        //                         bed => bed.RoomId,
        //                         room => room.Id,
        //                         (bed, room) => new { bed, room })
        //                     .Join(Context.Camp,
        //                       room => room.room.CampId,
        //                       camp => camp.Id,
        //                       (room, camp) => new GetBedResponse
        //                       {
        //                           Id = room.bed.Id,
        //                           Description = room.bed.Description,
        //                           Active = room.bed.Active,
        //                           CampName = camp.Description,
        //                           RoomNumber = room.room.Number,
        //                           DateCreated = room.bed.DateCreated,
        //                           DateUpdated = room.bed.DateUpdated

        //                       }).FirstOrDefaultAsync(cancellationToken);
        //}


        public async Task<List<GetAllBedResponse>> GetAllBedFilter(int? status, int roomId, CancellationToken cancellationToken)
        {


            var result = await Context.Bed
                             .Join(Context.Room,
                                 bed => bed.RoomId,
                                 room => room.Id,
                                 (bed, room) => new { bed, room })
                             .Join(Context.Camp,
                               room => room.room.CampId,
                               camp => camp.Id,
                               (room, camp) => new GetAllBedResponse
                               {
                                   Id = room.bed.Id,
                                   Description = room.bed.Description,
                                   Active = room.bed.Active,
                                   CampName = camp.Description,
                                   RoomNumber = room.room.Number,
                                   DateCreated = room.bed.DateCreated,
                                   DateUpdated = room.bed.DateUpdated

                               }).ToListAsync(cancellationToken);
            if (status.HasValue)
            {
                return result.Where(x => x.Active == status && x.RoomId == roomId).ToList();
            }
            else
            {
                return result.Where(x => x.RoomId == roomId).ToList();
            }

        }


  
    }
}
