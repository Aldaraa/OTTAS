using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestGroupConfigFeature.GetRequestDocumentType;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroup;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupAdd;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupById;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupByType;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupEmpLines;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupOrder;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupRemove;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupUpdate;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentRoute;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IRequestGroupConfigRepository : IBaseRepository<RequestGroupConfig>
    {
        Task<List<GetRequestDocumentTypeResponse>> GetAllDocuments(GetRequestDocumentTypeRequest request, CancellationToken cancellationToken);

        Task<List<RequestDocumentGroupResponse>> GetApproval(RequestDocumentGroupRequest request, CancellationToken cancellationToken);

        Task AddApproval(RequestDocumentGroupAddRequest request, CancellationToken cancellationToken);


        Task UpdateApproval(RequestDocumentGroupUpdateRequest request, CancellationToken cancellationToken);

        Task OrderApproval(RequestDocumentGroupOrderRequest request, CancellationToken cancellationToken);

        Task RemoveApproval(RequestDocumentGroupRemoveRequest request, CancellationToken cancellationToken);

        public Task<List<RequestDocumentGroupByIdResponse>> GetGroupsAndMembersById(RequestDocumentGroupByIdRequest request, CancellationToken cancellationToken);
    
        
        public Task<RequestDocumentGroupByTypeResponse> GetGroupsAndMembersByType(RequestDocumentGroupByTypeRequest request, CancellationToken cancellationToken);






        public Task<List<RequestDocumentRouteResponse>> GetRequestDocumentRoute(RequestDocumentRouteRequest request, CancellationToken cancellationToken);

         Task <List<RequestDocumentGroupEmpLinesResponse>> GetEmployeeLineManagers(RequestDocumentGroupEmpLinesRequest request, CancellationToken cancellationToken);
    }
}
