using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.GetAllEmployee;
using tas.Application.Repositories;

namespace tas.Application.Features.EmployeeFeature.GetProfileTransport
{

    public sealed class GetProfileTransportHandler : IRequestHandler<GetProfileTransportRequest, List<GetProfileTransportResponse>>
    {
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public GetProfileTransportHandler(IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<List<GetProfileTransportResponse>> Handle(GetProfileTransportRequest request, CancellationToken cancellationToken)
        {
            var data =  await _EmployeeRepository.GetProfileTransport(request, cancellationToken);
            return data;

        }
    }
}
