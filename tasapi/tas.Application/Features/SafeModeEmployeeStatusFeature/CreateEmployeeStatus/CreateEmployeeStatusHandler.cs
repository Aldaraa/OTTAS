using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.SafeModeEmployeeStatusFeature.CreateEmployeeStatus
{
    public sealed class CreateEmployeeStatusHandler : IRequestHandler<CreateEmployeeStatusRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISafeModeEmployeeStatusRepository _safeModeEmployeeStatusRepository;


        public CreateEmployeeStatusHandler(IUnitOfWork unitOfWork, ISafeModeEmployeeStatusRepository safeModeEmployeeStatusRepository)
        {
            _unitOfWork = unitOfWork;
            _safeModeEmployeeStatusRepository = safeModeEmployeeStatusRepository;
        }

        public async Task<Unit>  Handle(CreateEmployeeStatusRequest request, CancellationToken cancellationToken)
        {
            await _safeModeEmployeeStatusRepository.CreateEmployeeStatus(request, cancellationToken);
            await  _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
