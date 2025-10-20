using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.HotDeskFeature.EmployeeStatusSend
{

    public sealed class EmployeeStatusSendHandler : IRequestHandler<EmployeeStatusSendRequest>
    {
        private readonly IHotDeskRepository _HotDeskRepository;
        private readonly IMapper _mapper;

        public EmployeeStatusSendHandler(IHotDeskRepository HotDeskRepository, IMapper mapper)
        {
            _HotDeskRepository = HotDeskRepository;
            _mapper = mapper;
        }

        public async Task Handle(EmployeeStatusSendRequest request, CancellationToken cancellationToken)
        {
             await _HotDeskRepository.EmployeeStatusSendData(request, cancellationToken);
            return;

        }
    }
}
