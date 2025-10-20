using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestTravelPurposeFeature.UpdateRequestTravelPurpose
{
    public class UpdateRequestTravelPurposeHandler : IRequestHandler<UpdateRequestTravelPurposeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestTravelPurposeRepository _RequestTravelPurposeRepository;
        private readonly IMapper _mapper;

        public UpdateRequestTravelPurposeHandler(IUnitOfWork unitOfWork, IRequestTravelPurposeRepository RequestTravelPurposeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestTravelPurposeRepository = RequestTravelPurposeRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateRequestTravelPurposeRequest request, CancellationToken cancellationToken)
        {
            var RequestTravelPurpose = _mapper.Map<RequestTravelPurpose>(request);
            await _RequestTravelPurposeRepository.CheckDuplicateData(RequestTravelPurpose, c => c.Code, c => c.Description);
            _RequestTravelPurposeRepository.Update(RequestTravelPurpose);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
