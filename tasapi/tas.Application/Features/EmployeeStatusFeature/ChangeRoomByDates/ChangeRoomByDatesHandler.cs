using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.CreateUser;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeStatusFeature.ChangeRoomByDates
{
    public sealed class ChangeRoomByDatesHandler : IRequestHandler<ChangeRoomByDatesRequest, ChangeRoomByDatesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeStatusRepository _EmployeeStatusRepository;
        private readonly IMapper _mapper;
        public ChangeRoomByDatesHandler(IUnitOfWork unitOfWork, IEmployeeStatusRepository EmployeeStatusRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployeeStatusRepository = EmployeeStatusRepository;
            _mapper = mapper;
        }

        public async Task<ChangeRoomByDatesResponse>  Handle(ChangeRoomByDatesRequest request, CancellationToken cancellationToken)
        {
            var returnData =  await  _EmployeeStatusRepository.ChangeRoomByDatesAssign(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return returnData;
        }
    }
}
