using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tas.Application.Features.BedFeature.GetAllBed;
using tas.Application.Features.BedFeature.GetBed;
using tas.Application.Features.SysResponseTimeFeature.DeleteSysResponseTime;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface ISysResponseTimeRepository : IBaseRepository<SysResponseTime>
    {
        Task  DeleteOldData(DeleteSysResponseTimeRequest request, CancellationToken cancellationToken);
    }
}
