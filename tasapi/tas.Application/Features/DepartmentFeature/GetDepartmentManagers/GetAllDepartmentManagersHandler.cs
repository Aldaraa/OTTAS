using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.DepartmentFeature.GetAllDepartmentManagers
{

    public sealed class GetAllDepartmentManagersHandler : IRequestHandler<GetAllDepartmentManagersRequest, List<GetAllDepartmentManagersResponse>>
    {
        private readonly IDepartmentRepository _DepartmentRepository;
        private readonly IMapper _mapper;

        public GetAllDepartmentManagersHandler(IDepartmentRepository DepartmentRepository, IMapper mapper)
        {
            _DepartmentRepository = DepartmentRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllDepartmentManagersResponse>> Handle(GetAllDepartmentManagersRequest request, CancellationToken cancellationToken)
        {
            return await _DepartmentRepository.GetAllDepartmentManagers(request, cancellationToken);



        }
    }
}
