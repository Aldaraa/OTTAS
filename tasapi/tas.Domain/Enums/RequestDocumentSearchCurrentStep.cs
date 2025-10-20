using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Domain.Enums
{
    public static class RequestDocumentSearchCurrentStep
    {
        public static readonly string Pending = "Pending";
        public static readonly string Cancelled = "Cancelled";
        public static readonly string Completed = "Completed";
        public static readonly string Declined = "Declined";

    }
}
