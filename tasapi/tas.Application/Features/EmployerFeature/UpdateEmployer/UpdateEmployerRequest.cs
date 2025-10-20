using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployerFeature.UpdateEmployer
{
    public sealed record UpdateEmployerRequest(int Id, string Code, string Description) : IRequest;
}
