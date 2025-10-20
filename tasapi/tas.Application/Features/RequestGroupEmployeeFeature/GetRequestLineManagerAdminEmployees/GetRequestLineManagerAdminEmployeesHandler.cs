using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupEmployeeFeature.GetRequestLineManagerAdminEmployees
{

    public sealed class GetRequestLineManagerAdminEmployeesHandler : IRequestHandler<GetRequestLineManagerAdminEmployeesRequest, List<GetRequestLineManagerAdminEmployeesResponse>>
    {
        private readonly IRequestGroupEmployeeRepository _RequestGroupEmployeeRepository;
        private readonly IMapper _mapper;

        public GetRequestLineManagerAdminEmployeesHandler(IRequestGroupEmployeeRepository RequestGroupEmployeeRepository, IMapper mapper)
        {
            _RequestGroupEmployeeRepository = RequestGroupEmployeeRepository;
            _mapper = mapper;
        }

        public async Task<List<GetRequestLineManagerAdminEmployeesResponse>> Handle(GetRequestLineManagerAdminEmployeesRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestGroupEmployeeRepository.GetLineManagerAdminEmployees(request, cancellationToken);
            return data;
        }
    }
}
