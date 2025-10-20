using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.DepartmentCostCodeFeature.AddDepartmentCostCode
{

    public sealed class AddDepartmentCostCodeHandler : IRequestHandler<AddDepartmentCostCodeRequest,Unit>
    {
        private readonly IDepartmentCostCodeRepository _DepartmentRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AddDepartmentCostCodeHandler(IDepartmentCostCodeRepository DepartmentCostCodeRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _DepartmentRepository = DepartmentCostCodeRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(AddDepartmentCostCodeRequest request, CancellationToken cancellationToken)
        {
            await _DepartmentRepository.AddDepartmentCostCode(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;


        }
    }
}
