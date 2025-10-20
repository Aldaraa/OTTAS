using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.TransportFeature.GetEmployeeTransport
{

    public sealed class GetEmployeeTransportHandler : IRequestHandler<GetEmployeeTransportRequest, List<GetEmployeeTransportResponse>>
    {
        private readonly ITransportRepository _ITransportRepository;
        private readonly IMapper _mapper;

        public GetEmployeeTransportHandler(ITransportRepository TransportRepository, IMapper mapper)
        {
            _ITransportRepository = TransportRepository;
            _mapper = mapper;
        }

        public async Task<List<GetEmployeeTransportResponse>> Handle(GetEmployeeTransportRequest request, CancellationToken cancellationToken)
        {

            return await _ITransportRepository.EmployeeTransportSchedule(request, cancellationToken);




        }
    }
}
