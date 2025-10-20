using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.HotDeskFeature.DepartmentInfo
{

    public sealed class DepartmentInfoHandler : IRequestHandler<DepartmentInfoRequest, List<DepartmentInfoResponse>>
    {
        private readonly IHotDeskRepository _HotDeskInfoRepository;
        private readonly IMapper _mapper;

        public DepartmentInfoHandler(IHotDeskRepository HotDeskRepository, IMapper mapper)
        {
            _HotDeskInfoRepository = HotDeskRepository;
            _mapper = mapper;
        }

        public async Task<List<DepartmentInfoResponse>> Handle(DepartmentInfoRequest request, CancellationToken cancellationToken)
        {
            var data = await _HotDeskInfoRepository.DepartmentInfo();
            return data;

        }
    }
}
