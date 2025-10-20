
using Application.Common.Exceptions;
using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportTemplateFeature.UpdateTemplateParameter
{

    public sealed class UpdateTemplateParameterHandler : IRequestHandler<UpdateTemplateParameterRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportTemplateRepository _ReportTemplateRepository;

        public UpdateTemplateParameterHandler(IReportTemplateRepository reportTemplateRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ReportTemplateRepository = reportTemplateRepository;
        }

        public async Task<Unit> Handle(UpdateTemplateParameterRequest request, CancellationToken cancellationToken)
        {
            await _ReportTemplateRepository.UpdateTemplateParameter(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
