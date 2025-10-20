using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.DepartmentFeature.GetManagersDepartment
{

    public sealed class GetManagersDepartmentHandler : IRequestHandler<GetManagersDepartmentRequest, List<GetManagersDepartmentResponse>>
    {
        private readonly IDepartmentRepository _DepartmentRepository;
        private readonly IMapper _mapper;

        public GetManagersDepartmentHandler(IDepartmentRepository DepartmentRepository, IMapper mapper)
        {
            _DepartmentRepository = DepartmentRepository;
            _mapper = mapper;
        }

        public async Task<List<GetManagersDepartmentResponse>> Handle(GetManagersDepartmentRequest request, CancellationToken cancellationToken)
        {
            return await _DepartmentRepository.GetManagersDepartment(request, cancellationToken);
        }
    }
}
