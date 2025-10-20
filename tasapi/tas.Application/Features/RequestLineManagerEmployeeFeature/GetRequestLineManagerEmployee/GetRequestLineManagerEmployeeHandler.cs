using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestLineManagerEmployeeFeature.GetRequestLineManagerEmployee
{
    public sealed class GetRequestLineManagerEmployeeHandler : IRequestHandler<GetRequestLineManagerEmployeeRequest, List<GetRequestLineManagerEmployeeResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestLineManagerEmployeeRepository _IRequestLineManagerEmployeeRepository;

        public GetRequestLineManagerEmployeeHandler(IUnitOfWork unitOfWork, IRequestLineManagerEmployeeRepository RequestGroupRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _IRequestLineManagerEmployeeRepository = RequestGroupRepository;
        }

        public async Task<List<GetRequestLineManagerEmployeeResponse>>  Handle(GetRequestLineManagerEmployeeRequest request, CancellationToken cancellationToken)
        {
          return  await _IRequestLineManagerEmployeeRepository.GetRequestLineManagerEmployee(request, cancellationToken);
    
        }
    }
}
