using OfficeOpenXml.Drawing.Chart.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.SearchEmployee;

namespace tas.Application.Features.ActiveTransportFeature.ScheduleListActiveTransport
{
    public sealed record ScheduleListActiveTransportResponse
    {
        public int Id { get; set; }
        public string? Code { get; set; }

        public string? Description { get; set; }

        public string DayNum { get; set; }


        public string Direction { get; set; }

        public int Active { get; set; }

        public int CarrierId { get; set; }

        public string? CarrierName { get; set; }

        public int? TransportModeId { get; set; }

        public string? TransportModeName { get; set; }

        public int? TransportAudit { get; set; }

        public int? Seats { get; set; }

        public int? fromLocationId { get; set; }

        public string? fromLocationName { get; set; }

        public string? fromLocationCode { get; set; }

        public int? toLocationId { get; set; }

        public string? toLocationName { get; set; }
        public string? toLocationCode { get; set; }

        public int? Special { get; set; }

        public int? FrequencyWeeks { get; set; }

        public string? ETD { get; set; }
        public string? ETA { get; set; }

        public List<Schedules> schedules {get; set;}


    }

    public sealed record Schedules
{
        public int Id { get; set; }
        public string? Code { get; set; }

        public string? Description { get; set; }    

        public int? Seats { get; set; }

        public DateTime? EventDate { get; set; }

        public string? ETA { get; set; }

        public string? ETD { get; set; }



        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public int? Bookings { get; set; }

        public int? ActiveTransportId { get; set; }

        public string? RealETD { get; set; }

        public bool BusstopStatus { get; set; }

        public string? Remark { get; set; }




    }



}
