
using tas.Domain.Common;

namespace tas.Application.Features.TransportScheduleFeature.TransportScheduleExport
{


    public sealed record TransportScheduleExportResponse 
    {
        public byte[] ExcelFile { get; set; }
    }



    public sealed record TransportScheduleExportResponseData
    {
        public string? TransportCode { get; set; }
        public int? ScheduleId { get; set; }

        public string? Direction { get; set; }

        public string? Carrier { get; set; }

        public int? Seat { get; set; }
        public int? AvailableSeat { get; set; }


        public string? Description { get; set; }

        public string? Date { get; set; }

        public string? ETD { get; set; }

        public string? ETA { get; set; }

        public int? ActiveTransportModeId { get; set; }

        public string? DirectionData { get; set; }
        public DateTime? EventDate { get; set; }





    }


    





}