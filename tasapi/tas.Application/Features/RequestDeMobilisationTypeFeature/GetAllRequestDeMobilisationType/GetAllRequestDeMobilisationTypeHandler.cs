using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDeMobilisationTypeFeature.GetAllRequestDeMobilisationType
{

    public sealed class GetAllRequestDeMobilisationTypeHandler : IRequestHandler<GetAllRequestDeMobilisationTypeRequest, List<GetAllRequestDeMobilisationTypeResponse>>
    {
        private readonly IRequestDeMobilisationTypeRepository _RequestDeMobilisationTypeRepository;
        private readonly IMapper _mapper;

        public GetAllRequestDeMobilisationTypeHandler(IRequestDeMobilisationTypeRepository RequestDeMobilisationTypeRepository, IMapper mapper)
        {
            _RequestDeMobilisationTypeRepository = RequestDeMobilisationTypeRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllRequestDeMobilisationTypeResponse>> Handle(GetAllRequestDeMobilisationTypeRequest request, CancellationToken cancellationToken)
        {

           return await _RequestDeMobilisationTypeRepository.GetAllData(request, cancellationToken);

        }
    }
}
