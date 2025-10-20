using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.GetEmployee;

namespace tas.Application.Features.EmployeeFeature.RosterExecutePreviewEmployee
{
    public sealed record RosterExecutePreviewEmployeeResponse
    {
        public List<EmployeeStatusDate> newStatusDates { get; set; }

        public List<EmployeeStatusDate> oldStatusDates { get; set; }

        public List<RosterExecutePreviewEmployeeStatus> EmployeeOffSiteStatus { get; set; }


        public OnsiteData OnsiteData { get; set; }


    }


    public sealed record RosterExecutePreviewEmployeeStatus
    {
        public DateTime EventDate { get; set; }

        public int ShiftId { get; set; }

        public string ShiftCode { get; set; }

        public string ShiftName { get; set; }

    }

    public sealed record OnsiteData
    { 

        public DateTime? EventDate { get; set; }

        public string Status { get; set; }

        public DateTime? InTransportDate { get; set; }


        public DateTime? OutTransportDate { get; set; }




    }



}
