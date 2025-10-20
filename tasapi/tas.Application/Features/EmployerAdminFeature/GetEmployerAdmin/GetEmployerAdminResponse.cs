using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployerAdminFeature.GetEmployerAdmin
{
    public sealed record GetEmployerAdminResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public int? EmployerId { get; set; }

        public DateTime? DateCreated { get; set; }
    }

 
}