
using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.CreateReportJobTimeValidate
{

    public sealed class CreateReportJobTimeValidateHandler : IRequestHandler<CreateReportJobTimeValidateRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportJobRepository _ReportJobRepository;
        public CreateReportJobTimeValidateHandler(IReportJobRepository reportJobRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ReportJobRepository = reportJobRepository;
        }

        public async Task<bool> Handle(CreateReportJobTimeValidateRequest request, CancellationToken cancellationToken)
        {
         var data =    await _ReportJobRepository.ReportJobTimeValidate(request, cancellationToken);
            return data;
        }
    }
}
