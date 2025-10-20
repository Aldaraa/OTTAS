using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysTeamFeature.GetAllSysTeam
{
    public sealed record GetAllSysTeamResponse
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}
