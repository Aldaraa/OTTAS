using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.OtinfoFeature.JobInfo
{

    public sealed class JobInfoHandler : IRequestHandler<JobInfoRequest, List<JobInfoResponse>>
    {
        private readonly ITransportInfoRepository _transportInfoRepository;
        private readonly IMapper _mapper;

        public JobInfoHandler(ITransportInfoRepository transportInfoRepository, IMapper mapper)
        {
            _transportInfoRepository = transportInfoRepository;
            _mapper = mapper;
        }

        public async Task<List<JobInfoResponse>> Handle(JobInfoRequest request, CancellationToken cancellationToken)
        {
            var data = await _transportInfoRepository.JobInfo(request, cancellationToken);
            return data;

        }
    }
}
