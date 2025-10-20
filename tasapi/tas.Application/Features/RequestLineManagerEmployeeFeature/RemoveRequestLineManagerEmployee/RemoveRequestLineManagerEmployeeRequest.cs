using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestLineManagerEmployeeFeature.RemoveRequestLineManagerEmployee
{
    public sealed record RemoveRequestLineManagerEmployeeRequest(
            int Id

        ) : IRequest;
}
