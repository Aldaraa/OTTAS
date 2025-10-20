using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.ShiftFeature.GetAllShift
{

    public sealed class GetAllShiftHandler : IRequestHandler<GetAllShiftRequest, List<GetAllShiftResponse>>
    {
        private readonly IShiftRepository _ShiftRepository;
        private readonly IMapper _mapper;

        public GetAllShiftHandler(IShiftRepository ShiftRepository, IMapper mapper)
        {
            _ShiftRepository = ShiftRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllShiftResponse>> Handle(GetAllShiftRequest request, CancellationToken cancellationToken)
        {
            var users = await _ShiftRepository.GetAllShift(request, cancellationToken);
            return _mapper.Map<List<GetAllShiftResponse>>(users);

        }
    }
}
