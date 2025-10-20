using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SafeModeEmployeeStatusFeature.GetEmployeeStatus
{
    public sealed class GetEmployeeStatusResponse
    {
        public int Id { get; set; }
        public DateTime? EventDate { get; set; }
        public int? ShiftId { get; set; }
        public int? RoomId { get; set; }

        public string? RoomNumber { get; set; }


        public int? EmployeeId { get; set; }

        public int? CostCodeId { get; set; }
        public int? DepartmentId { get; set; }
        public int? EmployerId { get; set; }

        public int? PositionId { get; set; }

        




    }
}
