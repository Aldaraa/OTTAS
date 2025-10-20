using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestGroupFeature.DeleteRequestGroup
{

    public sealed class DeleteRequestGroupHandler : IRequestHandler<DeleteRequestGroupRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestGroupRepository _RequestGroupRepository;
        private readonly IMapper _mapper;

        public DeleteRequestGroupHandler(IUnitOfWork unitOfWork, IRequestGroupRepository RequestGroupRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestGroupRepository = RequestGroupRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteRequestGroupRequest request, CancellationToken cancellationToken)
        {
            var RequestGroup = _mapper.Map<RequestGroup>(request);
            _RequestGroupRepository.Delete(RequestGroup);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
