using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.GroupMasterFeature.DeleteGroupMaster
{

    public sealed class DeleteGroupMasterHandler : IRequestHandler<DeleteGroupMasterRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGroupMasterRepository _GroupMasterRepository;
        private readonly IMapper _mapper;

        public DeleteGroupMasterHandler(IUnitOfWork unitOfWork, IGroupMasterRepository GroupMasterRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _GroupMasterRepository = GroupMasterRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteGroupMasterRequest request, CancellationToken cancellationToken)
        {
            var GroupMaster = _mapper.Map<GroupMaster>(request);
            _GroupMasterRepository.Delete(GroupMaster);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
