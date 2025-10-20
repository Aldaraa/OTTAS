using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupEmployeeFeature.GetRequestGroupEmployees
{

    public sealed class GetRequestGroupEmployeesHandler : IRequestHandler<GetRequestGroupEmployeesRequest, GetRequestGroupEmployeesResponse>
    {
        private readonly IRequestGroupEmployeeRepository _RequestGroupEmployeeRepository;
        private readonly IMapper _mapper;

        public GetRequestGroupEmployeesHandler(IRequestGroupEmployeeRepository RequestGroupEmployeeRepository, IMapper mapper)
        {
            _RequestGroupEmployeeRepository = RequestGroupEmployeeRepository;
            _mapper = mapper;
        }

        public async Task<GetRequestGroupEmployeesResponse> Handle(GetRequestGroupEmployeesRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestGroupEmployeeRepository.GetGroupEmployees(request, cancellationToken);
            return data;
        }
    }
}
