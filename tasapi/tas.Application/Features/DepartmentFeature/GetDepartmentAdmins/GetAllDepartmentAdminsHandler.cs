using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.DepartmentFeature.GetAllDepartmentAdmins
{

    public sealed class GetAllDepartmentAdminsHandler : IRequestHandler<GetAllDepartmentAdminsRequest, List<GetAllDepartmentAdminsResponse>>
    {
        private readonly IDepartmentRepository _DepartmentRepository;
        private readonly IMapper _mapper;

        public GetAllDepartmentAdminsHandler(IDepartmentRepository DepartmentRepository, IMapper mapper)
        {
            _DepartmentRepository = DepartmentRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllDepartmentAdminsResponse>> Handle(GetAllDepartmentAdminsRequest request, CancellationToken cancellationToken)
        {
            return await _DepartmentRepository.GetAllDepartmentAdmins(request, cancellationToken);
        }
    }
}
