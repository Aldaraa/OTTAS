using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.GroupMasterFeature.SetSeqGroupMaster
{
    public sealed class SetSeqGroupMasterHandler : IRequestHandler<SetSeqGroupMasterRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGroupMasterRepository _GroupMasterRepository;
        private readonly IMapper _mapper;

        public SetSeqGroupMasterHandler(IUnitOfWork unitOfWork, IGroupMasterRepository GroupMasterRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _GroupMasterRepository = GroupMasterRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(SetSeqGroupMasterRequest request, CancellationToken cancellationToken)
        {
               await    _GroupMasterRepository.ChangeOrderBy(request.GroupMasterIds, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
