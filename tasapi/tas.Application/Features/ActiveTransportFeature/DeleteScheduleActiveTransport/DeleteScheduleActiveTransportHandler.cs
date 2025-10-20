using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.ActiveTransportFeature.DeleteActiveTransport
{

    public sealed class DeleteScheduleActiveTransportHandler : IRequestHandler<DeleteActiveTransportRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActiveTransportRepository _ActiveTransportRepository;
        private readonly IMapper _mapper;

        public DeleteScheduleActiveTransportHandler(IUnitOfWork unitOfWork, IActiveTransportRepository ActiveTransportRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _ActiveTransportRepository = ActiveTransportRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteActiveTransportRequest request, CancellationToken cancellationToken)
        {
           await _ActiveTransportRepository.DeleteTransportValidationDB(request.Id, cancellationToken);
            var ActiveTransport = _mapper.Map<ActiveTransport>(request);
           _ActiveTransportRepository.Delete(ActiveTransport);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
