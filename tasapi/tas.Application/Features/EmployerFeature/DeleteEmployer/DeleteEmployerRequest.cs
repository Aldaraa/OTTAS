using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployerFeature.DeleteEmployer
{
    public sealed record DeleteEmployerRequest(int Id) : IRequest;
}
