using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportFeature.CreateNoGoShow
{
    public sealed class CreateNoGoShowHandler : IRequestHandler<CreateNoGoShowRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransportRepository _TransportRepository;


        public CreateNoGoShowHandler(IUnitOfWork unitOfWork, ITransportRepository TransportRepository)
        {
            _unitOfWork = unitOfWork;
            _TransportRepository = TransportRepository;
        }

        public async Task<Unit>  Handle(CreateNoGoShowRequest request, CancellationToken cancellationToken)
        {
           await _TransportRepository.CreateNoGoShow(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
