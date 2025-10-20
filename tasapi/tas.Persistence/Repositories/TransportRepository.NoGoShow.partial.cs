using MediatR.Wrappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.TransportFeature.CreateNoGoShow;
using tas.Application.Features.TransportFeature.DeleteNoGoShow;
using tas.Domain.Entities;

namespace tas.Persistence.Repositories
{
    public partial class TransportRepository
    {

        #region DeleteNoGoShow

        public async Task DeleteNoGoShow(DeleteNoGoShowRequest request, CancellationToken cancellationToken)
        {
            if (request.NoShow)
            {
                var currentData =await Context.TransportNoShow.Where(c =>c.Id == request.Id).FirstOrDefaultAsync();
                if (currentData != null)
                {
                    Context.TransportNoShow.Remove(currentData);
                }

            }
            else
            {
                var currentData = await Context.TransportGoShow.Where(c => c.Id == request.Id).FirstOrDefaultAsync();
                if (currentData != null)
                {
                    Context.TransportGoShow.Remove(currentData);
                }
            }

        }


        #endregion


        #region CreateGoNoShow
        public async Task CreateNoGoShow(CreateNoGoShowRequest request, CancellationToken cancellationToken)
        {
            if (request.NoShow)
            {
                var newRecord = new TransportNoShow
                {
                    DateCreated = DateTime.Now,
                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                    Direction = request.Direction,
                    EmployeeId = request.EmployeeId,
                    Reason = request.Reason,
                    Active = 1,
                    Description = request.Description,
                    EventDate = request.EventDate,
                    EventDateTime = request.EventDate
                };

                Context.TransportNoShow.Add(newRecord);
            }
            else
            {
                var newRecord = new TransportGoShow
                {
                    DateCreated = DateTime.Now,
                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                    Direction = request.Direction,
                    EmployeeId = request.EmployeeId,
                    Reason = request.Reason,
                    Active = 1,
                    Description = request.Description,
                    EventDate = request.EventDate,
                    EventDateTime = request.EventDate
                };

                Context.TransportGoShow.Add(newRecord);
            }

        }

        #endregion





        private async Task NoGoShowSave(int EmployeeId, int ScheduleId, int? showStatus, string reason, bool NoShow = true)
        {
            if (NoShow)
            {
                if (showStatus.HasValue)
                {
                    if (showStatus.Value == 1)
                    {
                        var transportScheduleData = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == ScheduleId)
                                                           join activetransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activetransport.Id into activetransportData
                                                           from activetransport in activetransportData.DefaultIfEmpty()
                                                           select new
                                                           {
                                                               EventDate = schedule.EventDate,
                                                               TransportCode = activetransport.Code,
                                                               Direction = activetransport.Direction,
                                                               ScheduleDescription = schedule.Description
                                                           }).FirstOrDefaultAsync();
                        
                        if (transportScheduleData != null)
                        {
                            var newRecord = new TransportNoShow
                            {
                                DateCreated = DateTime.Now,
                                UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                Direction = transportScheduleData.Direction,
                                EmployeeId = EmployeeId,
                                Reason = reason,
                                Description = $"{transportScheduleData.TransportCode} {transportScheduleData.ScheduleDescription} {transportScheduleData.Direction}",
                                Active = 1,
                                EventDate = transportScheduleData.EventDate,
                                EventDateTime = transportScheduleData.EventDate
                            };

                            Context.TransportNoShow.Add(newRecord);

                        }
                    }

                }

            }
            else
            {

                if (showStatus.HasValue)
                {
                    if (showStatus.Value == 1)
                    {
                        var transportScheduleData = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == ScheduleId)
                                                           join activetransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activetransport.Id into activetransportData
                                                           from activetransport in activetransportData.DefaultIfEmpty()
                                                           select new
                                                           {
                                                               EventDate = schedule.EventDate,
                                                               TransportCode = activetransport.Code,
                                                               Direction = activetransport.Direction,
                                                               ScheduleDescription = schedule.Description
                                                           }).FirstOrDefaultAsync();
                        if (transportScheduleData != null)
                        {
                            var newRecord = new TransportGoShow
                            {
                                DateCreated = DateTime.Now,
                                UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                Direction = transportScheduleData.Direction,
                                EmployeeId = EmployeeId,
                                Reason =reason,
                                Description = $"{transportScheduleData.TransportCode} {transportScheduleData.ScheduleDescription} {transportScheduleData.Direction}",
                                Active = 1,
                                EventDate = transportScheduleData.EventDate,
                                EventDateTime = transportScheduleData.EventDate
                            };

                            Context.TransportGoShow.Add(newRecord);

                        }

                    }

                }
            }
        }

    }
}
