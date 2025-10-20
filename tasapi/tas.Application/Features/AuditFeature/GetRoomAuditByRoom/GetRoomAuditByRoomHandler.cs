using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.AuditFeature.GetRoomAuditByRoom
{
    public sealed class GetRoomAuditByRoomHandler : IRequestHandler<GetRoomAuditByRoomRequest, GetRoomAuditByRoomResponse>
    {
        private readonly IAuditRepository _AuditRepository;

        public GetRoomAuditByRoomHandler(IAuditRepository AuditRepository)
        {
            _AuditRepository = AuditRepository;
        }

        public async Task<GetRoomAuditByRoomResponse>  Handle(GetRoomAuditByRoomRequest request, CancellationToken cancellationToken)
        {
          return await _AuditRepository.GetRoomAuditByRoom(request, cancellationToken);
        }
    }
}
