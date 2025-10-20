using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.PositionFeature.GetAllPosition
{

    public sealed class GetAllPositionHandler : IRequestHandler<GetAllPositionRequest, GetAllPositionResponse>
    {
        private readonly IPositionRepository _PositionRepository;
        private readonly IMapper _mapper;

        public GetAllPositionHandler(IPositionRepository PositionRepository, IMapper mapper)
        {
            _PositionRepository = PositionRepository;
            _mapper = mapper;
        }

        public async Task<GetAllPositionResponse> Handle(GetAllPositionRequest request, CancellationToken cancellationToken)
        {
            var Positions = await _PositionRepository.GetAllData(request, cancellationToken);
            return _mapper.Map<GetAllPositionResponse>(Positions);
            //if (request.status.HasValue)
            //{

            //}
            //else {
            //    var users = await _PositionRepository.GetAll(cancellationToken);
            //    return _mapper.Map<List<GetAllPositionResponse>>(users);
            //}

        }
    }
}
