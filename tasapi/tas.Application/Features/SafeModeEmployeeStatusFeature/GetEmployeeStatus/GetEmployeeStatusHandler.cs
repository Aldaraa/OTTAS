using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.SafeModeEmployeeStatusFeature.GetEmployeeStatus
{
    public sealed class GetEmployeeStatusHandler : IRequestHandler<GetEmployeeStatusRequest, GetEmployeeStatusResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISafeModeEmployeeStatusRepository _safeModeEmployeeStatusRepository;


        public GetEmployeeStatusHandler(IUnitOfWork unitOfWork, ISafeModeEmployeeStatusRepository safeModeEmployeeStatusRepository)
        {
            _unitOfWork = unitOfWork;
            _safeModeEmployeeStatusRepository = safeModeEmployeeStatusRepository;
        }

        public async Task<GetEmployeeStatusResponse>  Handle(GetEmployeeStatusRequest request, CancellationToken cancellationToken)
        {
           var data = await _safeModeEmployeeStatusRepository.GetEmployeeStatus(request, cancellationToken);
            return data;
        }
    }
}
