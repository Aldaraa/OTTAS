using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;

namespace tas.Application.Features.PeopleTypeFeature.GetAllPeopleType
{
    public sealed record GetAllPeopleTypeRequest(int? status) : IRequest<List<GetAllPeopleTypeResponse>>;
}
