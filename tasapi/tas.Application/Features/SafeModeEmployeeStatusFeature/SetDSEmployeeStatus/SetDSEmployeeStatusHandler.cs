using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.SafeModeEmployeeStatusFeature.SetDSEmployeeStatus
{
    public sealed class SetDSEmployeeStatusHandler : IRequestHandler<SetDSEmployeeStatusRequest, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISafeModeEmployeeStatusRepository _safeModeEmployeeStatusRepository;


        public SetDSEmployeeStatusHandler(IUnitOfWork unitOfWork, ISafeModeEmployeeStatusRepository safeModeEmployeeStatusRepository)
        {
            _unitOfWork = unitOfWork;
            _safeModeEmployeeStatusRepository = safeModeEmployeeStatusRepository;
        }

        public async Task<int>  Handle(SetDSEmployeeStatusRequest request, CancellationToken cancellationToken)
        {
            var returndata = await _safeModeEmployeeStatusRepository.SetDSEmployeeStatus(request, cancellationToken);
            return returndata;
        }
    }
}
