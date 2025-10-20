using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.MultipleBookingFeature.MultipleBookingAddTransport
{
    public sealed record MultipleBookingAddTransportRequest(
       int firsScheduleId,
       int? lastSheduleId,
       List<MultipleBookingAddTransportEmployee> EmployeeData


        ) : IRequest<List<MultipleBookingAddTransportResponse>>;

    public sealed record MultipleBookingAddTransportEmployee
    { 
     public   int EmployeeId {  get; set; }
       public int DepartmentId { get; set; }
      public  int PositionId { get; set; }
      public  int EmployerId { get; set; }
       public int CostCodeId { get; set; }

       public int? firsScheduleGoShow { get; set; }
       public int? lastSheduleGoShow { get; set; }

        public int? ShiftId { get; set; }


    }
}
