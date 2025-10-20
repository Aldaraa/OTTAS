using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.CheckRequestDocumentSiteTravelReschedule
{ 
    public sealed class CheckRequestDocumentSiteTravelHandler : IRequestHandler<CheckRequestDocumentSiteTravelRescheduleRequest, List<CheckRequestDocumentSiteTravelRescheduleResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestSiteTravelRescheduleRepository _RequestSiteTravelRescheduleRepository;

        public CheckRequestDocumentSiteTravelHandler(IUnitOfWork unitOfWork, IRequestSiteTravelRescheduleRepository requestSiteTravelRescheduleRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestSiteTravelRescheduleRepository = requestSiteTravelRescheduleRepository;
        }


        public async Task<List<CheckRequestDocumentSiteTravelRescheduleResponse>> Handle(CheckRequestDocumentSiteTravelRescheduleRequest request, CancellationToken cancellationToken)
        {

            var data = await _RequestSiteTravelRescheduleRepository.CheckRequestDocumentSiteTravelReschedule(request, cancellationToken);

            return data;
        }
    }
}
