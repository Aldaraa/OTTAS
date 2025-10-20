using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDeMobilisationTypeFeature.CreateRequestDeMobilisationType
{
    public sealed class CreateRequestDeMobilisationTypeHandler : IRequestHandler<CreateRequestDeMobilisationTypeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDeMobilisationTypeRepository _RequestDeMobilisationTypeRepository;
        private readonly IMapper _mapper;

        public CreateRequestDeMobilisationTypeHandler(IUnitOfWork unitOfWork, IRequestDeMobilisationTypeRepository RequestDeMobilisationTypeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestDeMobilisationTypeRepository = RequestDeMobilisationTypeRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateRequestDeMobilisationTypeRequest request, CancellationToken cancellationToken)
        {
            var RequestDeMobilisationType = _mapper.Map<RequestDeMobilisationType>(request);
            _RequestDeMobilisationTypeRepository.Create(RequestDeMobilisationType);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
