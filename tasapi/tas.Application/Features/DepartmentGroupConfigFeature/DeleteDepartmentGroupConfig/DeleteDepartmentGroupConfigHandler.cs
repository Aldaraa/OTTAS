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

namespace tas.Application.Features.DepartmentGroupConfigFeature.DeleteDepartmentGroupConfig
{
    public sealed class DeleteDepartmentGroupConfigHandler : IRequestHandler<DeleteDepartmentGroupConfigRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentGroupConfigRepository _DepartmentGroupConfigRepository;
        private readonly IMapper _mapper;

        public DeleteDepartmentGroupConfigHandler(IUnitOfWork unitOfWork, IDepartmentGroupConfigRepository DepartmentGroupConfigRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _DepartmentGroupConfigRepository = DepartmentGroupConfigRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(DeleteDepartmentGroupConfigRequest request, CancellationToken cancellationToken)
        {
            await  _DepartmentGroupConfigRepository.DeleteDepartmentGroupConfig(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
