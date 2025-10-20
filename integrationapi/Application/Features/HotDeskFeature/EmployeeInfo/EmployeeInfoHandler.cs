using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.HotDeskFeature.EmployeeInfo
{

    public sealed class EmployeeInfoHandler : IRequestHandler<EmployeeInfoRequest, List<EmployeeInfoResponse>>
    {
        private readonly IHotDeskRepository _HotDeskInfoRepository;
        private readonly IMapper _mapper;

        public EmployeeInfoHandler(IHotDeskRepository HotDeskRepository, IMapper mapper)
        {
            _HotDeskInfoRepository = HotDeskRepository;
            _mapper = mapper;
        }

        public async Task<List<EmployeeInfoResponse>> Handle(EmployeeInfoRequest request, CancellationToken cancellationToken)
        {
            var data = await _HotDeskInfoRepository.EmployeeInfo();
            return data;

        }
    }
}
