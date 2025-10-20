using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.TransportFeature.GetEmployeeTransportAll
{

    public sealed class GetEmployeeTransportAllHandler : IRequestHandler<GetEmployeeTransportAllRequest, List<GetEmployeeTransportAllResponse>>
    {
        private readonly ITransportRepository _ITransportRepository;
        private readonly IMapper _mapper;

        public GetEmployeeTransportAllHandler(ITransportRepository TransportRepository, IMapper mapper)
        {
            _ITransportRepository = TransportRepository;
            _mapper = mapper;
        }

        public async Task<List<GetEmployeeTransportAllResponse>> Handle(GetEmployeeTransportAllRequest request, CancellationToken cancellationToken)
        {

            return await _ITransportRepository.EmployeeTransportAllSchedule(request, cancellationToken);




        }
    }
}
