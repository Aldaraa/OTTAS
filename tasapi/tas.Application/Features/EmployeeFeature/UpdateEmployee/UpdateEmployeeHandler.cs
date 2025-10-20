using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Extensions;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Application.Utils;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.UpdateEmployee
{
    public class UpdateEmployeeHandler : IRequestHandler<UpdateEmployeeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;
        private readonly HTTPUserRepository _userRepository;
        private readonly CacheService _memoryCache;

        public UpdateEmployeeHandler(IUnitOfWork unitOfWork, IEmployeeRepository EmployeeRepository, IMapper mapper, HTTPUserRepository userRepository, CacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _memoryCache = cacheService;
        }

        public async Task<Unit> Handle(UpdateEmployeeRequest request, CancellationToken cancellationToken)
        {
            await _EmployeeRepository.UpdateEmployeeValidateDB(request, cancellationToken);
            var Employee = _mapper.Map<Employee>(request);
            var userRole = _userRepository.LogCurrentUser()?.Role; 


            if (request.PassportRawImage?.Length > 0)
            {
                IFormFile file = request.PassportRawImage;

                var fileInfo = file.SaveImageFile();
                if (fileInfo.Status)
                {
                    Employee.PassportImage = fileInfo.FileName;
                }
            }

            if (request.Active == null)
            {
                Employee.Active = 0;
                if (!request.CompletionDate.HasValue) { 
                    Employee.CompletionDate = DateTime.Now; 
                }
            }
            if (!string.IsNullOrWhiteSpace(Employee.Firstname)  && string.IsNullOrWhiteSpace(Employee.MFirstname))
            {
                Employee.MFirstname = Transliterator.LatinToCyrillic(Employee.Firstname);
            }
            if (!string.IsNullOrWhiteSpace(Employee.Lastname) && string.IsNullOrWhiteSpace(Employee.MLastname))
            {
                Employee.MLastname = Transliterator.LatinToCyrillic(Employee.Lastname);
            }
            





            await _EmployeeRepository.Patch(Employee);
        //    await _unitOfWork.Save(cancellationToken);
            await _EmployeeRepository.UpdateProfileChangeData(Employee);
              
             await _unitOfWork.Save(cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.ADAccount)) 
            {
                _userRepository.ClearRoleCache(request.ADAccount);
            }
            return Unit.Value;
        }


      
    }
}
