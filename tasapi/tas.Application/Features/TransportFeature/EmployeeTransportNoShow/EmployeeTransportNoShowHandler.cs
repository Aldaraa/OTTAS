using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.TransportFeature.EmployeeTransportNoShow
{

    public sealed class EmployeeTransportNoShowHandler : IRequestHandler<EmployeeTransportNoShowRequest, List<EmployeeTransportNoShowResponse>>
    {
        private readonly ITransportRepository _ITransportRepository;
        private readonly IMapper _mapper;

        public EmployeeTransportNoShowHandler(ITransportRepository TransportRepository, IMapper mapper)
        {
            _ITransportRepository = TransportRepository;
            _mapper = mapper;
        }

        public async Task<List<EmployeeTransportNoShowResponse>> Handle(EmployeeTransportNoShowRequest request, CancellationToken cancellationToken)
        {
            return await _ITransportRepository.EmployeeTransportNoShow(request, cancellationToken);
        }
    }
}
