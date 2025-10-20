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

namespace tas.Application.Features.EmployeeStatusFeature.CalendarBookingRoomAssign
{
    public sealed class CalendarBookingRoomAssignHandler : IRequestHandler<CalendarBookingRoomAssignRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeStatusRepository _EmployeeStatusRepository;
        private readonly IMapper _mapper;
        public CalendarBookingRoomAssignHandler(IUnitOfWork unitOfWork, IEmployeeStatusRepository EmployeeStatusRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployeeStatusRepository = EmployeeStatusRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CalendarBookingRoomAssignRequest request, CancellationToken cancellationToken)
        {
             await  _EmployeeStatusRepository.CalendarRoomAssign(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
