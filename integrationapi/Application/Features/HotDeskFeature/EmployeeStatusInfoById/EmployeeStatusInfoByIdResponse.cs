
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.HotDeskFeature.EmployeeStatusInfoById
{
    [Keyless]
    public sealed record EmployeeStatusInfoByIdResponse
    {
        public int? UserId { get; set; }

        public DateTime? EventDate { get; set; }

        public string? ShiftCode { get; set; }

        public int? Onsite { get; set; }

        public string? ShiftColor { get; set; }


    }


}
