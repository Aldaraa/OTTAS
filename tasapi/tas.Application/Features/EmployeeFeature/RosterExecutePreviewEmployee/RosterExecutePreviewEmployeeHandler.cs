using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using tas.Application.Extensions;
using tas.Application.Features.EmployeeFeature.GetEmployee;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.RosterExecutePreviewEmployee
{
    public sealed class RosterExecutePreviewEmployeeHandler : IRequestHandler<RosterExecutePreviewEmployeeRequest, RosterExecutePreviewEmployeeResponse>
    {
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public RosterExecutePreviewEmployeeHandler(IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<RosterExecutePreviewEmployeeResponse> Handle(RosterExecutePreviewEmployeeRequest request, CancellationToken cancellationToken)
        {
            var returnData =  await _EmployeeRepository.RosterExecuteRequest(request, cancellationToken);

            return _mapper.Map<RosterExecutePreviewEmployeeResponse>(returnData);
        }
    }
}
