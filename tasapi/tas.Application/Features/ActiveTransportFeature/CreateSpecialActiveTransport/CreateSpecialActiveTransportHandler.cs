using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.ActiveTransportFeature.CreateSpecialActiveTransport
{ 
    public sealed class CreateActiveTransportHandler : IRequestHandler<CreateSpecialActiveTransportRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActiveTransportRepository _ActiveTransportRepository;
        private readonly IMapper _mapper;

        public CreateActiveTransportHandler(IUnitOfWork unitOfWork, IActiveTransportRepository ActiveTransportRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _ActiveTransportRepository = ActiveTransportRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateSpecialActiveTransportRequest request, CancellationToken cancellationToken)
        {
            await  _ActiveTransportRepository.CreateSpecial(request);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;

        }
    }
}
