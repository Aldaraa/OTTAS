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

namespace tas.Application.Features.EmployeeStatusFeature.RoomBookingEmployee
{

    public sealed class RoomBookingEmployeeHandler : IRequestHandler<RoomBookingEmployeeRequest, List<RoomBookingEmployeeResponse>>
    {
        private readonly IEmployeeStatusRepository _EmployeeStatusRepository;
        private readonly IMapper _mapper;

        public RoomBookingEmployeeHandler(IEmployeeStatusRepository EmployeeStatusRepository, IMapper mapper)
        {
            _EmployeeStatusRepository = EmployeeStatusRepository;
            _mapper = mapper;
        }

        public async Task<List<RoomBookingEmployeeResponse>> Handle(RoomBookingEmployeeRequest request, CancellationToken cancellationToken)
        {

            var data = await _EmployeeStatusRepository.EmployeeRoombooking(request, cancellationToken);
            return data;


        }
    }
}
