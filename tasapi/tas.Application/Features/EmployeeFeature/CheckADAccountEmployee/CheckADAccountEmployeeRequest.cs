using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.CheckADAccountEmployee
{
    public sealed record CheckADAccountEmployeeRequest(
        string AdAccount,
        int EmployeeId
        ) : IRequest<CheckADAccountEmployeeResponse>;
}

