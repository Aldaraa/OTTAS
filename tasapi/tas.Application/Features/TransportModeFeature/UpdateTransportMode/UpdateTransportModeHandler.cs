using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportModeFeature.UpdateTransportMode
{
    public class UpdateTransportModeHandler : IRequestHandler<UpdateTransportModeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransportModeRepository _TransportModeRepository;
        private readonly IMapper _mapper;

        public UpdateTransportModeHandler(IUnitOfWork unitOfWork, ITransportModeRepository TransportModeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _TransportModeRepository = TransportModeRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateTransportModeRequest request, CancellationToken cancellationToken)
        {
            var TransportMode = _mapper.Map<TransportMode>(request);
            await _TransportModeRepository.CheckDuplicateData(TransportMode, c => c.Code);
            _TransportModeRepository.Update(TransportMode);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
