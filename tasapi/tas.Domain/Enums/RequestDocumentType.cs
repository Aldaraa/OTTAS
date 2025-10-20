using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Domain.Enums
{
    public static class RequestDocumentType 
    {
        public static readonly string NonSiteTravel = "Non Site Travel";
        public static readonly string ProfileChanges = "Profile Changes";
        public static readonly string SiteTravel = "Site Travel";
        public static readonly string DeMobilisation = "De Mobilisation";

        public static readonly string ExternalTravel = "ExternalTravel";

    }

    public static class RequestDocumentFavorTime
    {
        public static readonly string EarlyMorning = "Early morning";
        public static readonly string Morning = "Morning";
        public static readonly string Lunch = "Lunch";
        public static readonly string Afternoon = "Afternoon";
        public static readonly string Evening = "Evening";
        public static readonly string LateNight = "Late night";

    }
}
