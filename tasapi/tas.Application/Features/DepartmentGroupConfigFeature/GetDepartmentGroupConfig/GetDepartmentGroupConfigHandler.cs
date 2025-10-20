using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.CostCodeFeature.GetAllCostCode;
using tas.Application.Features.UserFeatures.CreateUser;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentGroupConfigFeature.GetDepartmentGroupConfig
{
    public sealed class GetDepartmentGroupConfigHandler : IRequestHandler<GetDepartmentGroupConfigRequest, List<GetDepartmentGroupConfigResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentGroupConfigRepository _DepartmentGroupConfigRepository;
        private readonly IMapper _mapper;

        public GetDepartmentGroupConfigHandler(IUnitOfWork unitOfWork, IDepartmentGroupConfigRepository DepartmentGroupConfigRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _DepartmentGroupConfigRepository = DepartmentGroupConfigRepository;
            _mapper = mapper;
        }

        public async Task<List<GetDepartmentGroupConfigResponse>>  Handle(GetDepartmentGroupConfigRequest request, CancellationToken cancellationToken)
        {
         var data =  await  _DepartmentGroupConfigRepository.GetDepartmentGroupConfig(request, cancellationToken);
            return data;
        }
    }
}
