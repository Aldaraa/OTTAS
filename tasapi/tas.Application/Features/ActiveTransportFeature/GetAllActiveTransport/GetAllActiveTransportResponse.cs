using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.ActiveTransportFeature.GetAllActiveTransport
{

    public sealed record GetAllActiveTransportResponse : BasePaginationResponse<GetAllActiveTransportResult>
    {

    }


    public  record GetAllActiveTransportResult
    {
        public int Id { get; set; }
        public string? Code { get; set; }

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

        public int? CostCodeId { get; set; }

        public string? Description { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public DateTime? ScheduleStartDate { get; set; }

        public DateTime? ScheduleEndDate { get; set; }

        public string? ETA { get; set; }

        public string? ETD { get; set; }

        public string? AircraftCode { get; set; }


    }


}
