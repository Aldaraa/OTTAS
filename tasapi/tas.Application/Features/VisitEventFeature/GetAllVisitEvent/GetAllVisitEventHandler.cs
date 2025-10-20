using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.GetAllEmployee;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.VisitEventFeature.GetAllVisitEvent
{

    public sealed class GetAllVisitEventHandler : IRequestHandler<GetAllVisitEventRequest, List<GetAllVisitEventResponse>>
    {
        private readonly IVisitEventRepository _VisitEventRepository;
        public GetAllVisitEventHandler(IVisitEventRepository VisitEventRepository)
        {
            _VisitEventRepository = VisitEventRepository;
        }

        public async Task<List<GetAllVisitEventResponse>> Handle(GetAllVisitEventRequest request, CancellationToken cancellationToken)
        {

            return await _VisitEventRepository.GetAllData(request, cancellationToken);


        }
    }
}
