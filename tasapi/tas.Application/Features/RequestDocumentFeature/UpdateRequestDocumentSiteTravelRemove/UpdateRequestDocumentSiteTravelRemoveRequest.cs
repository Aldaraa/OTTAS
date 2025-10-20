using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentSiteTravelRemove
{

    public sealed record UpdateRequestDocumentSiteTravelRemoveRequest(
        int Id,
        int FirstScheduleId,
        int LastScheduleId,
        int shiftId,
        int? RoomId,
        int? CostCodeId,
        string? comment,
        int? FirstScheduleNoShow,
        int? LastScheduleNoShow

        ) : IRequest;




}
