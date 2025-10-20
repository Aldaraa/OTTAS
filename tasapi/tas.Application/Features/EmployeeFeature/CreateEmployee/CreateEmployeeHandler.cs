using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using tas.Application.Extensions;
using tas.Application.Repositories;
using tas.Application.Utils;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.CreateEmployee
{
    public sealed class CreateEmployeeHandler : IRequestHandler<CreateEmployeeRequest, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public CreateEmployeeHandler(IUnitOfWork unitOfWork, IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<int>  Handle(CreateEmployeeRequest request, CancellationToken cancellationToken)
        {
            await  _EmployeeRepository.CreateEmployeeValidateDB(request, cancellationToken);
            var Employee = _mapper.Map<Employee>(request);
            IFormFile file = request.PassportRawImage;
            var fileInfo = file.SaveImageFile();
            if (fileInfo.Status)
            {
                Employee.PassportImage = fileInfo.FileName;
            }
            if (!string.IsNullOrWhiteSpace(Employee.Firstname) && string.IsNullOrWhiteSpace(Employee.MFirstname))
            {
                Employee.MFirstname = Transliterator.LatinToCyrillic(Employee.Firstname);
            }
            if (!string.IsNullOrWhiteSpace(Employee.Lastname) && string.IsNullOrWhiteSpace(Employee.MLastname))
            {
                Employee.MLastname = Transliterator.LatinToCyrillic(Employee.Lastname);
            }


            Employee.Active = 1;
            _EmployeeRepository.Create(Employee);
            await _unitOfWork.Save(cancellationToken);
            return Employee.Id;
     
        }
    }
}
