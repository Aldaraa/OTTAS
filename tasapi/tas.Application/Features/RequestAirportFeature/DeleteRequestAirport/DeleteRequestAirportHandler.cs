using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestAirportFeature.DeleteRequestAirport
{

    public sealed class DeleteRequestAirportHandler : IRequestHandler<DeleteRequestAirportRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestAirportRepository _RequestAirportRepository;
        private readonly IMapper _mapper;

        public DeleteRequestAirportHandler(IUnitOfWork unitOfWork, IRequestAirportRepository RequestAirportRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestAirportRepository = RequestAirportRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteRequestAirportRequest request, CancellationToken cancellationToken)
        {
            var RequestAirport = _mapper.Map<RequestAirport>(request);
            _RequestAirportRepository.Delete(RequestAirport);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
