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

namespace tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentSiteTravelAdd
{

    public sealed record UpdateRequestDocumentSiteTravelAddRequest(
        int Id,
        int inScheduleId,
        int outScheduleId,
        int departmentId,
        int shiftId,
        int employerId,
        int positionId,
        int costcodeId,
        int? roomId,
        string? comment,
        bool? CheckRoom,
        int? inScheduleGoShow,
        int? outScheduleGoShow

        ) : IRequest<Unit>;




}
