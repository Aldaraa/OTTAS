using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.ActiveTransportFeature.UpdateActiveTransport
{
    public class UpdateActiveTransportHandler : IRequestHandler<UpdateActiveTransportRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActiveTransportRepository _ActiveTransportRepository;
        private readonly IMapper _mapper;

        public UpdateActiveTransportHandler(IUnitOfWork unitOfWork, IActiveTransportRepository ActiveTransportRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _ActiveTransportRepository = ActiveTransportRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateActiveTransportRequest request, CancellationToken cancellationToken)
        {
            await _ActiveTransportRepository.ChangeTransport(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
