using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CustomModel
{
   public class AuthUser
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

        public int? ReadonlyAccess { get; set; }

        public int? Agreement { get; set; }



        public List<LoginUserMenu> Menu { get; set; }
    }

    public sealed record LoginUserMenu
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Route { get; set; }
        public int? OrderIndex { get; set; }

    }
}
