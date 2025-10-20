using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.CarrierFeature.UpdateCarrier
{
    public class UpdateCarrierHandler : IRequestHandler<UpdateCarrierRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICarrierRepository _CarrierRepository;
        private readonly IMapper _mapper;

        public UpdateCarrierHandler(IUnitOfWork unitOfWork, ICarrierRepository CarrierRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _CarrierRepository = CarrierRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateCarrierRequest request, CancellationToken cancellationToken)
        {
            var Carrier = _mapper.Map<Carrier>(request);
            await _CarrierRepository.CheckDuplicateData(Carrier, c => c.Code, c => c.Description);
            _CarrierRepository.Update(Carrier);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
