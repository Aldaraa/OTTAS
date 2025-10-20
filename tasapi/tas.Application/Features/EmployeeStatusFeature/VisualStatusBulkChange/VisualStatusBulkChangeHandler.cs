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

namespace tas.Application.Features.EmployeeStatusFeature.VisualStatusBulkChange
{
    public class VisualStatusBulkChangeHandler : IRequestHandler<VisualStatusBulkChangeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeStatusRepository _EmployeeStatusRepository;
        private readonly IMapper _mapper;

        public VisualStatusBulkChangeHandler(IUnitOfWork unitOfWork, IEmployeeStatusRepository EmployeeStatusRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployeeStatusRepository = EmployeeStatusRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(VisualStatusBulkChangeRequest request, CancellationToken cancellationToken)
        {
             await   _EmployeeStatusRepository.VisualStatusBulkChange(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }





      
    }
}
