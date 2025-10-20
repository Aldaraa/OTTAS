using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.AuditFeature.GetMasterAudit
{
    public sealed class GetMasterAuditHandler : IRequestHandler<GetMasterAuditRequest, List<GetMasterAuditResponse>>
    {
        private readonly IAuditRepository _AuditRepository;

        public GetMasterAuditHandler(IAuditRepository AuditRepository)
        {
            _AuditRepository = AuditRepository;
        }

        public async Task<List<GetMasterAuditResponse>>  Handle(GetMasterAuditRequest request, CancellationToken cancellationToken)
        {
          return await _AuditRepository.GetMasterAudit(request, cancellationToken);
        }
    }
}
