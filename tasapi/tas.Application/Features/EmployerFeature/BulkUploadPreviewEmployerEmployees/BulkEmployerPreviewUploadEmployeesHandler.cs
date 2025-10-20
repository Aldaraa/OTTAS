using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployerFeature.BulkUploadPreviewEmployerEmployees
{
    public sealed class BulkEmployerUploadPreviewEmployeesHandler : IRequestHandler<BulkEmployerUploadPreviewEmployeesRequest, BulkEmployerUploadPreviewEmployeesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployerRepository _EmployerRepository;
        private readonly IMapper _mapper;

        public BulkEmployerUploadPreviewEmployeesHandler(IUnitOfWork unitOfWork, IEmployerRepository EmployerRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployerRepository = EmployerRepository;
            _mapper = mapper;
        }

        public async Task<BulkEmployerUploadPreviewEmployeesResponse>  Handle(BulkEmployerUploadPreviewEmployeesRequest request, CancellationToken cancellationToken)
        {
          var returnData =  await _EmployerRepository.BulkRequestPreviewEmployees(request, cancellationToken);
            return returnData;
        }
    }
}
