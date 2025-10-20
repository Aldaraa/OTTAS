using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RosterExecuteFeature.BulkRosterExectutePreview
{
    public sealed record BulkRosterExecutePreviewResponse
    {
        public int EmpId { get; set; }
        public string Fullname { get; set; }

       public List<BulkRosterExecutePreviewResponseOffSiteStatus> OffSiteStatus { get; set; }
      ////  public List<BulkRosterExecutePreviewResponseOnSiteStatus> OnSiteStatus { get; set; }

      //  public List<BulkRosterExecutePreviewResponseEmployeeRoomData> EmployeeRoomData { get; set; }

        public BulkRosterExecutePreviewResponseOnsiteData OnsiteData { get; set; }



        public bool EmpOnSiteStatus { get; set; } = true;

        public bool EmpOffSiteStatus { get; set; } = true;

        public bool EmployeeRoomDataStatus { get; set; } = true;

    }


    public sealed record BulkRosterExecutePreviewResponseOffSiteStatus
    {
       public DateTime EventDate { get; set; }

        public int? ShiftId { get; set; }

        public string? ShiftCode { get; set; }


    }


    public sealed record BulkRosterExecutePreviewResponseOnSiteStatus
    {
        public DateTime EventDate { get; set; }

        public string? Direction { get; set; }



    }

    public sealed record BulkRosterExecutePreviewResponseEmployeeRoomData
    {

        public int EmpId { get; set; }
        public string Fullname { get; set; }

        public DateTime EventDate { get; set; }


    }


    public sealed record BulkRosterExecutePreviewResponseOnsiteData
    {

        public DateTime? EventDate { get; set; }

        public string Status { get; set; }

        public DateTime? InTransportDate { get; set; }


        public DateTime? OutTransportDate { get; set; }



        public string? INActiveTransportCode { get; set; }

        public string? INScheduleDescription { get; set; }



        public string? OUTActiveTransportCode { get; set; }

        public string? OUTScheduleDescription { get; set; }

        public bool? AllowDelete { get; set; }





    }


}
