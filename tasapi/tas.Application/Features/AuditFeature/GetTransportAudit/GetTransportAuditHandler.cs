using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.AuditFeature.GetTransportAudit
{
    public sealed class GetTransportAuditHandler : IRequestHandler<GetTransportAuditRequest, GetTransportAuditResponse>
    {
        private readonly IAuditRepository _AuditRepository;

        public GetTransportAuditHandler(IAuditRepository AuditRepository)
        {
            _AuditRepository = AuditRepository;
        }

        public async Task<GetTransportAuditResponse>  Handle(GetTransportAuditRequest request, CancellationToken cancellationToken)
        {
          return await _AuditRepository.GetTransportAudit(request, cancellationToken);
        }
    }
}
