using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestNonSiteTravelOptionFeature.GetRequestNonSiteTravelOption
{

    public sealed class GetRequestNonSiteTravelOptionHandler : IRequestHandler<GetRequestNonSiteTravelOptionRequest, List<GetRequestNonSiteTravelOptionResponse>>
    {
        private readonly IRequestNonSiteTravelOptionRepository _RequestNonSiteTravelOptionRepository;
        private readonly IMapper _mapper;

        public GetRequestNonSiteTravelOptionHandler(IRequestNonSiteTravelOptionRepository RequestNonSiteTravelOptionRepository, IMapper mapper)
        {
            _RequestNonSiteTravelOptionRepository = RequestNonSiteTravelOptionRepository;
            _mapper = mapper;
        }

        public async Task<List<GetRequestNonSiteTravelOptionResponse>> Handle(GetRequestNonSiteTravelOptionRequest request, CancellationToken cancellationToken)
        {

           return await _RequestNonSiteTravelOptionRepository.GetData(request, cancellationToken);

        }
    }
}
