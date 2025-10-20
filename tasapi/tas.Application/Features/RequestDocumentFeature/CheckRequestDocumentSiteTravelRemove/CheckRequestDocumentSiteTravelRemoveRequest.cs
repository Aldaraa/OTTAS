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
using tas.Application.Features.RequestDocumentFeature.CheckRequestDocumentSiteTravelRemove;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentFeature.CheckRequestDocumentSiteTravelRemove
{

    public sealed record CheckRequestDocumentSiteTravelRemoveRequest(
        int FirstScheduleId,
        int LastScheduleId,
        int EmployeeId


        ) : IRequest<List<CheckRequestDocumentSiteTravelRemoveResponse>>;

 

}
