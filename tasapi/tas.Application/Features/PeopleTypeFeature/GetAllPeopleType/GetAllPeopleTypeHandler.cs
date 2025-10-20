using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.PeopleTypeFeature.GetAllPeopleType
{

    public sealed class GetAllPeopleTypeHandler : IRequestHandler<GetAllPeopleTypeRequest, List<GetAllPeopleTypeResponse>>
    {
        private readonly IPeopleTypeRepository _PeopleTypeRepository;
        private readonly IMapper _mapper;

        public GetAllPeopleTypeHandler(IPeopleTypeRepository PeopleTypeRepository, IMapper mapper)
        {
            _PeopleTypeRepository = PeopleTypeRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllPeopleTypeResponse>> Handle(GetAllPeopleTypeRequest request, CancellationToken cancellationToken)
        {
            var data = await _PeopleTypeRepository.GetAllData(request, cancellationToken);
            return _mapper.Map<List<GetAllPeopleTypeResponse>>(data);

        }
    }
}
