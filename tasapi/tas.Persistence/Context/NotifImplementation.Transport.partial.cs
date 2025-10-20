using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using tas.Domain.CustomModel;
using tas.Domain.Entities;

namespace tas.Persistence.Context
{
    public partial class DataContext
    {
        private async Task TransportNotificationListener(List<int> ids)
        {
            var changedTransport = ChangeTracker.Entries<Transport>()
            .Where(o => o.State == EntityState.Added || o.State == EntityState.Modified);

            if (changedTransport.Count() == 0)
            {
                return;
            }
            foreach (var transport in changedTransport)
            {
                if (transport.State == EntityState.Modified)
                {
                    var currentValues = transport.CurrentValues;
                    var originalValues = transport.OriginalValues;

                    var newScheduleId = Convert.ToInt32(currentValues["ScheduleId"]);
                    var oldScheduleId = Convert.ToInt32(originalValues["ScheduleId"]);
                    var empId = currentValues["EmployeeId"];
                    var direction = currentValues["Direction"];
                    var currentDate = currentValues["EventDate"];
                    var currentUserIdCreated = currentValues["UserIdCreated"];

                    var notifData = await NotificationTransportModified(Convert.ToInt32(empId), Convert.ToInt32(newScheduleId), Convert.ToInt32(oldScheduleId), Convert.ToString(direction));

                    var newNotif = new Notification
                    {
                        DateCreated = DateTime.Now,
                        Description = notifData.Description,
                        Active = 1,
                        UserIdCreated = Convert.ToInt32(currentUserIdCreated),
                        link = notifData.Link,
                        NotifIndex = notifData.NotifIndex
                    };

                    Notification.Add(newNotif);
                    await AccommodationNotificationCreate(notifData.NotifIndex, Convert.ToInt32(currentUserIdCreated), ids);

                }
                if (transport.State == EntityState.Added)
                {
                    var currentValues = transport.CurrentValues;
                    if (currentValues["Status"].ToString() == "Over Booked")
                    {
                        var newScheduleId =Convert.ToInt32(currentValues["ScheduleId"]);
                        var empId = currentValues["EmployeeId"];
                        var currentDate = currentValues["EventDate"];
                        var direction = currentValues["Direction"];

                        var currentUserIdCreated = currentValues["UserIdCreated"];

                       

                        var notifData = await NotificationTransportAdded(Convert.ToInt32(empId),  newScheduleId, Convert.ToString(direction));

                        var newNotif = new Notification
                        {
                            DateCreated = DateTime.Now,
                            Description = notifData.Description,
                            Active = 1,
                            UserIdCreated = Convert.ToInt32(currentUserIdCreated),
                            link = notifData.Link,
                            NotifIndex = notifData.NotifIndex
                        };

                        Notification.Add(newNotif);
                        await AccommodationNotificationCreate(notifData.NotifIndex, Convert.ToInt32(currentUserIdCreated), ids);
                    }
                 

                }
            }




        }

        private async Task<NotificationDetail> NotificationTransportAdded(int EmployeeId, int newScheduleId, string direction)
        {
            var returnData = new NotificationDetail();
            var currentSchedule = await TransportSchedule.Where(x => x.Id == newScheduleId).FirstOrDefaultAsync();
            var currentEmployee = await Employee
                .Where(x => x.Id == EmployeeId)
                .Select(x => new { x.Firstname, x.Lastname, x.SAPID })
                .FirstOrDefaultAsync();

            returnData.Description = $"{EmployeeId} #{currentEmployee?.SAPID} {currentEmployee?.Firstname}" +
                $" {currentEmployee?.Lastname} Flight data for {direction} on {currentSchedule?.EventDate.ToString("yyyy")}" +
                $" of the employee was created as OVERBOOKED";
            returnData.Link = $"/tas/people/search/{EmployeeId}/flight?startDate={currentSchedule?.EventDate.ToString("yyyy-MM-dd")}&endDate={currentSchedule?.EventDate.ToString("yyyy-MM-dd")}";
            returnData.NotifIndex = Guid.NewGuid().ToString();

            return returnData;

        }  
        
        private async Task<NotificationDetail> NotificationTransportModified(int EmployeeId, int newScheduleId, int oldScheduleId, string direction)
        {
            var returnData = new NotificationDetail();
            var currentSchedule = await TransportSchedule.Where(x => x.Id == newScheduleId).FirstOrDefaultAsync();
            var oldSchedule = await TransportSchedule.Where(x => x.Id == oldScheduleId).FirstOrDefaultAsync();

            var currentEmployee = await Employee
                .Where(x => x.Id == EmployeeId)
                .Select(x => new { x.Firstname, x.Lastname, x.SAPID })
                .FirstOrDefaultAsync();

            returnData.Description = $"{EmployeeId} #{currentEmployee?.SAPID} {currentEmployee?.Firstname}" +
                $" {currentEmployee?.Lastname} Flight Information of Employee on  {oldSchedule?.EventDate.ToString("yyyy")} to {direction}" +
                $" Changed to {currentSchedule?.EventDate.ToString("yyyy-MM-dd")}";
            returnData.Link = $"/tas/people/search/{EmployeeId}/flight?startDate={currentSchedule?.EventDate.ToString("yyyy-MM-dd")}&endDate={currentSchedule?.EventDate.ToString("yyyy-MM-dd")}";
            returnData.NotifIndex = Guid.NewGuid().ToString();

            return returnData;

        }

    }
}
