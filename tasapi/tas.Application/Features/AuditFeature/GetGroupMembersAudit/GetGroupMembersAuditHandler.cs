using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.AuditFeature.GetGroupMembersAudit
{
    public sealed class GetGroupMembersAuditHandler : IRequestHandler<GetGroupMembersAuditRequest, List<GetGroupMembersAuditResponse>>
    {
        private readonly IAuditRepository _AuditRepository;

        public GetGroupMembersAuditHandler(IAuditRepository AuditRepository)
        {
            _AuditRepository = AuditRepository;
        }

        public async Task<List<GetGroupMembersAuditResponse>>  Handle(GetGroupMembersAuditRequest request, CancellationToken cancellationToken)
        {
          return await _AuditRepository.GetGroupMembersAudit(request, cancellationToken);
        }
    }
}
