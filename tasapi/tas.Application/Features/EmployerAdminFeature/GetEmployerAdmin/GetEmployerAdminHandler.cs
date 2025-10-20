using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.EmployerAdminFeature.GetEmployerAdmin
{

    public sealed class GetEmployerAdminHandler : IRequestHandler<GetEmployerAdminRequest, List<GetEmployerAdminResponse>>
    {
        private readonly IEmployerAdminRepository _EmployerAdminRepository;
        private readonly IMapper _mapper;

        public GetEmployerAdminHandler(IEmployerAdminRepository EmployerAdminRepository, IMapper mapper)
        {
            _EmployerAdminRepository = EmployerAdminRepository;
            _mapper = mapper;
        }

        public async Task<List<GetEmployerAdminResponse>> Handle(GetEmployerAdminRequest request, CancellationToken cancellationToken)
        {
            return await _EmployerAdminRepository.GetEmployerAdmin(request, cancellationToken);
        }
    }
}
