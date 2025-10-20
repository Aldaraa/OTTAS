using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.GetAllEmployee;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentHistoryFeature.GetRequestDocumentHistory
{

    public sealed class GetRequestDocumentHistoryHandler : IRequestHandler<GetRequestDocumentHistoryRequest, List<GetRequestDocumentHistoryResponse>>
    {
        private readonly IRequestDocumentHistoryRepository _RequestDocumentHistoryRepository;
        private readonly IMapper _mapper;

        public GetRequestDocumentHistoryHandler(IRequestDocumentHistoryRepository RequestDocumentHistoryRepository, IMapper mapper)
        {
            _RequestDocumentHistoryRepository = RequestDocumentHistoryRepository;
            _mapper = mapper;
        }

        public async Task<List<GetRequestDocumentHistoryResponse>> Handle(GetRequestDocumentHistoryRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestDocumentHistoryRepository.GetRequestDocumentHistories(request, cancellationToken);
            return data;

        }
    }
}