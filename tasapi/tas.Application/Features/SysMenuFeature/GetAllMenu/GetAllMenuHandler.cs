using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.SysMenuFeature.GetAllMenu
{

    public sealed class GetAllMenuHandler : IRequestHandler<GetAllMenuRequest, List<GetAllMenuResponse>>
    {
        private readonly IMenuRepository _MenuRepository;
        private readonly IMapper _mapper;

        public GetAllMenuHandler(IMenuRepository MenuRepository, IMapper mapper)
        {
            _MenuRepository = MenuRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllMenuResponse>> Handle(GetAllMenuRequest request, CancellationToken cancellationToken)
        {
            return await _MenuRepository.GetAllMenu(cancellationToken);
        }
    }
}
