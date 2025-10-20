using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.DepartmentFeature.GetAllDepartment
{

    public sealed class GetAllDepartmentHandler : IRequestHandler<GetAllDepartmentRequest, List<GetAllDepartmentResponse>>
    {
        private readonly IDepartmentRepository _DepartmentRepository;
        private readonly IMapper _mapper;

        public GetAllDepartmentHandler(IDepartmentRepository DepartmentRepository, IMapper mapper)
        {
            _DepartmentRepository = DepartmentRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllDepartmentResponse>> Handle(GetAllDepartmentRequest request, CancellationToken cancellationToken)
        {
            var Departments = await _DepartmentRepository.GetAllDepartmentsWithChildren(request, cancellationToken);

            return _mapper.Map<List<GetAllDepartmentResponse>>(Departments);
            

            //if (request.status.HasValue)
            //{
            //    var Departments = await _DepartmentRepository.GetAllActiveFilter((int)request.status, cancellationToken);
            //    return _mapper.Map<List<GetAllDepartmentResponse>>(Departments);
            //}
            //else
            //{
            //    var Departments = await _DepartmentRepository.GetAll(cancellationToken);
            //    return _mapper.Map<List<GetAllDepartmentResponse>>(Departments);
            //}

        }
    }
}
