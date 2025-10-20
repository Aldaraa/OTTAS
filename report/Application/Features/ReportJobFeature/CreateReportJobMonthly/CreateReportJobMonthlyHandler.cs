
using Application.Common.Exceptions;
using Application.Features.ReportJobFeature.CreateReportJobTimeValidate;
using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.CreateReportJobMonthly
{

    public sealed class CreateReportJobMonthlyHandler : IRequestHandler<CreateReportJobMonthlyRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportJobRepository _ReportJobRepository;
        private readonly IMapper _mapper;

        public CreateReportJobMonthlyHandler(IReportJobRepository reportJobRepository, IUnitOfWork unitOfWork)
        {
            _ReportJobRepository = reportJobRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(CreateReportJobMonthlyRequest request, CancellationToken cancellationToken)
        {

            var validationRequest = await _ReportJobRepository.ReportJobTimeValidate(new CreateReportJobTimeValidateRequest(request.startDate, null), cancellationToken);
            if (validationRequest)
            {
                await _ReportJobRepository.CreateJobScheduleMonthly(request, cancellationToken);
                await _unitOfWork.Save(cancellationToken);
                return Unit.Value;
            }
            else
            {
                throw new BadRequestException("Unable to schedule. The selected time conflicts with an existing schedule. Please choose a different time slot.");
            }


        }
    }
}
