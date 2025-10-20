using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.RequestDocumentFeature.ChangeLineManagerRequestDocument;
using tas.Domain.Entities;
using tas.Domain.Enums;

namespace tas.Persistence.Repositories
{
    public partial class RequestDocumentRepository
    {
        public async Task ChangeLineManagerRequestDocument(ChangeLineManagerRequestDocumentRequest request, CancellationToken cancellationToken)
        {
            var currentUser = _HTTPUserRepository.LogCurrentUser();
            int? userId = currentUser?.Id;
            var userrole = currentUser?.Role;

            var currentDocument = await Context.RequestDocument
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (currentDocument == null)
                throw new NotFoundException($"Request document with Id {request.Id} not found.");

            if (currentDocument.CurrentAction == RequestDocumentAction.Cancelled
                || currentDocument.CurrentAction == RequestDocumentAction.Completed)
                throw new BadRequestException("Cannot change line manager for a cancelled or completed request.");

            var currentGroupConfig = await (from groupConfig in Context.RequestGroupConfig.AsNoTracking()
                                            where groupConfig.Id == currentDocument.AssignedRouteConfigId
                                            join requestGroup in Context.RequestGroup.AsNoTracking()
                                                on groupConfig.GroupId equals requestGroup.Id
                                            select new { groupConfig.Id, requestGroup.GroupTag })
                                             .FirstOrDefaultAsync(cancellationToken);

            if (currentGroupConfig == null)
                throw new BadRequestException("The request group configuration is incomplete. Please contact the admin team.");

            if (userrole == "SystemAdmin" && currentGroupConfig.GroupTag == "linemanager")
            {
                currentDocument.AssignedEmployeeId = request.newAssignEmployeeId;
                currentDocument.UserIdUpdated = userId;
                currentDocument.DateUpdated = DateTime.Now;
                Context.RequestDocument.Update(currentDocument);
            }
            else
            {
                throw new BadRequestException("Only SystemAdmin can change line manager assignment.");
            }
        }
    }
}
