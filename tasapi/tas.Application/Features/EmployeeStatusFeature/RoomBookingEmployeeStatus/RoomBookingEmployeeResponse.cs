using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeStatusFeature.RoomBookingEmployee
{
    public sealed record RoomBookingEmployeeResponse
    {
        public int Id { get; set; }
        public string? Camp { get; set; }

        public string? RoomType { get; set; }
        public string? RoomNumber { get; set; }

        public int? VirtualRoom { get; set; }

        public int? RoomId { get; set; }

        public DateTime? DateIn { get; set; }

        public DateTime? LastNight { get; set; }

        public int? Days { get; set; }


        public bool? RoomOwner { get; set; }

    }
}
