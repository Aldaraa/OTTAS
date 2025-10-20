using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;
using tas.Domain.Enums;

namespace tas.Persistence.Repositories
{
    public partial class RequestDocumentRepository
    {

        public async Task GenerateUpdateDocumentInfo(int documentId, CancellationToken cancellationToken)
        {
            var document = await Context.RequestDocument
            .Where(x => x.Id == documentId)
            .FirstOrDefaultAsync(cancellationToken);

                    if (document == null) return;

            await UpdateDocumentInfo(document, cancellationToken);
            await SaveChangesAsync(document);
        }

        public async Task GenerateDescription(int documentId, CancellationToken cancellationToken)
        {
            var document = await Context.RequestDocument
                .Where(x => x.Id == documentId)
                .FirstOrDefaultAsync(cancellationToken);

            if (document == null) return;
            if (string.IsNullOrWhiteSpace(document.DocumentType) || document.DocumentType == null)
            {
                // Handle the case where DocumentType is null if needed
                document.Description = "Document type is not specified.";
                document.MailDescription = "Document type is not specified.";
                await SaveChangesAsync(document);
                return;
            }

            if (document.DocumentType == RequestDocumentType.SiteTravel)
            {
                await HandleSiteTravel(document, cancellationToken);
            }
            else if (document.DocumentType == RequestDocumentType.NonSiteTravel)
            {
                await HandleNonSiteTravel(document, cancellationToken);
            }
            else if (document.DocumentType == RequestDocumentType.ProfileChanges)
            {
                await HandleProfileChanges(document, cancellationToken);
            }
            else if (document.DocumentType == RequestDocumentType.DeMobilisation)
            {
                await HandleDeMobilisation(document, cancellationToken);
            }
            else {
                document.Description = "Unknown document type.";
                document.MailDescription = "Unknown document type.";

            }


            await SaveChangesAsync(document);
        }

        private async Task UpdateDocumentInfo(RequestDocument document, CancellationToken cancellationToken)
        {
                var lastUpdateData = await Context.RequestDocumentHistory
                .Where(x => x.DocumentId == document.Id)
                .OrderByDescending(x => x.DateCreated)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastUpdateData != null)
            {
                var currentEmployee = await Context.Employee.AsNoTracking()
                    .Where(x => x.Id == lastUpdateData.ActionEmployeeId)
                    .Select(x => new { x.Lastname, x.Firstname })
                    .FirstOrDefaultAsync(cancellationToken);



                if (currentEmployee != null)
                {
                    document.UpdatedInfo = $"By {currentEmployee.Firstname} {currentEmployee.Lastname} {lastUpdateData.DateCreated}";
                }
            }
            


        }

        private async Task HandleSiteTravel(RequestDocument document, CancellationToken cancellationToken)
        {
            if (document.DocumentTag == "ADD")
            {
                await HandleSiteTravelAdd(document, cancellationToken);
            }
            else if (document.DocumentTag == "REMOVE")
            {
                await HandleSiteTravelRemove(document, cancellationToken);
            }
            else if (document.DocumentTag == "RESCHEDULE")
            {
                await HandleSiteTravelReschedule(document, cancellationToken);
            }
        }

