using Application.Features.OtinfoFeature.CheckTransport;
using Application.Features.OtinfoFeature.JobInfo;
using Application.Features.OtinfoFeature.ManualSent;
using Application.Features.TransportFeature.TransportInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{

    public interface ITransportInfoRepository
    {
        Task<List<TransportInfoResponse>> EmployeeTransportInfo();


        Task SendTransportData(CancellationToken cancellationToken);

        Task ManualSentData(ManualSentRequest request,  CancellationToken cancellationToken);

        Task<string> CheckTransport(CheckTransportRequest request,  CancellationToken cancellationToken);


        Task<List<JobInfoResponse>> JobInfo(JobInfoRequest request,  CancellationToken cancellationToken);



        Task LoadData();


       


    }

}
