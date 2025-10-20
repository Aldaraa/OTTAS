using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployerFeature.UpdateEmployer
{
    public class UpdateEmployerHandler : IRequestHandler<UpdateEmployerRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployerRepository _EmployerRepository;
        private readonly IMapper _mapper;

        public UpdateEmployerHandler(IUnitOfWork unitOfWork, IEmployerRepository EmployerRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployerRepository = EmployerRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateEmployerRequest request, CancellationToken cancellationToken)
        {
            var Employer = _mapper.Map<Employer>(request);
            await _EmployerRepository.CheckDuplicateData(Employer, c => c.Code, c => c.Description);
            _EmployerRepository.Update(Employer);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
