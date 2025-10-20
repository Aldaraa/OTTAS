using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.SafeModeEmployeeStatusFeature.SetRREmployeeStatus
{
    public sealed class SetRREmployeeStatusHandler : IRequestHandler<SetRREmployeeStatusRequest, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISafeModeEmployeeStatusRepository _safeModeEmployeeStatusRepository;


        public SetRREmployeeStatusHandler(IUnitOfWork unitOfWork, ISafeModeEmployeeStatusRepository safeModeEmployeeStatusRepository)
        {
            _unitOfWork = unitOfWork;
            _safeModeEmployeeStatusRepository = safeModeEmployeeStatusRepository;
        }

        public async Task<int>  Handle(SetRREmployeeStatusRequest request, CancellationToken cancellationToken)
        {
        var returndata =   await _safeModeEmployeeStatusRepository.SetRREmployeeStatus(request, cancellationToken);
            return returndata;
        }
    }
}
