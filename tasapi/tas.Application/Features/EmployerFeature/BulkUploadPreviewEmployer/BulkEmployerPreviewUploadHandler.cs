using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployerFeature.BulkUploadPreviewEmployer
{
    public sealed class BulkEmployerUploadPreviewHandler : IRequestHandler<BulkEmployerUploadPreviewRequest, BulkEmployerUploadPreviewResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployerRepository _EmployerRepository;
        private readonly IMapper _mapper;

        public BulkEmployerUploadPreviewHandler(IUnitOfWork unitOfWork, IEmployerRepository EmployerRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployerRepository = EmployerRepository;
            _mapper = mapper;
        }

        public async Task<BulkEmployerUploadPreviewResponse>  Handle(BulkEmployerUploadPreviewRequest request, CancellationToken cancellationToken)
        {
          var returnData =  await _EmployerRepository.BulkRequestPreview(request, cancellationToken);
            return returnData;
        }
    }
}
