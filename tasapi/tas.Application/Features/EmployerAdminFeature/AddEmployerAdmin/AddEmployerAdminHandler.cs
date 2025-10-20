using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.EmployerAdminFeature.AddEmployerAdmin
{

    public sealed class AddEmployerAdminHandler : IRequestHandler<AddEmployerAdminRequest,Unit>
    {
        private readonly IEmployerAdminRepository _EmployerAdminRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AddEmployerAdminHandler(IEmployerAdminRepository EmployerAdminRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _EmployerAdminRepository = EmployerAdminRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(AddEmployerAdminRequest request, CancellationToken cancellationToken)
        {
            await _EmployerAdminRepository.AddEmployerAdmin(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;


        }
    }
}
