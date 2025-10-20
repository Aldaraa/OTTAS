using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.DepartmentFeature.GetAllDepartment;
using tas.Application.Features.EmployerFeature.GetAllEmployer;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.GetDepartment
{
        public sealed class GetDepartmentHandler : IRequestHandler<GetDepartmentRequest, GetDepartmentResponse>
        {
        private readonly IDepartmentRepository _DepartmentRepository;

        public GetDepartmentHandler(IDepartmentRepository DepartmentRepository)
        {
            _DepartmentRepository = DepartmentRepository;
        }

        public async Task<GetDepartmentResponse> Handle(GetDepartmentRequest request, CancellationToken cancellationToken)
        {
            return await _DepartmentRepository.GetbyId(request, cancellationToken);
        }
    }
}
