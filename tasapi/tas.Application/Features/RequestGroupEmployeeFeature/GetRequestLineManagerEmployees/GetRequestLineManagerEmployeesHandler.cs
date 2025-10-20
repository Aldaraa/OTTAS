using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupEmployeeFeature.GetRequestLineManagerEmployees
{

    public sealed class GetRequestLineManagerEmployeesHandler : IRequestHandler<GetRequestLineManagerEmployeesRequest, GetRequestLineManagerEmployeesResponse>
    {
        private readonly IRequestGroupEmployeeRepository _RequestGroupEmployeeRepository;
        private readonly IMapper _mapper;

        public GetRequestLineManagerEmployeesHandler(IRequestGroupEmployeeRepository RequestGroupEmployeeRepository, IMapper mapper)
        {
            _RequestGroupEmployeeRepository = RequestGroupEmployeeRepository;
            _mapper = mapper;
        }

        public async Task<GetRequestLineManagerEmployeesResponse> Handle(GetRequestLineManagerEmployeesRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestGroupEmployeeRepository.GetLineManagerEmployees(request, cancellationToken);
            return data;
        }
    }
}
