using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.BulkUploadEmployee
{
    public sealed class BulkUploadEmployeeHandler : IRequestHandler<BulkUploadEmployeeRequest, List<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public BulkUploadEmployeeHandler(IUnitOfWork unitOfWork, IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<List<int>>  Handle(BulkUploadEmployeeRequest request, CancellationToken cancellationToken)
        {
           var returnData =  await _EmployeeRepository.BulkRequestUpload(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return returnData;
        }
    }
}
