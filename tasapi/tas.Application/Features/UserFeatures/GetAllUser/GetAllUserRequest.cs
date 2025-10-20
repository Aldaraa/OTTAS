using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.UserFeatures.GetAllUser
{
    public sealed record GetAllUserRequest : IRequest<List<GetAllUserResponse>>;
}
