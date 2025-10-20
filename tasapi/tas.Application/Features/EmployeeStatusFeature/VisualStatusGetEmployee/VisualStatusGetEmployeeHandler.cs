using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployerFeature.CreateEmployer;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.EmployeeStatusFeature.VisualStatusGetEmployee
{

    public sealed class VisualStatusGetEmployeeHandler : IRequestHandler<VisualStatusGetEmployeeRequest, List<VisualStatusGetEmployeeResponse>>
    {
        private readonly IEmployeeStatusRepository _EmployeeStatusRepository;
        private readonly IMapper _mapper;

        public VisualStatusGetEmployeeHandler(IEmployeeStatusRepository EmployeeStatusRepository, IMapper mapper)
        {
            _EmployeeStatusRepository = EmployeeStatusRepository;
            _mapper = mapper;
        }

        public async Task<List<VisualStatusGetEmployeeResponse>> Handle(VisualStatusGetEmployeeRequest request, CancellationToken cancellationToken)
        {

            var data = await _EmployeeStatusRepository.VisualStatusGetEmployee(request, cancellationToken);
            return data;


        }
    }
}
