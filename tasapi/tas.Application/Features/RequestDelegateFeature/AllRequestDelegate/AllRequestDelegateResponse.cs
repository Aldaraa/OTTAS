using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.RequestDelegateFeature.AllRequestDelegate
{
    public sealed record AllRequestDelegateResponse 
    {
        public int Id { get; set; }
        public string? fromEmployeeFullname { get; set; }

        public int? fromEmployeeId { get; set; }

        public string? toEmployeeFullname { get; set; }


        public int? toEmployeeId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

    }


}
