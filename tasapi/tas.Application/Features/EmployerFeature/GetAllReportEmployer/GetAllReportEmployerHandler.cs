using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.GetAllEmployee;
using tas.Application.Features.EmployerFeature.GetAllReportEmployer;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.EmployerFeature.GetAllReportEmployer
{

    public sealed class RoomBookingEmployeeHandler : IRequestHandler<GetAllReportEmployerRequest, List<GetAllReportEmployerResponse>>
    {
        private readonly IEmployerRepository _EmployerRepository;
        private readonly IMapper _mapper;

        public RoomBookingEmployeeHandler(IEmployerRepository EmployerRepository, IMapper mapper)
        {
            _EmployerRepository = EmployerRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllReportEmployerResponse>> Handle(GetAllReportEmployerRequest request, CancellationToken cancellationToken)
        {

            var data = await _EmployerRepository.GetAllReportData(request, cancellationToken);
            return data;

        }
    }
}
