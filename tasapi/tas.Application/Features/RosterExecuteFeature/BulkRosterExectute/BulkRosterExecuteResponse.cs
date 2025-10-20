using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RosterExecuteFeature.BulkRosterExectute
{



   public sealed class BulkRosterExecuteResponse
    {
        public List<BulkRosterExecutedEmployees> RosterExecutedEmployees { get; set; }
        public List<BulkRosterSkippedEmployees> RosterSkippedEmployees { get; set; }

    }

    public sealed class BulkRosterExecutedEmployees
    {
        public int EmployeeId { get; set; }
        public string? FullName { get; set; }

        public DateTime? EventDate { get; set; }

        public DateTime? EventDateTime { get; set; }

        public string? Direction { get; set; }
        public string? TransportMode { get; set; }

        public string? TransportCode { get; set; }

        public string? Description { get; set; }

        public string? ShiftCode { get; set; }

        public string? ShiftColorCode { get; set; }
        public int? Seats { get; set; }


        public int? Confirmed { get; set; }

        public int? OverBooked { get; set; }

        public int? ScheduleId { get; set; }



    }

    public sealed class BulkRosterSkippedEmployees
    {
        public int EmployeeId { get; set; }
        public string? FullName { get; set; }

        public DateTime? EventDate { get; set; }

        public string Status { get; set; }

        public DateTime? InTransportDate { get; set; }


        public DateTime? OutTransportDate { get; set; }

    }



}
