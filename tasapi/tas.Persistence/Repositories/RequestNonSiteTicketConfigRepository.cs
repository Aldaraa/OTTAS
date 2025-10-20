using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.RequestNonSiteTicketConfigFeature.ExtractOptioRequestNonSiteTicket;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class RequestNonSiteTicketConfigRepository : BaseRepository<RequestNonSiteTicketConfig>, IRequestNonSiteTicketConfigRepository
    {
        private readonly HTTPUserRepository _HTTPUserRepository;
        public RequestNonSiteTicketConfigRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _HTTPUserRepository = hTTPUserRepository;
        }


        public async Task<ExtractOptioRequestNonSiteTicketResponse> ExtractOption(ExtractOptioRequestNonSiteTicketRequest request, CancellationToken cancellationToken)
        {
            var optionConfigData = await Context.RequestNonSiteTicketConfig.AsNoTracking().ToListAsync();
            var returnData = new ExtractOptioRequestNonSiteTicketResponse();
            var input = request.OptionData;

            if (optionConfigData.Count > 0)
            {
                var flightDetails = new List<ExtractOptionRequestNonSiteTicketExtractedData>();
                var lines = input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);


               returnData.TicketRules =ExtractTicketRules(input);
                    

                for (int i = 1; i < lines.Length; i++)
                {
                    var data = await GetOptionLineData(lines[i], optionConfigData);
                    if (data != null) {
                        flightDetails.Add(data);
                    }
                    
                }
                returnData.FlightData = flightDetails;

                if (flightDetails.Count > 0)
                {
                    if (flightDetails.Count == 1)
                    {
                        returnData.BookIngType = "OT";
                    }
                    else {
                        returnData.BookIngType = flightDetails.First().AirlineCode == flightDetails.Last().AirlineCode ? "RT" : "OT";
                    }
                    
                    
                }

                returnData.TotalMinute = flightDetails.Sum(c => c.TravelDurationMinutes);
                return returnData;
            }
            else {
                throw new BadRequestException("Please register system airlines ticket config data");
            }
        }




        private async Task<ExtractOptionRequestNonSiteTicketExtractedData?> GetOptionLineData(string? optionLine, List<RequestNonSiteTicketConfig> optionConfigData)
        {
            if (string.IsNullOrEmpty(optionLine))
                return null;



            var parts = Regex.Split(optionLine.Trim(), @"\s+");
            if (parts.Length < 12) // Ensure there are enough parts
                return null;


            if (!int.TryParse(parts[0], out var optionNumberFirst))
            {
                return null;
            }

            try
            {


                string? fromAirportCode = parts[6].Substring(0, 3);
                string? toAirportCode = parts[6].Substring(3);

                var fromAirtportData = await GetAirportCountry(fromAirportCode);
                var toAirtportData = await GetAirportCountry(toAirportCode);


                var flightDetail = new ExtractOptionRequestNonSiteTicketExtractedData
                {
                    OptionNumber = int.TryParse(parts[0], out var optionNumber) ? optionNumber : null,
                    AirlineCode = parts[1],
                    AirlineName = optionConfigData.FirstOrDefault(x => x.Code == parts[1])?.Description,
                    TransportNumber = int.TryParse(parts[2], out var transportNumber) ? transportNumber : null,
                    ClassOfSeat = GetSeatClass(optionConfigData, parts[3], parts[1]),
                    TransportDate = DateTime.TryParseExact(parts[4], "ddMMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var transportDate) ? transportDate : null ,
                    WeekNum = int.TryParse(parts[5], out var weekNum) ? weekNum : null,
                    FromAirportCode = fromAirportCode,
                    FromAirportCountry = fromAirtportData?.Country,
                    FromAirportName = fromAirtportData?.Description,
                    ToAirportCode = toAirportCode,
                    ToAirportCountry = toAirtportData?.Country,
                    ToAirportName =toAirtportData?.Description,
                    TicketStatus = parts[7],
                    SeatType = parts[11]
                };


                try
                {
                    DateTime etd;
                    DateTime.TryParseExact(parts[8], "HHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out etd);


                    flightDetail.ETD = $"{etd.ToString("HH:mm")}";



                    DateTime eta;
                    DateTime.TryParseExact(parts[9], "HHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out eta);
                    flightDetail.ETA = $"{eta.ToString("HH:mm")}";

                    flightDetail.DepartureTimeZone = await GetTimeZoneForAirline(flightDetail.FromAirportCode);
                    flightDetail.ArrivalTimeZone = await GetTimeZoneForAirline(flightDetail.ToAirportCode);

                    var departureDateTime = new DateTimeOffset(flightDetail.TransportDate.Value.Date.AddHours(etd.Hour).AddMinutes(etd.Minute),
                                                               TimeZoneInfo.FindSystemTimeZoneById(flightDetail.DepartureTimeZone)
                                                                           .GetUtcOffset(flightDetail.TransportDate.Value.Date.AddHours(etd.Hour).AddMinutes(etd.Minute)));

                    var arrivalDateTime = new DateTimeOffset(flightDetail.TransportDate.Value.Date.AddHours(eta.Hour).AddMinutes(eta.Minute),
                                                             TimeZoneInfo.FindSystemTimeZoneById(flightDetail.ArrivalTimeZone)
                                                                         .GetUtcOffset(flightDetail.TransportDate.Value.Date.AddHours(eta.Hour).AddMinutes(eta.Minute)));

                    flightDetail.ArrivalDate = arrivalDateTime.DateTime;
                    flightDetail.DepartureDate = departureDateTime.DateTime;

                    //   flightDetail.TravelDurationMinutes = (int)(arrivalDateTime - departureDateTime).TotalMinutes;

                    //     flightDetail.TravelDurationMinutes = (int)(arrivalDateTime.UtcDateTime.ToUniversalTime() - departureDateTime.UtcDateTime.ToUniversalTime()).TotalMinutes;

                    //  flightDetail.TravelDurationMinutes = (int)(arrivalDateTime.ToUniversalTime() - departureDateTime.ToUniversalTime()).TotalMinutes;

                    if (arrivalDateTime < departureDateTime)
                    {
                        arrivalDateTime = arrivalDateTime.AddDays(1);
                    }

                    flightDetail.TravelDurationMinutes = (int)(arrivalDateTime.ToUniversalTime() - departureDateTime.ToUniversalTime()).TotalMinutes;

                    if ((int)(arrivalDateTime - departureDateTime).TotalMinutes > 0)
                    {

                    }
                    //else {
                    //    flightDetail.TravelDurationMinutes = (int)(departureDateTime - arrivalDateTime).TotalMinutes;
                    //}


                }
                catch (Exception)
                {
                    
                
                }

                return flightDetail;
            }
            catch(Exception ex)
            {
                var error = ex;
                return null;
            }
        }





        private async Task<RequestAirport> GetAirportCountry(string airportCode)
        {
           var currentData =await Context.RequestAirport.AsNoTracking().Where(x => x.Code == airportCode).FirstOrDefaultAsync();
            if (currentData != null)
            {
                return currentData;
            }
            return new RequestAirport();
        }


        private string GetSeatClass(List<RequestNonSiteTicketConfig> configData, string classofseat, string airportCode) 
        {
            var currentData =  configData.Where(x => x.Code == airportCode).FirstOrDefault();
            if (currentData != null) {
                if (currentData.FirstClass?.IndexOf(classofseat) > -1)
                {
                    return "FIRST CLASS";

                }
                else if (currentData.BusinessClass?.IndexOf(classofseat) > -1)
                {
                    return "BUSINESS CLASS";
                }
                else if (currentData.EconomyClass?.LastIndexOf(classofseat) > -1)
                {
                    return "ECONOMY CLASS";
                }
                else if (currentData.PremiumEconomyClass?.IndexOf(classofseat) >= -1)
                {
                    return "PREMIUM ECONOMY CLASS";
                }
                else {
                    return "ECONOMY CLASS";
                }

            }
            else {
                return "ECONOMY CLASS";
            }
        }





        private  async Task<string> GetTimeZoneForAirline(string airlineCode)
        {
            var currentData = await Context.RequestAirport.Where(x => x.Code == airlineCode).FirstOrDefaultAsync();
            if (currentData != null)
            {
                if (!string.IsNullOrWhiteSpace(currentData.TimeZone))
                { 
                    return currentData.TimeZone;
                }
            }
            return "Asia/Ulaanbaatar";
        }


        #region ExtractTicketRules 
        public static List<ExtractOptionRequestNonSiteTicketExtractedDataTicketRule> ExtractTicketRules(string? input)
        {

            var ticketRules = new List<ExtractOptionRequestNonSiteTicketExtractedDataTicketRule>();
            if (string.IsNullOrWhiteSpace(input))
                return ticketRules;

            var ticketRuleSection = input.Split(new[] { "Ticket rule:" }, StringSplitOptions.None).LastOrDefault();

            if (ticketRuleSection == null)
                return ticketRules;


            var airlineRules = ticketRuleSection.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var rule in airlineRules)
            {
                var ticketRule = new ExtractOptionRequestNonSiteTicketExtractedDataTicketRule();
                var airlineCodeMatch = Regex.Match(rule.TrimStart(), @"^(\w{2})([-:\s]|/|/ )");

                if (airlineCodeMatch.Success)
                {
                    ticketRule.AirlineCode = airlineCodeMatch.Groups[1].Value;
                }


                var refundChargeMatch = Regex.Match(rule, @"(?:REFUND(?: CHARGE)?|Refund charge) (\d+)(USD|AUD)?", RegexOptions.IgnoreCase);
                if (refundChargeMatch.Success)
                {
                    ticketRule.RefundCost = decimal.Parse(refundChargeMatch.Groups[1].Value);
                    ticketRule.TicketCondition = "REFUND";
                }
                else
                {
                    ticketRule.TicketCondition = "NON REFUND";
                    ticketRule.RefundCost = 0;
                }
                var changesMatch = Regex.Match(rule, @"(?:DATE CHANGE|CHANGE) (\d+)(USD|AUD|HKD)?", RegexOptions.IgnoreCase);
                if (changesMatch.Success)
                {
                    ticketRule.Changes = decimal.Parse(changesMatch.Groups[1].Value);
                }

                var noShowMatch = Regex.Match(rule, @"(?:NO[-\s]?SHOW|NOSHOW) (\d+)(USD|AUD|HKD)?", RegexOptions.IgnoreCase);
                if (noShowMatch.Success)
                {
                    ticketRule.NoShowCost = decimal.Parse(noShowMatch.Groups[1].Value);
                }



                var baggageMatch = Regex.Match(rule, @"Baggage: ([\d\w]+)", RegexOptions.IgnoreCase);
                if (baggageMatch.Success)
                {
                    ticketRule.LuggageAllowance = baggageMatch.Groups[1].Value;
                    ticketRule.CarryOnAllowance = baggageMatch.Groups[1].Value;
                }
                else
                {
                    ticketRule.LuggageAllowance = "N/A";
                    ticketRule.CarryOnAllowance = "N/A";
                }

                ticketRules.Add(ticketRule);
            }

            return ticketRules;
        }


        private static decimal ParseBaggageValue(string baggage)
        {
            if (baggage.EndsWith("kg", StringComparison.OrdinalIgnoreCase))
            {
                return decimal.Parse(baggage.TrimEnd('k', 'g'));
            }
            if (baggage.EndsWith("pc", StringComparison.OrdinalIgnoreCase))
            {
                return decimal.Parse(baggage.TrimEnd('p', 'c')) * 20; // Assuming 1 piece equals 20kg for comparison
            }
            return 0;
        }

        #endregion

    }

}
