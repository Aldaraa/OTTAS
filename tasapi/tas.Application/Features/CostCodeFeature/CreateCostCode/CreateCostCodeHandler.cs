using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.CreateUser;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.CostCodeFeature.CreateCostCode
{
    public sealed class CreateDepartmentHandler : IRequestHandler<CreateCostCodeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICostCodeRepository _costCodeRepository;
        private readonly IMapper _mapper;

        public CreateDepartmentHandler(IUnitOfWork unitOfWork, ICostCodeRepository costCodeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _costCodeRepository = costCodeRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateCostCodeRequest request, CancellationToken cancellationToken)
        {
            var costcode = _mapper.Map<CostCode>(request);

            await _costCodeRepository.CheckDuplicateData(costcode, c=> c.Number);
            _costCodeRepository.Create(costcode);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
