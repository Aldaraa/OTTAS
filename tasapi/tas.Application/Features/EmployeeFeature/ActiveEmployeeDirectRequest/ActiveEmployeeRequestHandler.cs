using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using tas.Application.Extensions;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.ActiveEmployeeDirectRequest
{
    public sealed class ActiveEmployeeRequestHandler : IRequestHandler<ActiveEmployeeDirectRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public ActiveEmployeeRequestHandler(IUnitOfWork unitOfWork, IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(ActiveEmployeeDirectRequest request, CancellationToken cancellationToken)
        {
            await  _EmployeeRepository.ActiveEmployeeRequestDirect(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
     
        }
    }
}
