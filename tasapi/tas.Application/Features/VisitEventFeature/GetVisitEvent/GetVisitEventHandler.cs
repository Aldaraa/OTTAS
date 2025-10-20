using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.VisitEventFeature.GetVisitEvent
{

    public sealed class GetVisitEventHandler : IRequestHandler<GetVisitEventRequest, GetVisitEventResponse>
    {
        private readonly IVisitEventRepository _VisitEventRepository;
        public GetVisitEventHandler(IVisitEventRepository VisitEventRepository)
        {
            _VisitEventRepository = VisitEventRepository;
        }

        public async Task<GetVisitEventResponse> Handle(GetVisitEventRequest request, CancellationToken cancellationToken)
        {

            return await _VisitEventRepository.GetData(request, cancellationToken);


        }
    }
}
