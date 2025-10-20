using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestNonSiteTravelOptionDataFeature.UpdateRequestNonSiteTravelOptionData
{
    public class UpdateRequestNonSiteTravelOptionDataHandler : IRequestHandler<UpdateRequestNonSiteTravelOptionDataRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestNonSiteTravelOptionRepository _RequestNonSiteTravelOptionDataRepository;
        private readonly IMapper _mapper;

        public UpdateRequestNonSiteTravelOptionDataHandler(IUnitOfWork unitOfWork, IRequestNonSiteTravelOptionRepository RequestNonSiteTravelOptionDataRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestNonSiteTravelOptionDataRepository = RequestNonSiteTravelOptionDataRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateRequestNonSiteTravelOptionDataRequest request, CancellationToken cancellationToken)
        {
          await  _RequestNonSiteTravelOptionDataRepository.UpdateOptionFullData(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
