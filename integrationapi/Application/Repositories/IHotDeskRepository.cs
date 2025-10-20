
using Application.Features.HotDeskFeature.DepartmentInfo;
using Application.Features.HotDeskFeature.DepartmentSend;
using Application.Features.HotDeskFeature.EmployeeInfo;
using Application.Features.HotDeskFeature.EmployeeInfoById;
using Application.Features.HotDeskFeature.EmployeeSend;
using Application.Features.HotDeskFeature.EmployeeStatusInfo;
using Application.Features.HotDeskFeature.EmployeeStatusInfoById;
using Application.Features.HotDeskFeature.EmployeeStatusSend;
using Application.Features.OtinfoFeature.ManualSent;
using Application.Features.TransportFeature.TransportInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IHotDeskRepository
    {

        Task EmployeeSendData(EmployeeSendRequest request, CancellationToken cancellationToken);

        Task EmployeeStatusSendData(EmployeeStatusSendRequest request, CancellationToken cancellationToken);

        Task DepartmentSendData(DepartmentSendRequest request, CancellationToken cancellationToken);

        Task<List<EmployeeInfoResponse>> EmployeeInfo();

        Task<EmployeeInfoByIdResponse> EmployeeInfoById(EmployeeInfoByIdRequest request, CancellationToken cancellationToken);


        Task<List<EmployeeStatusInfoByIdResponse>> EmployeeStatusInfoById(EmployeeStatusInfoByIdRequest request, CancellationToken cancellationToken);


        Task<List<EmployeeStatusInfoResponse>> EmployeeStatusInfo();


        Task<List<DepartmentInfoResponse>> DepartmentInfo();
    }
}
