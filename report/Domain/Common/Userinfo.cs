using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public class Userinfo
    {
         public int RoleId { get; set; }

         public int? EmployeeId { get; set; }

        public string? Email { get; set; }

        public string? RoleTag { get; set; }

    }
}
