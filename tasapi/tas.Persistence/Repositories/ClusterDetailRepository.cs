using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ClusterDetailFeature.CreateClusterDetail;
using tas.Application.Features.ClusterDetailFeature.DeleteClusterDetail;
using tas.Application.Features.ClusterDetailFeature.ReOrderClusterDetail;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class ClusterDetailRepository : BaseRepository<ClusterDetail>, IClusterDetailRepository
    {
        public ClusterDetailRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
        }

        public async Task ReOrder(ReOrderClusterDetailRequest request, CancellationToken cancellationToken)
        { 
            var clusterDetails =await Context.ClusterDetail.Where(x=> x.ClusterId == request.ClusterId).ToListAsync();

            var Rownum = 1;
            foreach (var clusterDetailId in request.ClusterDetailIds)
            {
                var clusterDetail = clusterDetails.FirstOrDefault(x => x.Id == clusterDetailId);
                if (clusterDetail != null) {
                    clusterDetail.SeqNumber = Rownum;
                    Context.ClusterDetail.Update(clusterDetail);
                }
                Rownum++;
            }
        }


        //public async Task ReOrder(ReOrderClusterDetailRequest request, CancellationToken cancellationToken)
        //{


        //    var currentRecord = Context.ClusterDetail.FirstOrDefault(x => x.Id == request.Id);

        //    if (currentRecord == null)
        //    {
        //        throw new ArgumentException($"Record with ID {request.Id} not found.");
        //    }

        //    if (request.SeqNumber <= 0 || request.SeqNumber > Context.ClusterDetail.Max(x => x.SeqNumber))
        //    {
        //        throw new ArgumentException($"Invalid sequence number {request.SeqNumber}.");
        //    }

        //    if (request.SeqNumber == currentRecord.SeqNumber)
        //    {
        //        return;
        //    }

        //    if (request.SeqNumber > currentRecord.SeqNumber)
        //    {
        //        var recordsToShift = Context.ClusterDetail.Where(x => x.ClusterId == currentRecord.ClusterId && x.SeqNumber > currentRecord.SeqNumber && x.SeqNumber <= request.SeqNumber).OrderBy(x => x.SeqNumber).ToList();

        //        foreach (var record in recordsToShift)
        //        {
        //            record.SeqNumber--;
        //            Context.ClusterDetail.Update(record);
        //        }

        //        currentRecord.SeqNumber = request.SeqNumber;
        //        Context.ClusterDetail.Update(currentRecord);
        //    }
        //    else // request.SeqNumber < currentRecord.SeqNumber
        //    {
        //        var recordsToShift = Context.ClusterDetail.Where(x => x.ClusterId == currentRecord.ClusterId && x.SeqNumber >= request.SeqNumber && x.SeqNumber < currentRecord.SeqNumber).OrderByDescending(x => x.SeqNumber).ToList();

        //        foreach (var record in recordsToShift)
        //        {
        //            record.SeqNumber++;
        //            Context.ClusterDetail.Update(record);
        //        }

        //        currentRecord.SeqNumber = request.SeqNumber;
        //        Context.ClusterDetail.Update(currentRecord);
        //    }


        //}

        public async Task Create(CreateClusterDetailRequest request, CancellationToken cancellationToken)
        {
            var currentCluster =await Context.Cluster.FirstOrDefaultAsync(x => x.Id == request.ClusterId);
            if (currentCluster != null)
            {
                // Check if the record already exists
                var oldRecord =await Context.ClusterDetail.FirstOrDefaultAsync(x => x.ClusterId == request.ClusterId && x.ActiveTransportId == request.ActiveTransportId);
                if (oldRecord == null)
                {
                    // Check if the active transport exists and matches the cluster criteria
                    var data =await Context.ActiveTransport.FirstOrDefaultAsync(x => x.Active == 1 && x.DayNum == currentCluster.DayNum && x.Direction == currentCluster.Direction && x.Id == request.ActiveTransportId);
                    if (data != null)
                    {
                        var newRecord = new ClusterDetail
                        {
                            ActiveTransportId = request.ActiveTransportId,
                            ClusterId = request.ClusterId,
                            SeqNumber = request.SeqNumber,
                            UserIdCreated = 1
                        };

                        // Reorder previous records if necessary
                        var overlappingRecords =await Context.ClusterDetail.Where(x => x.ClusterId == request.ClusterId && x.SeqNumber >= request.SeqNumber).OrderBy(x => x.SeqNumber).ToListAsync();
                        foreach (var record in overlappingRecords)
                        {
                            record.SeqNumber++;
                            Context.ClusterDetail.Update(record);
                        }

                        Context.ClusterDetail.Add(newRecord);
                    }
                }
                else
                {
                    // Update the existing record if necessary
                    if (oldRecord.SeqNumber != request.SeqNumber)
                    {
                        // Reorder previous records if necessary
                        var overlappingRecords =await Context.ClusterDetail.Where(x => x.ClusterId == request.ClusterId && x.SeqNumber >= request.SeqNumber && x.Id != oldRecord.Id).OrderBy(x => x.SeqNumber).ToListAsync();
                        foreach (var record in overlappingRecords)
                        {
                            record.SeqNumber++;
                            Context.ClusterDetail.Update(record);
                        }

                        oldRecord.SeqNumber = request.SeqNumber;
                        Context.ClusterDetail.Update(oldRecord);
                    }
                }
            }
        }



        public async Task Delete(DeleteClusterDetailRequest request, CancellationToken cancellationToken)
        {
            var item = await Context.ClusterDetail.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (item != null) { 
                Context.ClusterDetail.Remove(item);
            }
        }


    }
}
