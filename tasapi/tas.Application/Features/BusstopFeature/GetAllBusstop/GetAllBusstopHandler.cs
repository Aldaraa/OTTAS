using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.BusstopFeature.GetAllBusstop
{

    public sealed class GetAllBusstopHandler : IRequestHandler<GetAllBusstopRequest, List<GetAllBusstopResponse>>
    {
        private readonly IBusstopRepository _BusstopRepository;
        private readonly IMapper _mapper;

        public GetAllBusstopHandler(IBusstopRepository BusstopRepository, IMapper mapper)
        {
            _BusstopRepository = BusstopRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllBusstopResponse>> Handle(GetAllBusstopRequest request, CancellationToken cancellationToken)
        {

                return await _BusstopRepository.GetAllBusstop(request,  cancellationToken);
           

        }
    }
}
