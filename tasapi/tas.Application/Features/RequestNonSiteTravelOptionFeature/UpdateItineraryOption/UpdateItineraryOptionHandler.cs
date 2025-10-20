using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestNonSiteTravelOptionFeature.UpdateItineraryOption
{
    public class UpdateItineraryOptionHandler : IRequestHandler<UpdateItineraryOptionRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestNonSiteTravelOptionRepository _RequestNonSiteTravelOptionRepository;
        private readonly IMapper _mapper;

        public UpdateItineraryOptionHandler(IUnitOfWork unitOfWork, IRequestNonSiteTravelOptionRepository RequestNonSiteTravelOptionRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestNonSiteTravelOptionRepository = RequestNonSiteTravelOptionRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateItineraryOptionRequest request, CancellationToken cancellationToken)
        {
            await  _RequestNonSiteTravelOptionRepository.UpdateItinerary(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
