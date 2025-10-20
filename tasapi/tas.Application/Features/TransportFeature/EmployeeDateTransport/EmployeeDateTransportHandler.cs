using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.TransportFeature.EmployeeDateTransport
{

    public sealed class EmployeeDateTransportHandler : IRequestHandler<EmployeeDateTransportRequest, EmployeeDateTransportResponse>
    {
        private readonly ITransportRepository _ITransportRepository;
        private readonly IMapper _mapper;

        public EmployeeDateTransportHandler(ITransportRepository TransportRepository, IMapper mapper)
        {
            _ITransportRepository = TransportRepository;
            _mapper = mapper;
        }

        public async Task<EmployeeDateTransportResponse> Handle(EmployeeDateTransportRequest request, CancellationToken cancellationToken)
        {
            return await _ITransportRepository.EmployeeDateTransportSchedule(request, cancellationToken);
        }
    }
}
