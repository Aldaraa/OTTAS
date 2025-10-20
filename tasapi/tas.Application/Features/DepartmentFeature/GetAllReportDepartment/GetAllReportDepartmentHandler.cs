using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.DepartmentFeature.GetAllReportDepartment
{

    public sealed class GetAllReportDepartmentHandler : IRequestHandler<GetAllReportDepartmentRequest, List<GetAllReportDepartmentResponse>>
    {
        private readonly IDepartmentRepository _DepartmentRepository;
        private readonly IMapper _mapper;

        public GetAllReportDepartmentHandler(IDepartmentRepository DepartmentRepository, IMapper mapper)
        {
            _DepartmentRepository = DepartmentRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllReportDepartmentResponse>> Handle(GetAllReportDepartmentRequest request, CancellationToken cancellationToken)
        {
            var Departments = await _DepartmentRepository.GetAllReportDepartmentsWithChildren(request, cancellationToken);

            return _mapper.Map<List<GetAllReportDepartmentResponse>>(Departments);
            

            //if (request.status.HasValue)
            //{
            //    var Departments = await _DepartmentRepository.GetAllActiveFilter((int)request.status, cancellationToken);
            //    return _mapper.Map<List<GetAllReportDepartmentResponse>>(Departments);
            //}
            //else
            //{
            //    var Departments = await _DepartmentRepository.GetAll(cancellationToken);
            //    return _mapper.Map<List<GetAllReportDepartmentResponse>>(Departments);
            //}

        }
    }
}
