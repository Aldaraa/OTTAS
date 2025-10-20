using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ActiveTransportFeature.GetAllActiveTransport;
using tas.Domain.Common;

namespace tas.Application.Features.EmployeeFeature.RosterExecuteEmployee
{
    public sealed record RosterExecuteEmployeeResponse 
    {
        public int EmployeeId {  get; set; }
        public string? FirstName {  get; set; }
        public string? LastName {  get; set; }

        public DateTime? EventDate {  get; set; }

        public DateTime? EventDateTime { get; set; }

        public string? Direction {  get; set; }
        public string? TransportMode {  get; set; }

        public string? TransportCode { get; set; }


        public string? Description { get; set; }

        public string? ShiftCode { get; set; }

        public string? ShiftColorCode { get; set; }

        public int? Seats { get; set; }


        public int? Confirmed { get; set; }

        public int? OverBooked { get; set; }

        public int? ScheduleId { get; set; }






    }






}
