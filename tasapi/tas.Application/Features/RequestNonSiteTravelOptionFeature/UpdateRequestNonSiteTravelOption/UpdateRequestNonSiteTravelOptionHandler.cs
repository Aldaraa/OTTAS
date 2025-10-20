using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestNonSiteTravelOptionFeature.UpdateRequestNonSiteTravelOption
{
    public class UpdateRequestNonSiteTravelOptionHandler : IRequestHandler<UpdateRequestNonSiteTravelOptionRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestNonSiteTravelOptionRepository _RequestNonSiteTravelOptionRepository;
        private readonly IMapper _mapper;

        public UpdateRequestNonSiteTravelOptionHandler(IUnitOfWork unitOfWork, IRequestNonSiteTravelOptionRepository RequestNonSiteTravelOptionRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestNonSiteTravelOptionRepository = RequestNonSiteTravelOptionRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateRequestNonSiteTravelOptionRequest request, CancellationToken cancellationToken)
        {
            await  _RequestNonSiteTravelOptionRepository.UpdateOptionData(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
