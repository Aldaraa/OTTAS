using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ClusterFeature.GetActiveTransportCluster;
using tas.Application.Features.ClusterFeature.GetAllCluster;
using tas.Application.Features.ClusterFeature.GetCluster;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public class ClusterRepository : BaseRepository<Cluster>, IClusterRepository
    {
        public ClusterRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
        }


        public async Task<List<GetAllClusterResponse>> GetAllData(GetAllClusterRequest request, CancellationToken cancellationToken)
        {

            if (request.status.HasValue)
            {
                var clusters =await Context.Cluster.AsNoTracking().Where(x => x.Active == request.status).ToListAsync();
                var returnData = clusters.Select(x => new GetAllClusterResponse
                {
                    Id = x.Id,
                    Active = x.Active,
                    Code = x.Code,
                    DayNum = x.DayNum,
                    Description = x.Description,
                    Direction = x.Direction,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated

                });


                foreach (var item in returnData)
                {
                    item.DetailCount = await Context.ClusterDetail.CountAsync(c => c.ClusterId == item.Id);
                }

                return await Task.FromResult(returnData.ToList());
            }
            else {
                var clusters =await Context.Cluster.ToListAsync();
                var returnData = clusters.Select(x => new GetAllClusterResponse
                {
                    Id = x.Id,
                    Active = x.Active,
                    Code = x.Code,
                    DayNum = x.DayNum,
                    Description = x.Description,
                    Direction = x.Direction,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    DetailCount = Context.ClusterDetail.Count(c => c.ClusterId == x.Id)

                });



                foreach (var item in returnData)
                {
                    item.DetailCount = await Context.ClusterDetail.CountAsync(c => c.ClusterId == item.Id);
                }
                return await Task.FromResult(returnData.ToList());
            }

        }

        public async Task<List<GetActiveTransportClusterResponse>> GetActiveTranports(GetActiveTransportClusterRequest request, CancellationToken cancellationToken)
        {

            var currentCluster = await Context.Cluster.AsNoTracking()
                                                      .FirstOrDefaultAsync(x => x.Id == request.Id);
            if (currentCluster == null)
            {
                return new List<GetActiveTransportClusterResponse>(); // Return empty if no cluster found
            }


            var driveModeId = (await Context.TransportMode
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(c => c.Code == "Drive"))?.Id;


            var usedTransports = await Context.ClusterDetail.AsNoTracking()
                                                            .Where(x => x.ClusterId == request.Id)
                                                            .Select(x => x.ActiveTransportId)
                                                            .ToArrayAsync();


            var activeTransportQuery = Context.ActiveTransport.AsNoTracking()
                                                              .Where(x => x.Active == 1 &&
                                                                          x.DayNum == currentCluster.DayNum &&
                                                                          x.Direction == currentCluster.Direction &&
                                                                          !usedTransports.Contains(x.Id) &&
                                                                          x.Special != 1);

            if (driveModeId.HasValue)
            {
                activeTransportQuery = activeTransportQuery.Where(x => x.TransportModeId != driveModeId);
            }

            var activeTransportIds = await activeTransportQuery.Select(x => x.Id).ToListAsync();

            var scheduledTransportIds = await Context.TransportSchedule
                                                     .AsNoTracking()
                                                     .Where(x => activeTransportIds.Contains(x.ActiveTransportId))
                                                     .GroupBy(x => x.ActiveTransportId)
                                                     .Select(g => new { ActiveTransportId = g.Key, Count = g.Count() })
                                                     .Where(x => x.Count > 1)
                                                     .Select(x => x.ActiveTransportId)
                                                     .ToListAsync();


            var activeData = await Context.ActiveTransport.AsNoTracking()
                                                          .Where(x => scheduledTransportIds.Contains(x.Id))
                                                          .ToListAsync();

            return activeData.Select(x => new GetActiveTransportClusterResponse
            {
                Id = x.Id,
                Code = x.Code,
                Description = $"{x.DayNum.Substring(0, 3)} {x.Description}",
                Active = x.Active,
                DayNum = x.DayNum,
                Direction = x.Direction,
                DateCreated = x.DateCreated,
                DateUpdated = x.DateUpdated
            }).ToList();

        }


        public async Task<GetClusterResponse> GetCluster(GetClusterRequest request, CancellationToken cancellationToken)
        {
                var currentCluster = await Context.Cluster.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.clusterId);
            if (currentCluster != null)
            {
                var currentClusterDetails = await Context.ClusterDetail.AsNoTracking().Where(x => x.ClusterId == request.clusterId).OrderBy(x=> x.SeqNumber).ToListAsync();

                var data = new List<GetClusterResponseDetail>();
                foreach (var detail in currentClusterDetails)
                {
                    var currentTransport = await Context.ActiveTransport.AsNoTracking().FirstOrDefaultAsync(x => x.Id == detail.ActiveTransportId);
                    if (currentTransport != null)
                    {
                        var newRecord = new GetClusterResponseDetail
                        {

                            Id = detail.Id,
                            SeqNumber = detail.SeqNumber ?? 0,
                            ActiveTransportId = detail.ActiveTransportId,
                            ActiveTransportDescription = currentTransport?.Description,
                            ActiveTransportCode = currentTransport?.Code,
                        };

                        data.Add(newRecord);
                    }



                }

                //var data = currentClusterDetails.Select(async detail => new GetClusterResponseDetail
                //{
                //    Id = detail.Id,
                //    SeqNumber = detail.SeqNumber ?? 0,
                //    ActiveTransportId = detail.ActiveTransportId,
                //    ActiveTransportDescription =await Context.ActiveTransport.AsNoTracking().FirstOrDefault(x => x.Id == detail.ActiveTransportId)?.Description,
                //}).OrderBy(x=> x.SeqNumber).ToList();

                return new GetClusterResponse
                {
                    Id = currentCluster.Id,
                    Code = currentCluster.Code,
                    Description = currentCluster.Description,
                    Active = currentCluster.Active,
                    DayNum = currentCluster.DayNum,
                    Direction = currentCluster.Direction,
                    DateCreated = currentCluster.DateCreated,
                    DateUpdated = currentCluster.DateUpdated,
                    data = data.OrderBy(c => c.SeqNumber).ToList()
                };

            }
            else {
                return null;
            }




        }
    }
}
