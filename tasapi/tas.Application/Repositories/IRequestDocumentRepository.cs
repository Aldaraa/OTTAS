using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.ApproveRequestDocument;
using tas.Application.Features.RequestDocumentFeature.CancelRequestDocument;
using tas.Application.Features.RequestDocumentFeature.ChangeLineManagerRequestDocument;
using tas.Application.Features.RequestDocumentFeature.CheckDemobRequest;
using tas.Application.Features.RequestDocumentFeature.CheckDuplicateRequestDocument;
using tas.Application.Features.RequestDocumentFeature.DeclineRequestDocument;
using tas.Application.Features.RequestDocumentFeature.ExistingBookingRequestDocument;
using tas.Application.Features.RequestDocumentFeature.GenerateCompletedDeclinedChange;
using tas.Application.Features.RequestDocumentFeature.GenerateDescriptionTest;
using tas.Application.Features.RequestDocumentFeature.GetDocumentList;
using tas.Application.Features.RequestDocumentFeature.GetDocumentListCancelled;
using tas.Application.Features.RequestDocumentFeature.GetDocumentListInpersonate;
using tas.Application.Features.RequestDocumentFeature.GetNonSiteTravelGroup;
using tas.Application.Features.RequestDocumentFeature.GetNonSiteTravelMaster;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentMyInfo;
using tas.Application.Features.RequestDocumentFeature.RemoveCancelRequestDocument;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IRequestDocumentRepository : IBaseRepository<RequestDocument>
    {

        Task<GetNonSiteTravelMasterResponse> GetMaster(GetNonSiteTravelMasterRequest request, CancellationToken cancellationToken);

        Task<GetDocumentListResponse> GetDocumentList(GetDocumentListRequest request, CancellationToken cancellationToken);
        Task DocumentEmployeeActiveCheck(int documentId);

        Task DocumentEmployeeActiveCompleteAddTravelCheck(int documentId);

        Task<GetDocumentListCancelledResponse> GetDocumentListCancelled(GetDocumentListCancelledRequest request, CancellationToken cancellationToken);

        Task CancelRequestDocument(CancelRequestDocumentRequest request, CancellationToken cancellationToken);
        Task RemoveCancelRequestDocument(RemoveCancelRequestDocumentRequest request, CancellationToken cancellationToken);


        Task DeclineRequestDocument(DeclineRequestDocumentRequest request, CancellationToken cancellationToken);

        Task ApproveRequestDocument(ApproveRequestDocumentRequest request, CancellationToken cancellationToken);


        Task ChangeLineManagerRequestDocument(ChangeLineManagerRequestDocumentRequest request, CancellationToken cancellationToken);


        Task<List<CheckDuplicateRequestDocumentResponse>> CheckDuplicateRequestDocument(CheckDuplicateRequestDocumentRequest request, CancellationToken cancellationToken);

        Task<ExistingBookingRequestDocumentResponse> ExistingBookingRequestDocument(ExistingBookingRequestDocumentRequest request, CancellationToken cancellationToken);

        Task<List<GetNonSiteTravelGroupResponse>> GetNonSiteTravelGroup(GetNonSiteTravelGroupRequest request, CancellationToken cancellationToken);
        Task<List<GetDocumentListInpersonateResponse>> GetDocumentListInpersonate(GetDocumentListInpersonateRequest request, CancellationToken cancellationToken);

        Task<GetRequestDocumentMyInfoResponse> GetRequestDocumentMyInfo(GetRequestDocumentMyInfoRequest request, CancellationToken cancellationToken);

        // TODO: Only available for developer use.
        Task GenerateDescription(int documentId, CancellationToken cancellationToken);

        Task GenerateUpdateDocumentInfo(int documentId, CancellationToken cancellationToken);



        // TODO: Only available for developer use.
        Task GenerateCompletedDeclinedChange(GenerateCompletedDeclinedChangeRequest request, CancellationToken cancellationToken);


        Task GenerateDescriptionTest(GenerateDescriptionTestRequest request, CancellationToken cancellationToken);


        Task DocumentEmployeeActiveCompleteExternalAddTravelCheck(int documentId);


        Task<List<int>> GetRoleEmployeeIds();

        Task CheckDemobRequest(CheckDemobRequestRequest request, CancellationToken cancellationToken);









    }
}

