using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.GroupMasterFeature.GetAllGroupMaster
{
    public sealed record GetAllGroupMasterResponse
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public int Active { get; set; }
        public int? OrderBy { get; set; }

        public int? DetailCount { get; set; }
        public int? ShowOnProfile { get; set; }

        public int? CreateLog { get; set; }

        public int? Required { get; set; }


        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}
