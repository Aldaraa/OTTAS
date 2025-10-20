using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.AgreementFeature.GetAgreement
{

    public sealed class GetAgreementHandler : IRequestHandler<GetAgreementRequest, GetAgreementResponse>
    {
        private readonly IAgreementRepository _AgreementRepository;
        private readonly IMapper _mapper;

        public GetAgreementHandler(IAgreementRepository AgreementRepository, IMapper mapper)
        {
            _AgreementRepository = AgreementRepository;
            _mapper = mapper;
        }

        public async Task<GetAgreementResponse> Handle(GetAgreementRequest request, CancellationToken cancellationToken)
        {
            var returnData = await _AgreementRepository.GetData(request, cancellationToken);
            return returnData;


        }
    }
}
