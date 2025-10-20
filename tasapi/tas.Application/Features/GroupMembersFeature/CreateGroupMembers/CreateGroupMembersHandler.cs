using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.GroupMembersFeature.CreateGroupMembers
{
    public sealed class CreateGroupMembersHandler : IRequestHandler<CreateGroupMembersRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGroupMembersRepository _GroupMembersRepository;
        private readonly IMapper _mapper;

        public CreateGroupMembersHandler(IUnitOfWork unitOfWork, IGroupMembersRepository GroupMembersRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _GroupMembersRepository = GroupMembersRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateGroupMembersRequest request, CancellationToken cancellationToken)
        {
            await _GroupMembersRepository.SaveData(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
