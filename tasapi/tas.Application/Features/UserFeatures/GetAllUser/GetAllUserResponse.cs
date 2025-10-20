using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.UserFeatures.GetAllUser
{
    public sealed record GetAllUserResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
