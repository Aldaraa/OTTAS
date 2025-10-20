using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.GroupMembersFeature.CreateGroupMembers;
using tas.Application.Features.StatusChangesEmployeeRequestFeature.DeleteStatusChangesEmployeeRequest;
using tas.Application.Features.StatusChangesEmployeeRequestFeature.GetStatusChangesEmployeeRequestDeActive;
using tas.Application.Features.StatusChangesEmployeeRequestFeature.GetStatusChangesEmployeeRequestReActive;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface IStatusChangesEmployeeRequestRepository : IBaseRepository<StatusChangesEmployeeRequest>
    {

        Task <List<GetStatusChangesEmployeeRequestReActiveResponse>> GetAllReActiveData(CancellationToken cancellation);


        Task<List<GetStatusChangesEmployeeRequestDeActiveResponse>> GetAllDeActiveData(CancellationToken cancellation);

   

        Task DeleteStatusChangesEmployeeRequest(DeleteStatusChangesEmployeeRequestRequest request, CancellationToken cancellationToken);
    }
}
