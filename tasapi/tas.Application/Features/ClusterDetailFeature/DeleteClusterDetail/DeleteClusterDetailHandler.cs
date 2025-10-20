using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.ClusterDetailFeature.DeleteClusterDetail
{

    public sealed class DeleteClusterDetailHandler : IRequestHandler<DeleteClusterDetailRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClusterDetailRepository _ClusterDetailRepository;
        private readonly IMapper _mapper;

        public DeleteClusterDetailHandler(IUnitOfWork unitOfWork, IClusterDetailRepository ClusterDetailRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _ClusterDetailRepository = ClusterDetailRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteClusterDetailRequest request, CancellationToken cancellationToken)
        {
            await  _ClusterDetailRepository.Delete(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
