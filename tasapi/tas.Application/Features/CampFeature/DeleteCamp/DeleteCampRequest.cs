using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.CampFeature.DeleteCamp
{
    public sealed record DeleteCampRequest(int Id) : IRequest;
}
