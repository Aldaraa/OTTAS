using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Extensions;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeStatusFeature.VisualStatusDateChangeBulk
{
    public class VisualStatusDateChangeBulkHandler : IRequestHandler<VisualStatusDateChangeBulkRequest, List<VisualStatusDateChangeBulkResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeStatusRepository _EmployeeStatusRepository;
        private readonly IMapper _mapper;

        public VisualStatusDateChangeBulkHandler(IUnitOfWork unitOfWork, IEmployeeStatusRepository EmployeeStatusRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployeeStatusRepository = EmployeeStatusRepository;
            _mapper = mapper;
        }

        public async Task<List<VisualStatusDateChangeBulkResponse>> Handle(VisualStatusDateChangeBulkRequest request, CancellationToken cancellationToken)
        {
             var data = await  _EmployeeStatusRepository.VisualStatusDateChangeBulk(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return data;
        }





      
    }
}
