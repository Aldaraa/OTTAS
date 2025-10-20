using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.CampFeature.GetAllCamp
{

    public sealed class GetAllCampHandler : IRequestHandler<GetAllCampRequest, List<GetAllCampResponse>>
    {
        private readonly ICampRepository _CampRepository;
        private readonly IMapper _mapper;

        public GetAllCampHandler(ICampRepository CampRepository, IMapper mapper)
        {
            _CampRepository = CampRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllCampResponse>> Handle(GetAllCampRequest request, CancellationToken cancellationToken)
        {

                return await _CampRepository.GetAllData(request,  cancellationToken);
           

        }
    }
}
