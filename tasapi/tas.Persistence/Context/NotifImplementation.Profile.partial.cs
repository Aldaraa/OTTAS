using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.CustomModel;
using tas.Domain.Entities;
using tas.Domain.Enums;

namespace tas.Persistence.Context
{
    public partial class DataContext
    {
        private async Task ProfileNotificationListener(List<int> ids)
        {
            var changedEmployeeStatus = ChangeTracker.Entries<Employee>()
            .Where(o =>  o.State == EntityState.Modified);

            if (changedEmployeeStatus.Count() == 0)
                return;
            foreach (var empStatus in changedEmployeeStatus)
            {
                if (empStatus.State == EntityState.Modified)
                {
                    var currentValues = empStatus.CurrentValues;
                    var empId = currentValues["Id"];
                    var chn = currentValues["Id"];
                    var currentUserIdCreated = currentValues["UserIdCreated"];

                    var notifData = await NotificationProfileModified(Convert.ToInt32(empId));

                    var newNotif = new Notification
                    {
                        DateCreated = DateTime.Now,
                        Description = notifData.Description,
                        Active = 1,
                        UserIdCreated = Convert.ToInt32(currentUserIdCreated),
                        link = notifData.Link,
                        NotificationType = SystemNotificationType.PROFILE_CHANGES,
                        NotifIndex = notifData.NotifIndex
                    };

                    Notification.Add(newNotif);
                    await AccommodationNotificationCreate(notifData.NotifIndex, Convert.ToInt32(currentUserIdCreated), ids);

                }
               
            }
        }

        private async Task<NotificationDetail> NotificationProfileModified(int EmployeeId)
        {
            var returnData = new NotificationDetail();

            var currentEmployee = await Employee
                .Where(x => x.Id == EmployeeId)
                .Select(x => new { x.Firstname, x.Lastname, x.SAPID })
                .FirstOrDefaultAsync();


            returnData.Description = $"{EmployeeId} #{currentEmployee?.SAPID} {currentEmployee?.Firstname}" +
                $" {currentEmployee?.Lastname} Changed employee information";
            returnData.Link = $"/tas/people/search/{EmployeeId}";
            returnData.NotifIndex = Guid.NewGuid().ToString();

            return returnData;

        }

    }
}
