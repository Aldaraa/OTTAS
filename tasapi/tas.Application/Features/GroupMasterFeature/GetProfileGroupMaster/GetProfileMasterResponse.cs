using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.GroupMasterFeature.GetProfileGroupMaster
{
    public sealed record GetProfileGroupMasterResponse
    {
        public int Id { get; set; }
        public string? Description { get; set; }

        public int? CreateLog { get; set; }

        public int? Required { get; set; }

        public  List<GetProfileGroupMasterDetail>? details { get; set; }

    }


    public sealed record GetProfileGroupMasterDetail
    { 
        public int Id { get; set; }  

        public string? Description { get; set; }

        public string? Code { get; set; }
    }
}
