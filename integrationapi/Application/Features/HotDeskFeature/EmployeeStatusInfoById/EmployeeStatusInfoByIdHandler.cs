using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.HotDeskFeature.EmployeeStatusInfoById
{

    public sealed class EmployeeStatusInfoByIdHandler : IRequestHandler<EmployeeStatusInfoByIdRequest, List<EmployeeStatusInfoByIdResponse>>
    {
        private readonly IHotDeskRepository _HotDeskInfoRepository;
        private readonly IMapper _mapper;

        public EmployeeStatusInfoByIdHandler(IHotDeskRepository HotDeskRepository, IMapper mapper)
        {
            _HotDeskInfoRepository = HotDeskRepository;
            _mapper = mapper;
        }

        public async Task<List<EmployeeStatusInfoByIdResponse>> Handle(EmployeeStatusInfoByIdRequest request, CancellationToken cancellationToken)
        {
            var data = await _HotDeskInfoRepository.EmployeeStatusInfoById(request, cancellationToken);
            return data;

        }
    }
}
