using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestGroupEmployeeFeature.GetRequestGroupInpersonateUsers
{
    public sealed record GetRequestGroupInpersonateUsersResponse
    {
        public int Id { get; set; }

        public int? employeeId { get; set; }

        public string? fullName { get; set; }

        public string? displayName { get; set; }

        public string? email { get; set; }

        public string? groupName { get; set; }

        public string? ADAccount { get; set; }


        public int? SAPID { get; set; }

    }

   



}
