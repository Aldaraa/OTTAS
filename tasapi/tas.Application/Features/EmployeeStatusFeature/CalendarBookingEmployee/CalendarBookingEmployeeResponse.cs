using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeStatusFeature.CalendarBookingEmployee
{
    public sealed record CalendarBookingEmployeeResponse
    {
        public int Id { get; set; }

        public DateTime EventDate { get; set; }

        public string? CampName { get; set; }

        public string? RoomTypeName { get; set; }
        public string? RoomNumber { get; set; }

        public int? VirtualRoom { get; set; }

        public int? RoomId { get; set; }

    }
}
