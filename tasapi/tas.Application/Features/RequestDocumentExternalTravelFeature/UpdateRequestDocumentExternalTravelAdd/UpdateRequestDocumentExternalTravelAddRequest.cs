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
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.UpdateRequestDocumentExternalTravelAdd
{

    public sealed record UpdateRequestDocumentExternalTravelAddRequest(
        int Id,
        int FirstScheduleId,
        int? LastScheduleId,
        int departmentId,
        int employerId,
        int positionId,
        int costcodeId,
        string? comment 

        ) : IRequest<Unit>;




}
