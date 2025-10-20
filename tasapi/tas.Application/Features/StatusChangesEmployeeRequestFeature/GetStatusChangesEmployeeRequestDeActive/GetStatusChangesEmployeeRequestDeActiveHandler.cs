using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.StatusChangesEmployeeRequestFeature.GetStatusChangesEmployeeRequestDeActive
{

    public sealed class GetStatusChangesEmployeeRequestDeActiveHandler : IRequestHandler<GetStatusChangesEmployeeRequestDeActiveRequest, List<GetStatusChangesEmployeeRequestDeActiveResponse>>
    {
        private readonly IStatusChangesEmployeeRequestRepository _StatusChangesEmployeeRequestRepository;
        private readonly IMapper _mapper;

        public GetStatusChangesEmployeeRequestDeActiveHandler(IStatusChangesEmployeeRequestRepository StatusChangesEmployeeRequestRepository, IMapper mapper)
        {
            _StatusChangesEmployeeRequestRepository = StatusChangesEmployeeRequestRepository;
            _mapper = mapper;
        }

        public async Task<List<GetStatusChangesEmployeeRequestDeActiveResponse>> Handle(GetStatusChangesEmployeeRequestDeActiveRequest request, CancellationToken cancellationToken)
        {
            return await _StatusChangesEmployeeRequestRepository.GetAllDeActiveData(cancellationToken);
        }
    }
}