
using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.GetAvailableTimeSlots
{

    public sealed class GetAvailableTimeSlotsHandler : IRequestHandler<GetAvailableTimeSlotsRequest, List<DateTime>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportJobRepository _ReportJobRepository;
        public GetAvailableTimeSlotsHandler(IReportJobRepository reportJobRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ReportJobRepository = reportJobRepository;
        }

        public async Task<List<DateTime>> Handle(GetAvailableTimeSlotsRequest request, CancellationToken cancellationToken)
        {
         var data =    await _ReportJobRepository.GetAvailableTimeSlots(request, cancellationToken);
            return data;
        }
    }
}
