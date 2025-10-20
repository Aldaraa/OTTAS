using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestGroupFeature.UpdateRequestGroup
{
    public class UpdateRequestGroupHandler : IRequestHandler<UpdateRequestGroupRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestGroupRepository _RequestGroupRepository;
        private readonly IMapper _mapper;

        public UpdateRequestGroupHandler(IUnitOfWork unitOfWork, IRequestGroupRepository RequestGroupRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestGroupRepository = RequestGroupRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateRequestGroupRequest request, CancellationToken cancellationToken)
        {
            var RequestGroup = _mapper.Map<RequestGroup>(request);
            await _RequestGroupRepository.CheckDuplicateData(RequestGroup, c => c.Description, c => c.Description);
            await _RequestGroupRepository.UpdateData(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
