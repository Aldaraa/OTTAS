using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysTeamFeature.GetSysTeam
{
    public sealed record GetSysTeamResponse
    {
        public int Id { get; set; }

        public string? Name { get; set; }


        public List<TeamMenu> TeamMenus {get; set;}

        public List<TeamUser> TeamUsers { get; set; }

    }

    public sealed record TeamMenu
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Route { get; set; }

        public string? ApplicationName { get; set; }

        public int? Permission { get; set; }


    }

    public sealed record TeamUser
    {
        public int Id { get; set; }

        public string? Firstname { get; set; }

        public string? lastname { get; set; }

        public string? DepartmentName { get; set; }

        public string? NRN { get; set; }


    }



}
