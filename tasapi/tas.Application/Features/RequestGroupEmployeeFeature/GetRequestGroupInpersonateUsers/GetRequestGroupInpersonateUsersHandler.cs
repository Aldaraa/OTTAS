using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelAdd;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupEmployeeFeature.GetRequestGroupInpersonateUsers
{ 
    public sealed class GetRequestGroupInpersonateUsersHandler : IRequestHandler<GetRequestGroupInpersonateUsersRequest, List<GetRequestGroupInpersonateUsersResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestGroupEmployeeRepository _RequestGroupEmployeeRepository;
        private readonly IMapper _mapper;

        public GetRequestGroupInpersonateUsersHandler(IUnitOfWork unitOfWork, IRequestGroupEmployeeRepository requestGroupEmployeeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestGroupEmployeeRepository = requestGroupEmployeeRepository;
            _mapper = mapper;
        }

        public async Task<List<GetRequestGroupInpersonateUsersResponse>> Handle(GetRequestGroupInpersonateUsersRequest request, CancellationToken cancellationToken)
        {
            return await _RequestGroupEmployeeRepository.GetRequestGroupInpersonateUsers(request, cancellationToken);

              

        }
    }
}
