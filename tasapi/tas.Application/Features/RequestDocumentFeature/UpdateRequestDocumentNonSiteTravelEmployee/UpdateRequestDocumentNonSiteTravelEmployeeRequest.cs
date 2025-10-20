using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentNonSiteTravelEmployee
{

    public sealed record UpdateRequestDocumentNonSiteTravelEmployeeRequest(
        string? EmergencyContactName,
        string? EmergencyContactMobile,
        DateTime PassportExpiry,
        string PassportName,
        string PassportNumber,
        int EmployeeId,
        int DocumentId,
        int? FrequentFlyer,
        string? Email
        ) : IRequest;
}
