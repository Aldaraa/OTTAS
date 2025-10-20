using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Domain.CustomModel
{
    public class TokenUserData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Role { get; set; }

        public int? RoleId { get; set; }

        public string? UserName { get; set; }
    }
}
