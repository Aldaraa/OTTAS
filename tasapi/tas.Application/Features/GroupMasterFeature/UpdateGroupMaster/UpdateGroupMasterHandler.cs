using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.GroupMasterFeature.UpdateGroupMaster
{
    public class UpdateGroupMasterHandler : IRequestHandler<UpdateGroupMasterRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGroupMasterRepository _GroupMasterRepository;
        private readonly IMapper _mapper;

        public UpdateGroupMasterHandler(IUnitOfWork unitOfWork, IGroupMasterRepository GroupMasterRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _GroupMasterRepository = GroupMasterRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateGroupMasterRequest request, CancellationToken cancellationToken)
        {
            var GroupMaster = _mapper.Map<GroupMaster>(request);
            await _GroupMasterRepository.CheckDuplicateData(GroupMaster, c => c.Description);
            _GroupMasterRepository.Update(GroupMaster);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