        private async Task HandleSiteTravelAdd(RequestDocument document, CancellationToken cancellationToken)
        {
            var currentData = await Context.RequestSiteTravelAdd.AsNoTracking()
                .Where(x => x.DocumentId == document.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentData == null)
            {
                document.Description = "Booking";
                document.MailDescription = "Booking";

                return;
            }

            var currentSchedule = await Context.TransportSchedule.AsNoTracking()
                .Where(x => x.Id == currentData.inScheduleId)
                .FirstOrDefaultAsync(cancellationToken);

            var outSchedule = await Context.TransportSchedule.AsNoTracking()
                .Where(x => x.Id == currentData.outScheduleId)
                .FirstOrDefaultAsync(cancellationToken);
            if (currentSchedule == null || outSchedule == null)
            {
                document.Description = "Booking";
                document.MailDescription = "Booking";


            }

            var currentTransport = await GetActiveTransportDetails(currentSchedule?.ActiveTransportId, cancellationToken);
            var outTransport = await GetActiveTransportDetails(outSchedule?.ActiveTransportId, cancellationToken);

            if (currentTransport != null && outTransport != null)
            {
                document.Description = $"Booking, {currentTransport?.fromLocationCode} {currentTransport?.Direction} /{outTransport?.fromLocationCode} {outTransport?.Direction} \n{currentData.Reason} \n";

                //document.MailDescription = $"Booking <br> {currentData.Reason}<br> {currentSchedule?.EventDate.ToString("yyyy-MM-dd")} " +
                 //   $"{currentTransport?.fromLocationCode} {currentTransport?.Direction} {currentSchedule?.Description}<br> {outSchedule?.EventDate.ToString("yyyy-MM-dd")} {outTransport?.fromLocationCode} {outTransport?.Direction} {outSchedule?.Description} <br>";
                if (currentSchedule?.EventDate > outSchedule?.EventDate)
                {
                    document.DaysAwayDate = outSchedule?.EventDate;
                    document.MailDescription = $"Booking <br> {currentData.Reason}<br> " +
                         $"{outSchedule?.EventDate.ToString("yyyy-MM-dd")} {outTransport?.fromLocationCode} {outTransport?.Direction} {outSchedule?.Description} <br>{currentSchedule?.EventDate.ToString("yyyy-MM-dd")}  {currentTransport?.fromLocationCode} {currentTransport?.Direction} {currentSchedule?.Description} <br>";
                }
                else
                {
                    document.DaysAwayDate = currentSchedule?.EventDate;
                    document.MailDescription = $"Booking <br> {currentData.Reason}<br> {currentSchedule?.EventDate.ToString("yyyy-MM-dd")} " +
    $"{currentTransport?.fromLocationCode} {currentTransport?.Direction} {currentSchedule?.Description}<br> {outSchedule?.EventDate.ToString("yyyy-MM-dd")} {outTransport?.fromLocationCode} {outTransport?.Direction} {outSchedule?.Description} <br>";
                }

            }
            else if (currentTransport != null)
            {
                document.Description = $"Booking, {currentTransport.fromLocationCode} {currentTransport.Direction}/NA \n{currentData.Reason} \n";
                document.MailDescription = $"Booking <br> {currentData.Reason} <br> {currentTransport.fromLocationCode} {currentTransport.Direction}/NA <br>";

                if (currentSchedule?.EventDate > outSchedule?.EventDate)
                {
                    document.DaysAwayDate = outSchedule?.EventDate;
                }
                else {
                    document.DaysAwayDate = currentSchedule?.EventDate;
                }

                
            }
            else if (outTransport != null)
            {
                document.Description = $"Booking, NA/{outTransport.fromLocationCode} {outTransport.Direction}  \n{currentData.Reason}";
                document.MailDescription = $"Booking <br> {currentData.Reason} <br> NA/{outTransport.fromLocationCode} {outTransport.Direction}";

                document.DaysAwayDate = outSchedule?.EventDate;
            }
            else
            {
                document.Description = $"Booking, {currentSchedule?.EventDate.ToShortDateString()}";
                document.MailDescription = $"Booking <br> {currentSchedule?.EventDate.ToShortDateString()}";

                document.DaysAwayDate = currentSchedule?.EventDate;
            }
        }

