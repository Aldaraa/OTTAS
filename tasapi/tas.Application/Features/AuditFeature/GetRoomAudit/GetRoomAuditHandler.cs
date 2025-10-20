using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.AuditFeature.GetRoomAudit
{
    public sealed class GetRoomAuditHandler : IRequestHandler<GetRoomAuditRequest, GetRoomAuditResponse>
    {
        private readonly IAuditRepository _AuditRepository;

        public GetRoomAuditHandler(IAuditRepository AuditRepository)
        {
            _AuditRepository = AuditRepository;
        }

        public async Task<GetRoomAuditResponse>  Handle(GetRoomAuditRequest request, CancellationToken cancellationToken)
        {
          return await _AuditRepository.GetRoomAudit(request, cancellationToken);
        }
    }
}
