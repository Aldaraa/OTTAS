using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.NationalityFeature.GetAllNationality
{

    public sealed class GetAllNationalityHandler : IRequestHandler<GetAllNationalityRequest, List<GetAllNationalityResponse>>
    {
        private readonly INationalityRepository _NationalityRepository;
        private readonly IMapper _mapper;

        public GetAllNationalityHandler(INationalityRepository NationalityRepository, IMapper mapper)
        {
            _NationalityRepository = NationalityRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllNationalityResponse>> Handle(GetAllNationalityRequest request, CancellationToken cancellationToken)
        {

           return await _NationalityRepository.GetAllData(request, cancellationToken);

        }
    }
}
