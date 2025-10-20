using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.DepartmentCostCodeFeature.DeleteDepartmentCostCode
{

    public sealed class DeleteDepartmentCostCodeHandler : IRequestHandler<DeleteDepartmentCostCodeRequest,Unit>
    {
        private readonly IDepartmentCostCodeRepository _DepartmentCostCodeRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDepartmentCostCodeHandler(IDepartmentCostCodeRepository DepartmentCostCodeRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _DepartmentCostCodeRepository = DepartmentCostCodeRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteDepartmentCostCodeRequest request, CancellationToken cancellationToken)
        {
            await _DepartmentCostCodeRepository.DeleteDepartmentCostCode(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;


        }
    }
}
