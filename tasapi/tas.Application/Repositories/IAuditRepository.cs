using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.AgreementFeature.CreateAgreement;
using tas.Application.Features.AgreementFeature.GetAgreement;
using tas.Application.Features.AuditFeature.GetEmployeeAudit;
using tas.Application.Features.AuditFeature.GetGroupMembersAudit;
using tas.Application.Features.AuditFeature.GetMasterAudit;
using tas.Application.Features.AuditFeature.GetRoomAudit;
using tas.Application.Features.AuditFeature.GetRoomAuditByRoom;
using tas.Application.Features.AuditFeature.GetTransportAudit;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{


    public interface IAuditRepository : IBaseRepository<Audit>
    {
        Task<GetRoomAuditResponse> GetRoomAudit(GetRoomAuditRequest request, CancellationToken cancellationToken);

        Task<GetRoomAuditByRoomResponse> GetRoomAuditByRoom(GetRoomAuditByRoomRequest request, CancellationToken cancellationToken);



        Task<GetTransportAuditResponse> GetTransportAudit(GetTransportAuditRequest request, CancellationToken cancellationToken);

        Task<GetEmployeeAuditResponse> GetEmployeeAudit(GetEmployeeAuditRequest request, CancellationToken cancellationToken);
        Task<List<GetMasterAuditResponse>> GetMasterAudit(GetMasterAuditRequest request, CancellationToken cancellationToken);


        Task<List<GetGroupMembersAuditResponse>> GetGroupMembersAudit(GetGroupMembersAuditRequest request, CancellationToken cancellationToken);




    }
}
    