using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployerFeature.CreateEmployer
{
    public sealed record CreateEmployerRequest(string Code, string Description) : IRequest;
}
