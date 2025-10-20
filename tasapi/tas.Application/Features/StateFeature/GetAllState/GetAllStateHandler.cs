using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.StateFeature.GetAllState
{

    public sealed class GetAllStateHandler : IRequestHandler<GetAllStateRequest, List<GetAllStateResponse>>
    {
        private readonly IStateRepository _StateRepository;
        private readonly IMapper _mapper;

        public GetAllStateHandler(IStateRepository StateRepository, IMapper mapper)
        {
            _StateRepository = StateRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllStateResponse>> Handle(GetAllStateRequest request, CancellationToken cancellationToken)
        {
                return await _StateRepository.GetAllData(request, cancellationToken);
            
         
        }
    }
}
