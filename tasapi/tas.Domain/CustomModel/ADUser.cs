using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Domain.CustomModel
{
    public class ADUser
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public bool? Active { get; set; }
        public string? UserPrincipalName { get; set; }
        public string? DisplayName { get; set; }
        public string? SamAccountName { get; set; }
        public string? DistinguishedName { get; set; }
        public Guid? Guid { get; set; }
    }
}
