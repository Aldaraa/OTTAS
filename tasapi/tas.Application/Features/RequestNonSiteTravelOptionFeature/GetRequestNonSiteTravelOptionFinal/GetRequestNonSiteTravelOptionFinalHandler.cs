using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestNonSiteTravelOptionFeature.GetRequestNonSiteTravelOptionFinal;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestNonSiteTravelOptionFeature.GetRequestNonSiteTravelOptionFinal
{

    public sealed class GetRequestNonSiteTravelOptionFinalHandler : IRequestHandler<GetRequestNonSiteTravelOptionFinalRequest, List<GetRequestNonSiteTravelOptionFinalResponse>>
    {
        private readonly IRequestNonSiteTravelOptionRepository _RequestNonSiteTravelOptionRepository;
        private readonly IMapper _mapper;

        public GetRequestNonSiteTravelOptionFinalHandler(IRequestNonSiteTravelOptionRepository RequestNonSiteTravelOptionRepository, IMapper mapper)
        {
            _RequestNonSiteTravelOptionRepository = RequestNonSiteTravelOptionRepository;
            _mapper = mapper;
        }

        public async Task<List<GetRequestNonSiteTravelOptionFinalResponse>> Handle(GetRequestNonSiteTravelOptionFinalRequest request, CancellationToken cancellationToken)
        {

           return await _RequestNonSiteTravelOptionRepository.GetFinalOptionData(request, cancellationToken);

        }
    }
}
