using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.ProfileFieldFeature.GetAllProfileField
{

    public sealed class GetAllProfileFieldHandler : IRequestHandler<GetAllProfileFieldRequest, List<GetAllProfileFieldResponse>>
    {
        private readonly IProfileFieldRepository _ProfileFieldRepository;
        private readonly IMapper _mapper;

        public GetAllProfileFieldHandler(IProfileFieldRepository ProfileFieldRepository, IMapper mapper)
        {
            _ProfileFieldRepository = ProfileFieldRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllProfileFieldResponse>> Handle(GetAllProfileFieldRequest request, CancellationToken cancellationToken)
        {
            var returnData = await _ProfileFieldRepository.GetAllData(request, cancellationToken);
            return returnData;
           

        }
    }
}
