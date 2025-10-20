using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupEmployeeFeature.RequestGroupActiveEmployees
{

    public sealed class RequestGroupActiveEmployeesHandler : IRequestHandler<RequestGroupActiveEmployeesRequest, List<RequestGroupActiveEmployeesResponse>>
    {
        private readonly IRequestGroupEmployeeRepository _RequestGroupEmployeeRepository;
        private readonly IMapper _mapper;

        public RequestGroupActiveEmployeesHandler(IRequestGroupEmployeeRepository RequestGroupEmployeeRepository, IMapper mapper)
        {
            _RequestGroupEmployeeRepository = RequestGroupEmployeeRepository;
            _mapper = mapper;
        }

        public async Task<List<RequestGroupActiveEmployeesResponse>> Handle(RequestGroupActiveEmployeesRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestGroupEmployeeRepository.GetActiveEmployees(request, cancellationToken);
            return data;
        }
    }
}
