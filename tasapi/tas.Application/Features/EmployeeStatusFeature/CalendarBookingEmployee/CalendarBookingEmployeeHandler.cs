using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployerFeature.CreateEmployer;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.EmployeeStatusFeature.CalendarBookingEmployee
{

    public sealed class CalendarBookingEmployeeHandler : IRequestHandler<CalendarBookingEmployeeRequest, List<CalendarBookingEmployeeResponse>>
    {
        private readonly IEmployeeStatusRepository _EmployeeStatusRepository;
        private readonly IMapper _mapper;

        public CalendarBookingEmployeeHandler(IEmployeeStatusRepository EmployeeStatusRepository, IMapper mapper)
        {
            _EmployeeStatusRepository = EmployeeStatusRepository;
            _mapper = mapper;
        }

        public async Task<List<CalendarBookingEmployeeResponse>> Handle(CalendarBookingEmployeeRequest request, CancellationToken cancellationToken)
        {

            var data = await _EmployeeStatusRepository.EmployeeBookingViewCalendar(request, cancellationToken);
            return data;


        }
    }
}
