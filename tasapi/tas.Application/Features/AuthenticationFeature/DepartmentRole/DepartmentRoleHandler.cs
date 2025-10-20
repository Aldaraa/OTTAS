using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.BedFeature.CreateBed;
using tas.Application.Repositories;

namespace tas.Application.Features.AuthenticationFeature.DepartmentRole
{


    public sealed class DepartmentRoleHandler : IRequestHandler<DepartmentRoleRequest, DepartmentRoleResponse>
    {
        private readonly IAuthenticationRepository _AuthenticationRepository;

        public DepartmentRoleHandler(IAuthenticationRepository AuthenticationRepository)
        {
            _AuthenticationRepository = AuthenticationRepository;
        }

        public async Task<DepartmentRoleResponse> Handle(DepartmentRoleRequest request, CancellationToken cancellationToken)
        {
            return  await _AuthenticationRepository.GetDepartmentRoleData(request);
        }
    }
}