        private async Task HandleSiteTravelRemove(RequestDocument document, CancellationToken cancellationToken)
        {
            var currentData = await Context.RequestSiteTravelRemove.AsNoTracking()
                .Where(x => x.DocumentId == document.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentData == null)
            {
                document.Description = "Remove, NA";
                document.MailDescription = "Remove, NA";

                return;
            }

            var currentSchedule = await Context.TransportSchedule.AsNoTracking()
                .Where(x => x.Id == currentData.FirstScheduleId)
                .FirstOrDefaultAsync(cancellationToken);


            var lastSchedule = await Context.TransportSchedule.AsNoTracking()
                .Where(x => x.Id == currentData.LastScheduleId)
                .FirstOrDefaultAsync(cancellationToken);


            if (currentSchedule == null)
            {
                document.Description = $"Remove, NA {currentData.Reason}";
                document.MailDescription = $"Remove, <br> NA <br> {currentData.Reason}";

                return;
            }

            var currentTransport = await GetActiveTransportDetails(currentSchedule.ActiveTransportId, cancellationToken);

            var lastTransport = await GetActiveTransportDetails(lastSchedule?.ActiveTransportId, cancellationToken);




            document.Description = $"Remove, {currentTransport?.fromLocationCode} {currentTransport?.Direction} \n{currentData.Reason}";
            document.MailDescription = $"Remove <br> {currentData.Reason} <br>" +
                $" {currentSchedule.EventDate.ToString("yyyy-MM-dd")} {currentTransport?.fromLocationCode} {currentTransport?.Direction} {currentSchedule.Description} <br>" +  
                $" {lastSchedule.EventDate.ToString("yyyy-MM-dd")} {lastTransport?.fromLocationCode} {lastTransport?.Direction} {lastSchedule.Description}";


            document.DaysAwayDate = currentSchedule.EventDate > lastSchedule.EventDate ? lastSchedule.EventDate : currentSchedule.EventDate;
        }

        private async Task HandleSiteTravelReschedule(RequestDocument document, CancellationToken cancellationToken)
        {
            var currentData = await Context.RequestSiteTravelReschedule.AsNoTracking()
                .Where(x => x.DocumentId == document.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentData == null)
            {
                document.Description = "Reschedule N/A";
                return;
            }

            var currentSchedule = await Context.TransportSchedule.AsNoTracking()
                .Where(x => x.Id == currentData.ReScheduleId)
                .FirstOrDefaultAsync(cancellationToken);

            var existingSchedule = await Context.TransportSchedule.AsNoTracking()
                .Where(x => x.Id == currentData.ExistingScheduleId)
                .FirstOrDefaultAsync(cancellationToken);

            var currentTransport = await GetActiveTransportDetails(currentSchedule?.ActiveTransportId, cancellationToken);
            var existingTransport = await GetActiveTransportDetails(existingSchedule?.ActiveTransportId, cancellationToken);

            if (currentTransport != null && existingTransport != null)
            {
                document.Description = $"Reschedule,{currentTransport?.fromLocationCode} {currentTransport?.Direction}/{existingTransport?.fromLocationCode} {existingTransport?.Direction} \n{currentData.Reason} \n";
                document.MailDescription = $"Reschedule <br> {currentData.Reason} <br>" +
                     $"<b>PREVIOUS :</b> {existingSchedule?.EventDate.ToString("yyyy-MM-dd")} {existingTransport?.fromLocationCode} {existingTransport?.Direction} {existingSchedule?.Description} <br>" +
                $"<b>CURRENT :</b> {currentSchedule?.EventDate.ToString("yyyy-MM-dd")} {currentTransport?.fromLocationCode} {currentTransport?.Direction} {currentSchedule?.Description}  ";
                   

                document.DaysAwayDate = existingSchedule?.EventDate > currentSchedule?.EventDate ? currentSchedule.EventDate : existingSchedule?.EventDate;
            }
            else if (currentTransport != null)
            {
                document.Description = $"Reschedule, {currentTransport.fromLocationCode} {currentTransport.Direction} {currentData.Reason} \n";

                document.MailDescription = $"Reschedule <br> {currentData.Reason}" +
                     $"<b>PREVIOUS :</b> None <br>" +
                    $"<b>CURRENT :</b> {currentSchedule?.EventDate.ToString("yyyy-MM-dd")} {currentTransport?.fromLocationCode} {currentTransport?.Direction} {currentSchedule?.Description} <br> ";

                document.DaysAwayDate = currentSchedule?.EventDate;
            }
            else if (existingTransport != null)
            {
                document.Description = $"Reschedule, {existingTransport.Direction} {currentData.Reason} \n{existingSchedule?.EventDate.ToShortDateString()} {existingTransport.fromLocationCode} {existingSchedule?.Description} \n";

                document.MailDescription = $"Reschedule <br>" +
                    $"<b>PREVIOUS :</b> {existingSchedule?.EventDate.ToString("yyyy-MM-dd")} {existingTransport?.fromLocationCode} {existingTransport?.Direction} {existingSchedule?.Description}" +
                    $"<b>CURRENT :</b> None";

                document.DaysAwayDate = currentSchedule?.EventDate;
            }
            else
            {
                document.Description = "Reschedule N/A";
                document.MailDescription = "Reschedule N/A";

            }
        }

