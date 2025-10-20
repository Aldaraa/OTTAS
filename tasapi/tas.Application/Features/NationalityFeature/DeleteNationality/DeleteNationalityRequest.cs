using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.NationalityFeature.DeleteNationality
{
    public sealed record DeleteNationalityRequest(int Id) : IRequest;
}
