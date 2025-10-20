using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.SearchReSchedulePeople
{

    public sealed record SearchReSchedulePeopleResponse
    { 
        public  List<SearchReSchedulePeople> Peoples { get; set; }

        public List<SearchReScheduleToSchedule> toSchdules { get; set; }

        public List<SearchReScheduleTOShift> ShifData { get; set; }
    } 
    public sealed record SearchReSchedulePeople
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public string? FullName { get; set; }

        public string? Department { get; set; }

        public string? CostCodeDescr { get; set; }


        public  DateTime? DateCreated { get; set; }

        public string? TransportCode { get; set; }

        public string? Position { get; set; }

        public string? EmployerName { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? ShiftCode { get; set; }

        public string? ShiftCodeColor { get; set; }




    }

    public sealed record SearchReScheduleToSchedule 
    {
        public int? ScheduleId { get; set; }

        public string? ScheduleDescription { get; set; }

        public int? ActiveTransportId { get; set; }

    }

    public sealed record SearchReScheduleTOShift
    { 
        public int? Id { get; set; }

        public string? Code { get; set; }

        public string? Description { get; set; }

        public int? OnSite { get; set; }

    }



}
