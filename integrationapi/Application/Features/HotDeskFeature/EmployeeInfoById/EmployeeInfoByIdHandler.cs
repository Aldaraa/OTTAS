using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.HotDeskFeature.EmployeeInfoById
{

    public sealed class EmployeeInfoByIdHandler : IRequestHandler<EmployeeInfoByIdRequest, EmployeeInfoByIdResponse>
    {
        private readonly IHotDeskRepository _HotDeskInfoRepository;
        private readonly IMapper _mapper;

        public EmployeeInfoByIdHandler(IHotDeskRepository HotDeskRepository, IMapper mapper)
        {
            _HotDeskInfoRepository = HotDeskRepository;
            _mapper = mapper;
        }

        public async Task<EmployeeInfoByIdResponse> Handle(EmployeeInfoByIdRequest request, CancellationToken cancellationToken)
        {
            var data = await _HotDeskInfoRepository.EmployeeInfoById(request, cancellationToken);
            return data;

        }
    }
}
