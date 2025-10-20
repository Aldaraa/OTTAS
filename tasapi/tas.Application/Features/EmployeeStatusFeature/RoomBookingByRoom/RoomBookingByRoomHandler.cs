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

namespace tas.Application.Features.EmployeeStatusFeature.RoomBookingByRoom
{

    public sealed class RoomBookingByRoomHandler : IRequestHandler<RoomBookingByRoomRequest, RoomBookingByRoomResponse>
    {
        private readonly IEmployeeStatusRepository _EmployeeStatusRepository;
        private readonly IMapper _mapper;

        public RoomBookingByRoomHandler(IEmployeeStatusRepository EmployeeStatusRepository, IMapper mapper)
        {
            _EmployeeStatusRepository = EmployeeStatusRepository;
            _mapper = mapper;
        }

        public async Task<RoomBookingByRoomResponse> Handle(RoomBookingByRoomRequest request, CancellationToken cancellationToken)
        {

            var data = await _EmployeeStatusRepository.RoombookingByRoom(request, cancellationToken);
            return data;


        }
    }
}
