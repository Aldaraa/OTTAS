using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using tas.Application.Features.RequestDocumentFeature.CheckRequestDocumentSiteTravelAdd;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentFeature.CheckRequestDocumentSiteTravelAdd
{

    public sealed record CheckRequestDocumentSiteTravelAddRequest(
        int inScheduleId,
        int outScheduleId,
        int EmployeeId


        ) : IRequest<List<CheckRequestDocumentSiteTravelAddResponse>>;

 

}
