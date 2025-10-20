using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.ClusterFeature.UpdateCluster
{
    public class UpdateClusterHandler : IRequestHandler<UpdateClusterRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClusterRepository _ClusterRepository;
        private readonly IMapper _mapper;

        public UpdateClusterHandler(IUnitOfWork unitOfWork, IClusterRepository ClusterRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _ClusterRepository = ClusterRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateClusterRequest request, CancellationToken cancellationToken)
        {
            var Cluster = _mapper.Map<Cluster>(request);
            _ClusterRepository.Update(Cluster);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
