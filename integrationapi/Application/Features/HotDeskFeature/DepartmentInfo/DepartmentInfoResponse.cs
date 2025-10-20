
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.HotDeskFeature.DepartmentInfo
{
    [Keyless]
    public sealed record DepartmentInfoResponse
    {

        public int DepartmentId { get; set; }

        public string Name { get; set; }

        public int? ParentDepartmentId { get; set; }


    }


}
