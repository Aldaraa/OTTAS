using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupEmployeeFeature.GetAllRequestGroupEmployees
{

    public sealed class GetAllRequestGroupEmployeesHandler : IRequestHandler<GetAllRequestGroupEmployeesRequest, List<GetAllRequestGroupEmployeesResponse>>
    {
        private readonly IRequestGroupEmployeeRepository _RequestGroupEmployeeRepository;
        private readonly IMapper _mapper;

        public GetAllRequestGroupEmployeesHandler(IRequestGroupEmployeeRepository RequestGroupEmployeeRepository, IMapper mapper)
        {
            _RequestGroupEmployeeRepository = RequestGroupEmployeeRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllRequestGroupEmployeesResponse>> Handle(GetAllRequestGroupEmployeesRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestGroupEmployeeRepository.GetAllGroupEmployees(request, cancellationToken);
            return data;
        }
    }
}
