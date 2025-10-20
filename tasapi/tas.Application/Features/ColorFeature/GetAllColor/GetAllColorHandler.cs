using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.ColorFeature.GetAllColor
{

    public sealed class GetAllColorHandler : IRequestHandler<GetAllColorRequest, List<GetAllColorResponse>>
    {
        private readonly IColorRepository _ColorRepository;
        private readonly IMapper _mapper;

        public GetAllColorHandler(IColorRepository ColorRepository, IMapper mapper)
        {
            _ColorRepository = ColorRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllColorResponse>> Handle(GetAllColorRequest request, CancellationToken cancellationToken)
        {
            if (request.status.HasValue)
            {
                var Colors = await _ColorRepository.GetAllActiveFilter((int)request.status, cancellationToken);
                return _mapper.Map<List<GetAllColorResponse>>(Colors);
            }
            else {
                var users = await _ColorRepository.GetAll(cancellationToken);
                return _mapper.Map<List<GetAllColorResponse>>(users);
            }

        }
    }
}
