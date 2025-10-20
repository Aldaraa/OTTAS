using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.AuditFeature.GetEmployeeAudit
{
    public sealed class GetEmployeeAuditHandler : IRequestHandler<GetEmployeeAuditRequest, GetEmployeeAuditResponse>
    {
        private readonly IAuditRepository _AuditRepository;

        public GetEmployeeAuditHandler(IAuditRepository AuditRepository)
        {
            _AuditRepository = AuditRepository;
        }

        public async Task<GetEmployeeAuditResponse>  Handle(GetEmployeeAuditRequest request, CancellationToken cancellationToken)
        {
          return await _AuditRepository.GetEmployeeAudit(request, cancellationToken);
        }
    }
}
