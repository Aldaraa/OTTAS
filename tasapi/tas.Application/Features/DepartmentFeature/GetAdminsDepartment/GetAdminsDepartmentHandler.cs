using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.DepartmentFeature.GetAdminsDepartment
{

    public sealed class GetAdminsDepartmentHandler : IRequestHandler<GetAdminsDepartmentRequest, List<GetAdminsDepartmentResponse>>
    {
        private readonly IDepartmentRepository _DepartmentRepository;
        private readonly IMapper _mapper;

        public GetAdminsDepartmentHandler(IDepartmentRepository DepartmentRepository, IMapper mapper)
        {
            _DepartmentRepository = DepartmentRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAdminsDepartmentResponse>> Handle(GetAdminsDepartmentRequest request, CancellationToken cancellationToken)
        {
            return await _DepartmentRepository.GetAdminsDepartment(request, cancellationToken);
        }
    }
}
