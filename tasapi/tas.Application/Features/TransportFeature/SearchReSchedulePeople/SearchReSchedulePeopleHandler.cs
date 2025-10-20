using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.TransportFeature.SearchReSchedulePeople
{

    public sealed class SearchReSchedulePeopleHandler : IRequestHandler<SearchReSchedulePeopleRequest, SearchReSchedulePeopleResponse>
    {
        private readonly ITransportRepository _ITransportRepository;
        private readonly IMapper _mapper;

        public SearchReSchedulePeopleHandler(ITransportRepository TransportRepository, IMapper mapper)
        {
            _ITransportRepository = TransportRepository;
            _mapper = mapper;
        }

        public async Task<SearchReSchedulePeopleResponse> Handle(SearchReSchedulePeopleRequest request, CancellationToken cancellationToken)
        {

            return await _ITransportRepository.SearchReschuleData(request, cancellationToken);




        }
    }
}
