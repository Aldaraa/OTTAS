using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.GroupDetailFeature.UpdateGroupDetail;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.GroupDetailFeature.DeleteGroupDetail
{

    public sealed class DeleteGroupDetailHandler : IRequestHandler<DeleteGroupDetailRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGroupDetailRepository _GroupDetailRepository;
        private readonly IMapper _mapper;

        public DeleteGroupDetailHandler(IUnitOfWork unitOfWork, IGroupDetailRepository GroupDetailRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _GroupDetailRepository = GroupDetailRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteGroupDetailRequest request, CancellationToken cancellationToken)
        {
            var GroupDetail = _mapper.Map<GroupDetail>(request);
            _GroupDetailRepository.Delete(GroupDetail);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
