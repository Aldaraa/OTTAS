using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.TransportFeature.EmployeeExistingTransport
{

    public sealed class EmployeeExistingTransportHandler : IRequestHandler<EmployeeExistingTransportRequest, List<EmployeeExistingTransportResponse>>
    {
        private readonly ITransportRepository _ITransportRepository;
        private readonly IMapper _mapper;

        public EmployeeExistingTransportHandler(ITransportRepository TransportRepository, IMapper mapper)
        {
            _ITransportRepository = TransportRepository;
            _mapper = mapper;
        }

        public async Task<List<EmployeeExistingTransportResponse>> Handle(EmployeeExistingTransportRequest request, CancellationToken cancellationToken)
        {
            return await _ITransportRepository.EmployeeExistingTransportSchedule(request, cancellationToken);
        }
    }
}
