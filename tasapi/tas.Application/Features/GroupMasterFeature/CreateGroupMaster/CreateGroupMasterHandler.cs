using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.GroupMasterFeature.CreateGroupMaster
{
    public sealed class CreateGroupMasterHandler : IRequestHandler<CreateGroupMasterRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGroupMasterRepository _GroupMasterRepository;
        private readonly IMapper _mapper;

        public CreateGroupMasterHandler(IUnitOfWork unitOfWork, IGroupMasterRepository GroupMasterRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _GroupMasterRepository = GroupMasterRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateGroupMasterRequest request, CancellationToken cancellationToken)
        {
            var GroupMaster = _mapper.Map<GroupMaster>(request);
            await _GroupMasterRepository.CheckDuplicateData(GroupMaster, c => c.Description);
            _GroupMasterRepository.Create(GroupMaster);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
