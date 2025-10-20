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

namespace tas.Application.Features.DepartmentFeature.CustomListDepartmment
{
        public sealed class CustomListDepartmentHandler : IRequestHandler<CustomListDepartmentRequest, List<CustomListDepartmentResponse>>
        {
        private readonly IDepartmentRepository _DepartmentRepository;

        public CustomListDepartmentHandler(IDepartmentRepository DepartmentRepository)
        {
            _DepartmentRepository = DepartmentRepository;
        }

        public async Task<List<CustomListDepartmentResponse>> Handle(CustomListDepartmentRequest request, CancellationToken cancellationToken)
        {
            return await _DepartmentRepository.GetMinimumList(cancellationToken);
        }
    }
}
