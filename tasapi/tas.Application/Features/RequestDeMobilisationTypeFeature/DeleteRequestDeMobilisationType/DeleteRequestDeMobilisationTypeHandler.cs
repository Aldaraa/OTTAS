using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDeMobilisationTypeFeature.DeleteRequestDeMobilisationType
{

    public sealed class DeleteRequestDeMobilisationTypeHandler : IRequestHandler<DeleteRequestDeMobilisationTypeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDeMobilisationTypeRepository _RequestDeMobilisationTypeRepository;
        private readonly IMapper _mapper;

        public DeleteRequestDeMobilisationTypeHandler(IUnitOfWork unitOfWork, IRequestDeMobilisationTypeRepository RequestDeMobilisationTypeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestDeMobilisationTypeRepository = RequestDeMobilisationTypeRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteRequestDeMobilisationTypeRequest request, CancellationToken cancellationToken)
        {
            var RequestDeMobilisationType = _mapper.Map<RequestDeMobilisationType>(request);
            _RequestDeMobilisationTypeRepository.Delete(RequestDeMobilisationType);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
