using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.GetAllEmployee;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.EmployerFeature.GetAllEmployer
{

    public sealed class RoomBookingEmployeeHandler : IRequestHandler<GetAllEmployerRequest, List<GetAllEmployerResponse>>
    {
        private readonly IEmployerRepository _EmployerRepository;
        private readonly IMapper _mapper;

        public RoomBookingEmployeeHandler(IEmployerRepository EmployerRepository, IMapper mapper)
        {
            _EmployerRepository = EmployerRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllEmployerResponse>> Handle(GetAllEmployerRequest request, CancellationToken cancellationToken)
        {

            var users = await _EmployerRepository.GetAllData(request, cancellationToken);
            return _mapper.Map<List<GetAllEmployerResponse>>(users);

            //if (request.status.HasValue)
            //{
            //    var Employers = await _EmployerRepository.GetAllActiveFilter((int)request.status, cancellationToken);
            //    return _mapper.Map<List<GetAllEmployerResponse>>(Employers);
            //}
            //else {

            //}

        }
    }
}
