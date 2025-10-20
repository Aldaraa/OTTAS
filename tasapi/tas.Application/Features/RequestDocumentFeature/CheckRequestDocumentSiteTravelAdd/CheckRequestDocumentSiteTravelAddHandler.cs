using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.CheckRequestDocumentSiteTravelAdd
{ 
    public sealed class CheckRequestDocumentSiteTravelHandler : IRequestHandler<CheckRequestDocumentSiteTravelAddRequest, List<CheckRequestDocumentSiteTravelAddResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestSiteTravelAddRepository _RequestSiteTravelAddRepository;

        public CheckRequestDocumentSiteTravelHandler(IUnitOfWork unitOfWork, IRequestSiteTravelAddRepository requestSiteTravelAddRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestSiteTravelAddRepository = requestSiteTravelAddRepository;
        }


        public async Task<List<CheckRequestDocumentSiteTravelAddResponse>> Handle(CheckRequestDocumentSiteTravelAddRequest request, CancellationToken cancellationToken)
        {

            var data = await _RequestSiteTravelAddRepository.CheckRequestDocumentSiteTravelAdd(request, cancellationToken);

            return data;
        }
    }
}
