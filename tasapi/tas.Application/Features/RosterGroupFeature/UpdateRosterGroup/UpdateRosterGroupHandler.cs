using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RosterGroupFeature.UpdateRosterGroup
{
    public class UpdateRosterGroupHandler : IRequestHandler<UpdateRosterGroupRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRosterGroupRepository _RosterGroupRepository;
        private readonly IMapper _mapper;

        public UpdateRosterGroupHandler(IUnitOfWork unitOfWork, IRosterGroupRepository RosterGroupRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RosterGroupRepository = RosterGroupRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateRosterGroupRequest request, CancellationToken cancellationToken)
        {
            var RosterGroup = _mapper.Map<RosterGroup>(request);
            _RosterGroupRepository.Update(RosterGroup);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
