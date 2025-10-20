using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public class RequestNonSiteTicketConfig : BaseEntity
    {
        public string? Code { get; set; }
        public string? Description { get; set; }

        public string? FirstClass { get; set; }

        public string? BusinessClass { get; set;}

        public string? PremiumEconomyClass { get; set; }    

        public string? EconomyClass  { get; set; }

        public string? TimeZone { get; set; }

    }
}
