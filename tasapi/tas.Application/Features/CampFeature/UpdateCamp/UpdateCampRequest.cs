using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.CampFeature.UpdateCamp
{
    public sealed record UpdateCampRequest(int Id, string Code, string Description, int onSite, int Active) : IRequest;
}
