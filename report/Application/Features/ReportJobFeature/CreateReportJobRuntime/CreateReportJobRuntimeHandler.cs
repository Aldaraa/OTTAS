
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

namespace Application.Features.ReportJobFeature.CreateReportJobRuntime
{

    public sealed class CreateReportJobRuntimeHandler : IRequestHandler<CreateReportJobRuntimeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportJobRepository _ReportJobRepository;
        private readonly IMapper _mapper;

        public CreateReportJobRuntimeHandler(IReportJobRepository reportJobRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ReportJobRepository = reportJobRepository;
        }

        public async Task<Unit> Handle(CreateReportJobRuntimeRequest request, CancellationToken cancellationToken)
        {
            var validationRequest = await _ReportJobRepository.ReportJobTimeValidate(new CreateReportJobTimeValidateRequest(request.executeDate, null), cancellationToken);
            if (validationRequest)
            {
                await _ReportJobRepository.CreateJobScheduleRuntime(request, cancellationToken);
                await _unitOfWork.Save(cancellationToken);
                return Unit.Value;
            }
            else {
                throw new BadRequestException("Unable to schedule. The selected time conflicts with an existing schedule. Please choose a different time slot.");
            }

        }
    }
}
