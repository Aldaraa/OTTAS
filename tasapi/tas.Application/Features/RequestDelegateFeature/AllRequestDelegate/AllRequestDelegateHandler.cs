using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDelegateFeature.AllRequestDelegate
{

    public sealed class AllRequestDelegateHandler : IRequestHandler<AllRequestDelegateRequest, List<AllRequestDelegateResponse>>
    {
        private readonly IRequestDelegateRepository _RequestDelegateRepository;
        private readonly IMapper _mapper;

        public AllRequestDelegateHandler(IRequestDelegateRepository RequestDelegateRepository, IMapper mapper)
        {
            _RequestDelegateRepository = RequestDelegateRepository;
            _mapper = mapper;
        }

        public async Task<List<AllRequestDelegateResponse>> Handle(AllRequestDelegateRequest request, CancellationToken cancellationToken)
        {
              return await _RequestDelegateRepository.AllData(request, cancellationToken);


        }
    }
}
