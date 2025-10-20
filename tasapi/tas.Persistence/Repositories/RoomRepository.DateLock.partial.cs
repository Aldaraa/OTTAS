using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RoomOwnerAndLockFeature.DateProfileRoomOwnerAndLock;
using tas.Domain.Enums;

namespace tas.Persistence.Repositories
{
    public partial class RoomRepository
    {
        private async Task<int> GetDateLockedCount(int RoomId, DateTime currentDate)
        {

            var activeDocumentActions = new List<string> { "Submitted", "Approved" };


            var requestDocumentIds = await Context.RequestDocument.AsNoTracking().Where(x => x.DocumentType == RequestDocumentType.SiteTravel
                    && x.DocumentTag == "ADD" && activeDocumentActions.Contains(x.CurrentAction)).Select(x => x.Id).ToListAsync();


            var requestDocumentRoomDate = await (from requestAddList in Context.RequestSiteTravelAdd.Where(x => requestDocumentIds.Contains(x.DocumentId) && x.RoomId == RoomId)
                                                 join inschedule in Context.TransportSchedule on requestAddList.inScheduleId equals inschedule.Id /*into inscheduleData
                                                 from inschedule in inscheduleData.DefaultIfEmpty()*/

                                                 join doc in Context.RequestDocument on requestAddList.DocumentId equals doc.Id/*into docData
                                                 from doc in docData.DefaultIfEmpty()*/

                                                 join outschedule in Context.TransportSchedule.AsNoTracking() on requestAddList.outScheduleId equals outschedule.Id /*into outScheduleData
                                                 from outschedule in outScheduleData.DefaultIfEmpty()*/
                                                 select new
                                                 {
                                                     roomId = requestAddList.RoomId,
                                                     EmployeeId = requestAddList.EmployeeId,
                                                     startDate = inschedule.EventDate < outschedule.EventDate ? inschedule.EventDate : outschedule.EventDate,
                                                     endDate = inschedule.EventDate < outschedule.EventDate ? outschedule.EventDate : inschedule.EventDate,
                                                     DocumentId = requestAddList.DocumentId,
                                                     DocumentTag = doc.DocumentTag

                                                 }
                                                ).Where(x => x.startDate.Date <= currentDate && x.endDate > currentDate.Date).ToListAsync();

            return requestDocumentRoomDate.Count();
        }


        private async Task<List<int>> GetRoomLockedBookingAddTravelRequestEmployees(int RoomId, DateTime currentDate)
        {

            var activeDocumentActions = new List<string> { "Submitted", "Approved" };


            var requestDocumentIds = await Context.RequestDocument.AsNoTracking().Where(x => x.DocumentType == RequestDocumentType.SiteTravel
                      && x.DocumentTag == "ADD"
                     && activeDocumentActions.Contains(x.CurrentAction)).Select(x => x.Id).ToListAsync();


            var requestDocumentRoomDateAdd = await (from requestAddList in Context.RequestSiteTravelAdd.AsNoTracking().Where(x => requestDocumentIds.Contains(x.DocumentId) && x.RoomId == RoomId)
                                                    join inschedule in Context.TransportSchedule.AsNoTracking() on requestAddList.inScheduleId equals inschedule.Id /*into inscheduleData
                                                 from inschedule in inscheduleData.DefaultIfEmpty()*/

                                                    join doc in Context.RequestDocument.AsNoTracking() on requestAddList.DocumentId equals doc.Id /*into docData
                                                 from doc in docData.DefaultIfEmpty()*/

                                                    join outschedule in Context.TransportSchedule.AsNoTracking() on requestAddList.outScheduleId equals outschedule.Id /*into outScheduleData
                                                 from outschedule in outScheduleData.DefaultIfEmpty()*/
                                                    select new
                                                    {
                                                        roomId = requestAddList.RoomId,
                                                        EmployeeId = requestAddList.EmployeeId,
                                                        startDate = inschedule.EventDate < outschedule.EventDate ? inschedule.EventDate : outschedule.EventDate,
                                                        endDate = inschedule.EventDate < outschedule.EventDate ? outschedule.EventDate : inschedule.EventDate,
                                                        DocumentId = requestAddList.DocumentId,
                                                        DocumentTag = doc.DocumentTag

                                                    }
                                                ).Where(x => x.startDate.Date <= currentDate.Date && x.endDate.Date >= currentDate.Date).ToListAsync();

            return requestDocumentRoomDateAdd.Select(x => x.EmployeeId).ToList();


        }


        //private async Task<List<int>> GetRoomLockedBookingRemoveTravelRequestEmployees(int RoomId, DateTime currentDate)
        //{

        //    var activeDocumentActions = new List<string> { "Submitted", "Approved" };


        //    var requestDocumentIds = await Context.RequestDocument.AsNoTracking().Where(x => x.DocumentType == RequestDocumentType.SiteTravel
        //              && x.DocumentTag == "REMOVE"
        //             && activeDocumentActions.Contains(x.CurrentAction)).Select(x => x.Id).ToListAsync();




        //    var currentData = await ( from travelData in  Context.RequestSiteTravelRemove.Where(x => x.RoomId == RoomId && requestDocumentIds.Contains(x.DocumentId))
        //                              join firstschedule in Context.TransportSchedule.AsNoTracking() on travelData.FirstScheduleId equals firstschedule.Id
        //                              join activetransport in Context.ActiveTransport.AsNoTracking() on firstschedule.ActiveTransportId equals activetransport.Id
                                      
        //                              )






        //    var requestDocumentRoomDateAdd = await (from requestAddList in Context.RequestSiteTravelRemove.AsNoTracking().Where(x => requestDocumentIds.Contains(x.DocumentId) && x.RoomId == RoomId)
        //                                            join inschedule in Context.TransportSchedule.AsNoTracking() on requestAddList.FirstScheduleId equals inschedule.Id

        //                                            /*into inscheduleData
        //                                         from inschedule in inscheduleData.DefaultIfEmpty()*/

        //                                            join doc in Context.RequestDocument.AsNoTracking() on requestAddList.DocumentId equals doc.Id /*into docData
        //                                         from doc in docData.DefaultIfEmpty()*/

        //                                            join outschedule in Context.TransportSchedule.AsNoTracking() on requestAddList.FirstScheduleId equals outschedule.Id /*into outScheduleData
        //                                         from outschedule in outScheduleData.DefaultIfEmpty()*/
        //                                            select new
        //                                            {
        //                                                roomId = requestAddList.RoomId,
        //                                                EmployeeId = requestAddList.EmployeeId,
        //                                                startDate = inschedule.EventDate < outschedule.EventDate ? inschedule.EventDate : outschedule.EventDate,
        //                                                endDate = inschedule.EventDate < outschedule.EventDate ? outschedule.EventDate : inschedule.EventDate,
        //                                                DocumentId = requestAddList.DocumentId,
        //                                                DocumentTag = doc.DocumentTag

        //                                            }
        //                                        ).Where(x => x.startDate.Date <= currentDate.Date && x.endDate.Date >= currentDate.Date).ToListAsync();

        //    return requestDocumentRoomDateAdd.Select(x => x.EmployeeId).ToList();


        //}





    }
}
