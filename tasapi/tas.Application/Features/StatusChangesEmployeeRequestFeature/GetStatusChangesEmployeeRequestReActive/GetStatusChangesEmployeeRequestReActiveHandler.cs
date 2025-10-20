using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.StatusChangesEmployeeRequestFeature.GetStatusChangesEmployeeRequestReActive
{

    public sealed class GetStatusChangesEmployeeRequestReActiveHandler : IRequestHandler<GetStatusChangesEmployeeRequestReActiveRequest, List<GetStatusChangesEmployeeRequestReActiveResponse>>
    {
        private readonly IStatusChangesEmployeeRequestRepository _StatusChangesEmployeeRequestRepository;
        private readonly IMapper _mapper;

        public GetStatusChangesEmployeeRequestReActiveHandler(IStatusChangesEmployeeRequestRepository StatusChangesEmployeeRequestRepository, IMapper mapper)
        {
            _StatusChangesEmployeeRequestRepository = StatusChangesEmployeeRequestRepository;
            _mapper = mapper;
        }

        public async Task<List<GetStatusChangesEmployeeRequestReActiveResponse>> Handle(GetStatusChangesEmployeeRequestReActiveRequest request, CancellationToken cancellationToken)
        {
            return await _StatusChangesEmployeeRequestRepository.GetAllReActiveData(cancellationToken);
        }
    }
}