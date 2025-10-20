using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.PeopleTypeFeature.CreatePeopleType
{
    public sealed record CreatePeopleTypeRequest(string Code, string Description) : IRequest;
}
