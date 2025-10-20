using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CustomModel
{
    public class SessionInfo
    {
        
        public int KillId { get; set; }
        public int SessionId { get; set; }
        public string? SessionName { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? Error { get; set; }
    }
}
