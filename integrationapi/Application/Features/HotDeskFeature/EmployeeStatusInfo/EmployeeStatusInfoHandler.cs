using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.HotDeskFeature.EmployeeStatusInfo
{

    public sealed class EmployeeStatusInfoHandler : IRequestHandler<EmployeeStatusInfoRequest, List<EmployeeStatusInfoResponse>>
    {
        private readonly IHotDeskRepository _HotDeskInfoRepository;
        private readonly IMapper _mapper;

        public EmployeeStatusInfoHandler(IHotDeskRepository HotDeskRepository, IMapper mapper)
        {
            _HotDeskInfoRepository = HotDeskRepository;
            _mapper = mapper;
        }

        public async Task<List<EmployeeStatusInfoResponse>> Handle(EmployeeStatusInfoRequest request, CancellationToken cancellationToken)
        {
            var data = await _HotDeskInfoRepository.EmployeeStatusInfo();
            return data;

        }
    }
}