        private async Task HandleNonSiteTravel(RequestDocument document, CancellationToken cancellationToken)
        {
            var currentEmployee = await Context.Employee.AsNoTracking()
                .Where(c => c.Id == document.EmployeeId)
                .FirstOrDefaultAsync(cancellationToken);

            var accommodationData = await Context.RequestNonSiteTravelAccommodation.AsNoTracking()
                .Where(x => x.DocumentId == document.Id)
                .FirstOrDefaultAsync(cancellationToken);

            var flightData = await (
                from flight in Context.RequestNonSiteTravelFlight.AsNoTracking().Where(x => x.DocumentId == document.Id)
                join departLocation in Context.RequestAirport.AsNoTracking() on flight.DepartLocationId equals departLocation.Id into departLocationData
                from departLocation in departLocationData.DefaultIfEmpty()
                join arriveLocation in Context.RequestAirport.AsNoTracking() on flight.ArriveLocationId equals arriveLocation.Id into arriveLocationData
                from arriveLocation in arriveLocationData.DefaultIfEmpty()
                select new
                {
                    Id = flight.Id,
                    ArriveLocationCode = arriveLocation.Code,
                    DepartLocationCode = departLocation.Code,
                    TravelDate = flight.TravelDate,
                    DepartLocationName = departLocation.Description,
                }
            ).OrderBy(x=> x.TravelDate).ToListAsync(cancellationToken);

            if (flightData.Count > 0 && accommodationData != null)
            {
                //var flightInfo = string.Join(", ", flightData.Select(f => $"{f.DepartLocationCode} to {f.ArriveLocationCode}"));

                //   var flightDateInfo =  flightData.OrderBy(x => x.TravelDate).Select(x => $"{x.DepartLocationName} - {x.TravelDate?.ToShortDateString()}").FirstOrDefault();

                var flightDateInfo = new
                {
                    TravelStartDate = flightData.FirstOrDefault()?.TravelDate,
                    TravelEndDate = flightData.LastOrDefault()?.TravelDate,
                    FirstLocationCode =$"{flightData.FirstOrDefault()?.ArriveLocationCode}-{flightData.FirstOrDefault()?.DepartLocationCode}",
                    LastlocationCode = $"{flightData.LastOrDefault()?.ArriveLocationCode}-{flightData.LastOrDefault()?.DepartLocationCode}",

                };

                var flightInfo =  $"{flightDateInfo?.FirstLocationCode} to {flightDateInfo?.LastlocationCode}";

                string flightDateInfoString;
                if (!flightData.Any())
                {
                    flightDateInfoString = "N/A";
                }
                else if (flightData.Count == 1)
                {
                    flightDateInfoString = flightDateInfo.TravelStartDate.HasValue
                            ? flightDateInfo.TravelStartDate.Value.ToShortDateString()
                            : "N/A";
                }
                else
                {
                    flightDateInfoString = flightDateInfo.TravelStartDate.HasValue && flightDateInfo.TravelEndDate.HasValue
                        ? $"{flightDateInfo.TravelStartDate.Value.ToShortDateString()} - {flightDateInfo.TravelEndDate.Value.ToShortDateString()}"
                        : "N/A";
                }


                var hotelInfo = $"{accommodationData.Hotel} {accommodationData.City} {accommodationData?.FirstNight?.ToShortDateString()} - {accommodationData?.LastNight?.ToShortDateString()}";
                document.Description = $"{currentEmployee?.Firstname} {currentEmployee?.Lastname} \nFlights: {flightInfo}, {flightDateInfoString} \nHotel: {hotelInfo}";
                document.MailDescription = $"{currentEmployee?.Firstname} {currentEmployee?.Lastname}\nFlights: {flightInfo},  {flightDateInfoString}  \nHotel: {hotelInfo}";
                document.DaysAwayDate = flightData.First().TravelDate ?? accommodationData?.FirstNight;
            }
            else if (flightData.Count == 0 && accommodationData != null)
            {
                var hotelInfo = $"{accommodationData.Hotel} {accommodationData.City} {accommodationData?.FirstNight?.ToShortDateString()} - {accommodationData?.LastNight?.ToShortDateString()}";

                document.Description = $"{currentEmployee?.Firstname} {currentEmployee?.Lastname} \nHotel: {hotelInfo}";
                document.MailDescription = $"{currentEmployee?.Firstname} {currentEmployee?.Lastname} \nHotel: {hotelInfo}";
                document.DaysAwayDate = accommodationData?.FirstNight;
            }
            else if (flightData.Count > 0 && accommodationData == null)
            {
                //  var flightInfo = string.Join(", ", flightData.Select(f => $"{f.DepartLocationCode} to {f.ArriveLocationCode}"));
                //    var flightDateInfo = flightData.OrderBy(x => x.TravelDate).Select(x => $"{x.DepartLocationName} - {x.TravelDate?.ToShortDateString()}").FirstOrDefault();



                var flightDateInfo = new
                {
                    TravelStartDate = flightData.FirstOrDefault()?.TravelDate,
                    TravelEndDate = flightData.LastOrDefault()?.TravelDate,
                    FirstLocationCode = $"{flightData.FirstOrDefault()?.ArriveLocationCode}-{flightData.FirstOrDefault()?.DepartLocationCode}",
                    LastlocationCode = $"{flightData.LastOrDefault()?.ArriveLocationCode}-{flightData.LastOrDefault()?.DepartLocationCode}",

                };

                var flightInfo = $"{flightDateInfo?.FirstLocationCode} to {flightDateInfo?.LastlocationCode}";


                string flightDateInfoString;
                if (!flightData.Any())
                {
                    flightDateInfoString = "N/A";
                }
                else if (flightData.Count == 1)
                {
                    flightDateInfoString = flightDateInfo.TravelStartDate.HasValue
                            ? flightDateInfo.TravelStartDate.Value.ToShortDateString()
                            : "N/A";

                }
                else
                {

                    flightDateInfoString = flightDateInfo.TravelStartDate.HasValue && flightDateInfo.TravelEndDate.HasValue
                        ? $"{flightDateInfo.TravelStartDate.Value.ToShortDateString()} - {flightDateInfo.TravelEndDate.Value.ToShortDateString()}"
                        : "N/A";
                }


                document.Description = $"{currentEmployee?.Firstname} {currentEmployee?.Lastname} \nFlights: {flightInfo}, {flightDateInfoString}";
                document.MailDescription = $"{currentEmployee?.Firstname} {currentEmployee?.Lastname}\nFlights: {flightInfo}, {flightDateInfoString}";

                document.DaysAwayDate = flightData.First().TravelDate;
            }
            else
            {
                document.Description = $"{currentEmployee?.Firstname} {currentEmployee?.Lastname}";
                document.MailDescription = $"{currentEmployee?.Firstname} {currentEmployee?.Lastname}";

            }
        }

