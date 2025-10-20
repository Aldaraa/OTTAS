using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Domain.Enums
{
    public static class RequestDocumentAction
    {
        public static readonly string Created = "Created";
        public static readonly string Draft = "Draft";
        public static readonly string Saved = "Saved";
        public static readonly string Approved = "Approved";
        public static readonly string Cancelled = "Cancelled";
        public static readonly string Declined = "Declined";
        public static readonly string Submitted = "Submitted";
        public static readonly string Completed = "Completed";

        public static readonly string WaitingAgent = "Waiting Agent";



    }
}
