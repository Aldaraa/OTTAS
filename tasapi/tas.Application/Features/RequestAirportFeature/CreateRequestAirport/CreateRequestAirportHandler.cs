using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestAirportFeature.CreateRequestAirport
{
    public sealed class CreateRequestAirportHandler : IRequestHandler<CreateRequestAirportRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestAirportRepository _RequestAirportRepository;
        private readonly IMapper _mapper;

        public CreateRequestAirportHandler(IUnitOfWork unitOfWork, IRequestAirportRepository RequestAirportRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestAirportRepository = RequestAirportRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateRequestAirportRequest request, CancellationToken cancellationToken)
        {
            var RequestAirport = _mapper.Map<RequestAirport>(request);
            await _RequestAirportRepository.CheckDuplicateData(RequestAirport, c => c.Code, c => c.Description);
            _RequestAirportRepository.Create(RequestAirport);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