        private async Task HandleProfileChanges(RequestDocument document, CancellationToken cancellationToken)
        {
            var currentDataPermanent = await Context.RequestDocumentProfileChangeEmployee.AsNoTracking()
                .Where(x => x.DocumentId == document.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (currentDataPermanent != null)
            {
                document.Description = $"{document.DocumentType} Permanent {currentDataPermanent.Firstname} {currentDataPermanent.Lastname}";

                document.MailDescription = $"Permanent {currentDataPermanent.Firstname} {currentDataPermanent.Lastname}";

                document.DaysAwayDate =  document.DateCreated;
            }
            else
            {

                var currentDataTemp = await Context.RequestDocumentProfileChangeEmployeeTemp.AsNoTracking()
                    .Where(x => x.DocumentId == document.Id)
                    .FirstOrDefaultAsync(cancellationToken);



                if (currentDataTemp == null)
                {
                    var resourceEmployee = await Context.Employee.AsNoTracking().Where(c => c.Id == document.EmployeeId).FirstOrDefaultAsync(cancellationToken);

                    document.Description = $"{document.DocumentType} temporary {resourceEmployee?.Firstname} {resourceEmployee?.Lastname}";
                    document.MailDescription = $"Temporary {resourceEmployee?.Firstname} {resourceEmployee?.Lastname}";
                }
                else {
                    var resourceEmployee = await Context.Employee.AsNoTracking().Where(c => c.Id == document.EmployeeId).FirstOrDefaultAsync(cancellationToken);

                    document.Description = $"{document.DocumentType} temporary {resourceEmployee?.Firstname} {resourceEmployee?.Lastname}";
                    document.MailDescription = $"Temporary {resourceEmployee?.Firstname} {resourceEmployee?.Lastname}";
                }

            }
        }

        private async Task HandleDeMobilisation(RequestDocument document, CancellationToken cancellationToken)
        {
            var currentData = await Context.RequestDeMobilisation.AsNoTracking()
                .Where(x => x.DocumentId == document.Id)
                .FirstOrDefaultAsync(cancellationToken);

            var currentEmployee = await Context.Employee.AsNoTracking()
                .Where(c => c.Id == document.EmployeeId)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentData != null)
            {
                var currentDemobType = await Context.RequestDeMobilisationType.AsNoTracking()
                    .Where(x => x.Id == currentData.RequestDeMobilisationTypeId)
                    .FirstOrDefaultAsync(cancellationToken);

                document.Description = $"{document.DocumentType} {currentEmployee?.Firstname} {currentEmployee?.Lastname}";
                document.MailDescription = $"{document.DocumentType} {currentEmployee?.Firstname} {currentEmployee?.Lastname}";

                document.DaysAwayDate = currentData.CompletionDate ?? document.DateCreated;
            }
            else
            {
                document.Description = $"{document.DocumentType} {currentEmployee?.Firstname} {currentEmployee?.Lastname}";
                document.MailDescription = $"{document.DocumentType} {currentEmployee?.Firstname} {currentEmployee?.Lastname}";

            }
        }



