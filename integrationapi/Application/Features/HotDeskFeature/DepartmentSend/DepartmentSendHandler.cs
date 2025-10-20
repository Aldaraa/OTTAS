using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.HotDeskFeature.DepartmentSend
{

    public sealed class DepartmentSendHandler : IRequestHandler<DepartmentSendRequest>
    {
        private readonly IHotDeskRepository _HotDeskRepository;
        private readonly IMapper _mapper;

        public DepartmentSendHandler(IHotDeskRepository HotDeskRepository, IMapper mapper)
        {
            _HotDeskRepository = HotDeskRepository;
            _mapper = mapper;
        }

        public async Task Handle(DepartmentSendRequest request, CancellationToken cancellationToken)
        {
             await _HotDeskRepository.DepartmentSendData(request, cancellationToken);
            return;

        }
    }
}
