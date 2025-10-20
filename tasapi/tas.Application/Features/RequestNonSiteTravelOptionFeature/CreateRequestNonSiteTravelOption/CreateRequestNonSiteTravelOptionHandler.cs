using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestNonSiteTravelOptionFeature.CreateRequestNonSiteTravelOption
{
    public class CreateRequestNonSiteTravelOptionHandler : IRequestHandler<CreateRequestNonSiteTravelOptionRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestNonSiteTravelOptionRepository _RequestNonSiteTravelOptionRepository;
        private readonly IMapper _mapper;

        public CreateRequestNonSiteTravelOptionHandler(IUnitOfWork unitOfWork, IRequestNonSiteTravelOptionRepository RequestNonSiteTravelOptionRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestNonSiteTravelOptionRepository = RequestNonSiteTravelOptionRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(CreateRequestNonSiteTravelOptionRequest request, CancellationToken cancellationToken)
        {
            await  _RequestNonSiteTravelOptionRepository.CreateOptionData(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
