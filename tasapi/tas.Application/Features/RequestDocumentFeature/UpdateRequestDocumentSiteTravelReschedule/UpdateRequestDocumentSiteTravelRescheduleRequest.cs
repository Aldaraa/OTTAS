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

namespace tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentSiteTravelReschedule
{

    public sealed record UpdateRequestDocumentSiteTravelRescheduleRequest(
        int Id,
        int ReScheduleId,
        int ExistingScheduleId,
        int shiftId,
        int? RoomId,
        string? comment,
        int? ExistingScheduleIdGoShow,
        int? ExistingScheduleIdNoShow,
        int? ReScheduleGoShow,
        int? ReScheduleNoShow

        ) : IRequest;




}
