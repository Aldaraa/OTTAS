using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.PeopleTypeFeature.UpdatePeopleType
{
    public sealed record UpdatePeopleTypeRequest(int Id, string Code, string Description, int Active) : IRequest;
}
