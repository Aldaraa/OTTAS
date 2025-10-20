using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.PositionFeature.AllPosition
{

    public sealed class AllPositionHandler : IRequestHandler<AllPositionRequest, List<AllPositionResponse>>
    {
        private readonly IPositionRepository _PositionRepository;
        private readonly IMapper _mapper;

        public AllPositionHandler(IPositionRepository PositionRepository, IMapper mapper)
        {
            _PositionRepository = PositionRepository;
            _mapper = mapper;
        }

        public async Task<List<AllPositionResponse>> Handle(AllPositionRequest request, CancellationToken cancellationToken)
        {
              return await _PositionRepository.AllData(request, cancellationToken);

            //if (request.status.HasValue)
            //{

            //}
            //else {
            //    var users = await _PositionRepository.All(cancellationToken);
            //    return _mapper.Map<List<AllPositionResponse>>(users);
            //}

        }
    }
}
