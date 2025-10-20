using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportFeature.DeleteNoGoShow
{
    public sealed class DeleteNoGoShowHandler : IRequestHandler<DeleteNoGoShowRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransportRepository _TransportRepository;


        public DeleteNoGoShowHandler(IUnitOfWork unitOfWork, ITransportRepository TransportRepository)
        {
            _unitOfWork = unitOfWork;
            _TransportRepository = TransportRepository;
        }

        public async Task<Unit>  Handle(DeleteNoGoShowRequest request, CancellationToken cancellationToken)
        {
           await _TransportRepository.DeleteNoGoShow(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
