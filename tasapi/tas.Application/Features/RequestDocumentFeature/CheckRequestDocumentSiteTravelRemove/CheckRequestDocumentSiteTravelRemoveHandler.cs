using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.CheckRequestDocumentSiteTravelRemove
{ 
    public sealed class CheckRequestDocumentSiteTravelHandler : IRequestHandler<CheckRequestDocumentSiteTravelRemoveRequest, List<CheckRequestDocumentSiteTravelRemoveResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestSiteTravelRemoveRepository _RequestSiteTravelRemoveRepository;

        public CheckRequestDocumentSiteTravelHandler(IUnitOfWork unitOfWork, IRequestSiteTravelRemoveRepository requestSiteTravelRemoveRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestSiteTravelRemoveRepository = requestSiteTravelRemoveRepository;
        }


        public async Task<List<CheckRequestDocumentSiteTravelRemoveResponse>> Handle(CheckRequestDocumentSiteTravelRemoveRequest request, CancellationToken cancellationToken)
        {

            var data = await _RequestSiteTravelRemoveRepository.CheckRequestDocumentSiteTravelRemove(request, cancellationToken);

            return data;
        }
    }
}