        private async Task<dynamic> GetActiveTransportDetails(int? activeTransportId, CancellationToken cancellationToken)
        {
            if (!activeTransportId.HasValue) return null;

            return await (from at in Context.ActiveTransport.AsNoTracking().Where(x => x.Id == activeTransportId.Value)
                          join fromLocation in Context.Location.AsNoTracking() on at.fromLocationId equals fromLocation.Id into fromLocationData
                          from fromLocation in fromLocationData.DefaultIfEmpty()
                          join toLocation in Context.Location.AsNoTracking() on at.toLocationId equals toLocation.Id into toLocationData
                          from toLocation in toLocationData.DefaultIfEmpty()
                          select new
                          {
                              Id = at.Id,
                              fromLocationCode = fromLocation.Code,
                              toLocationCode = toLocation.Code,
                              Direction = at.Direction,
                          }).FirstOrDefaultAsync(cancellationToken);
        }

        private async Task SaveChangesAsync(RequestDocument document)
        {
            try
            {
                if (document.DaysAwayDate == null)
                {
                    document.DaysAwayDate = document.DateCreated;
                }

                if (document.Description?.Length > 500)
                {
                    document.Description = document.Description.Substring(0, 500);
                }

                if (document.MailDescription?.Length > 500)
                {
                    document.MailDescription = document.MailDescription.Substring(0, 500);
                }

                Context.RequestDocument.Update(document);
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log exception or handle accordingly
            }
        }



    }
}
