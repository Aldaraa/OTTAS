using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.NationalityFeature.CreateNationality
{
    public sealed record CreateNationalityRequest(string Code, string Description) : IRequest;
}
