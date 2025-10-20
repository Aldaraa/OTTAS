using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.CampFeature.GetAllCamp;
using tas.Application.Features.VisitEventFeature.CreateVisitEvent;
using tas.Application.Features.VisitEventFeature.DeleteVisitEvent;
using tas.Application.Features.VisitEventFeature.GetAllVisitEvent;
using tas.Application.Features.VisitEventFeature.GetVisitEvent;
using tas.Application.Features.VisitEventFeature.ReplaceProfile;
using tas.Application.Features.VisitEventFeature.ReplaceProfileMultiple;
using tas.Application.Features.VisitEventFeature.ReplaceProfileUndo;
using tas.Application.Features.VisitEventFeature.SetTransport;
using tas.Application.Features.VisitEventFeature.UpdateVisitEvent;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface IVisitEventRepository : IBaseRepository<VisitEvent>
    {
        Task<List<GetAllVisitEventResponse>> GetAllData(GetAllVisitEventRequest request, CancellationToken cancellationToken);

        Task  CreateData(CreateVisitEventRequest request, CancellationToken cancellationToken);

        Task  DeleteData(DeleteVisitEventRequest request, CancellationToken cancellationToken);



        Task  UpdateData(UpdateVisitEventRequest request, CancellationToken cancellationToken);

        Task EventEmployeeSetTransport(SetTransportRequest request, CancellationToken cancellationToken);

        Task<GetVisitEventResponse> GetData(GetVisitEventRequest request, CancellationToken cancellationToken);

        Task EventEmployeeReplaceProfile(ReplaceProfileRequest  request, CancellationToken cancellationToken);

        Task EventEmployeeReplaceProfileUndo(ReplaceProfileUndoRequest  request, CancellationToken cancellationToken);


        Task<List<ReplaceProfileMultipleResponse>> EventEmployeeReplaceProfileMultiple(ReplaceProfileMultipleRequest  request, CancellationToken cancellationToken);


    }
}