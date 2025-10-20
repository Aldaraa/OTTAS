using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.CreateUser;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.GroupDetailFeature.CreateGroupDetail
{
    public sealed class CreateDepartmentHandler : IRequestHandler<CreateGroupDetailRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGroupDetailRepository _GroupDetailRepository;
        private readonly IMapper _mapper;

        public CreateDepartmentHandler(IUnitOfWork unitOfWork, IGroupDetailRepository GroupDetailRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _GroupDetailRepository = GroupDetailRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateGroupDetailRequest request, CancellationToken cancellationToken)
        {
            var GroupDetail = _mapper.Map<GroupDetail>(request);
            _GroupDetailRepository.Create(GroupDetail);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
