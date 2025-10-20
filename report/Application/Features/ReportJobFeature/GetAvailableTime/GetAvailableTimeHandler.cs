
using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.GetAvailableTime
{

    public sealed class GetAvailableTimeHandler : IRequestHandler<GetAvailableTimeRequest, List<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportJobRepository _ReportJobRepository;
        public GetAvailableTimeHandler(IReportJobRepository reportJobRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ReportJobRepository = reportJobRepository;
        }

        public async Task<List<int>> Handle(GetAvailableTimeRequest request, CancellationToken cancellationToken)
        {
         var data =    await _ReportJobRepository.GetAvailableTime(request, cancellationToken);
            return data;
        }
    }
}
