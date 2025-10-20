using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.TransportFeature.EmployeeTransportGoShow
{

    public sealed class EmployeeTransportGoShowHandler : IRequestHandler<EmployeeTransportGoShowRequest, List<EmployeeTransportGoShowResponse>>
    {
        private readonly ITransportRepository _ITransportRepository;
        private readonly IMapper _mapper;

        public EmployeeTransportGoShowHandler(ITransportRepository TransportRepository, IMapper mapper)
        {
            _ITransportRepository = TransportRepository;
            _mapper = mapper;
        }

        public async Task<List<EmployeeTransportGoShowResponse>> Handle(EmployeeTransportGoShowRequest request, CancellationToken cancellationToken)
        {
            return await _ITransportRepository.EmployeeTransportGoShow(request, cancellationToken);
        }
    }
}
