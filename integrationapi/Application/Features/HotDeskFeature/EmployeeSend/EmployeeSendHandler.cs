using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.HotDeskFeature.EmployeeSend
{

    public sealed class EmployeeSendHandler : IRequestHandler<EmployeeSendRequest>
    {
        private readonly IHotDeskRepository _HotDeskRepository;
        private readonly IMapper _mapper;

        public EmployeeSendHandler(IHotDeskRepository HotDeskRepository, IMapper mapper)
        {
            _HotDeskRepository = HotDeskRepository;
            _mapper = mapper;
        }

        public async Task Handle(EmployeeSendRequest request, CancellationToken cancellationToken)
        {
             await _HotDeskRepository.EmployeeSendData(request, cancellationToken);
            return;

        }
    }
}
