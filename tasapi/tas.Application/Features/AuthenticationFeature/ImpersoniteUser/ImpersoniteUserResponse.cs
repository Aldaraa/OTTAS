using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.AuthenticationFeature.ImpersoniteUser
{
    public sealed record ImpersoniteUserResponse
    {

        public int Id { get; set; }

        public string? Lastname { get; set; }

        public string? Firstname { get; set; }

        public string? Email { get; set; }

        public string? Mobile { get; set; }

        public int? SAPID { get; set; }

        public string? NRN { get; set; }

        public string? ADAccount { get; set; }

        public string? Role { get; set; }

        public int? RoleId { get; set; }

    }


}
