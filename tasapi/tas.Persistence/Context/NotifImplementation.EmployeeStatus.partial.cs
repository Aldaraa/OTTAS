using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.CustomModel;
using tas.Domain.Entities;

namespace tas.Persistence.Context
{
    public partial class DataContext
    {

        private async Task<List<int>> SystemAllRoleEmployees()
        {
          return SysRoleEmployees.Where(x=> x.EmployeeId != null).Select(x => x.EmployeeId.Value).ToListAsync().Result;
        }

        private async Task EmployeeStatusNotificationListener()
        {
            var changedEmployeeStatus = ChangeTracker.Entries<EmployeeStatus>()
            .Where(o => o.State == EntityState.Added || o.State == EntityState.Modified);

            if (changedEmployeeStatus.Count() == 0)
            {
                return;
            }

            var ids =await SystemAllRoleEmployees();
            foreach (var empStatus in changedEmployeeStatus)
            {
                if (empStatus.State == EntityState.Modified)
                {
                    var currentValues = empStatus.CurrentValues;
                    var originalValues = empStatus.OriginalValues;
                    var newRoomId = currentValues["RoomId"];
                    var oldRoomId = originalValues["RoomId"];
                    var empId = currentValues["EmployeeId"];
                    var currentDate = currentValues["EventDate"];
                    var currentUserIdCreated = currentValues["UserIdCreated"];

                    var notifData = await NotificationEmployeeStatusModified(Convert.ToInt32(newRoomId), Convert.ToInt32(oldRoomId), Convert.ToInt32(empId), Convert.ToDateTime(currentDate));

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
                if (empStatus.State == EntityState.Added)
                {
                    var currentValues = empStatus.CurrentValues;
                    var originalValues = empStatus.OriginalValues;
                    var newRoomId = currentValues["RoomId"];
                    var oldRoomId = originalValues["RoomId"];
                    var empId = currentValues["EmployeeId"];
                    var currentDate = currentValues["EventDate"];
                    var currentUserIdCreated = currentValues["UserIdCreated"];

                    var notifData = await NotificationEmployeeStatusAdded(Convert.ToInt32(newRoomId), Convert.ToInt32(empId), Convert.ToDateTime(currentDate));

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




        private async Task<NotificationDetail> NotificationEmployeeStatusModified(int oldRoomId, int newRoomId, int EmployeeId, DateTime eventDate)
        {
            var returnData = new NotificationDetail();

            var newRoom = await Room.Where(x => x.Id == newRoomId).Select(x => new { x.Number }).FirstOrDefaultAsync();
            var oldRoom = await Room.Where(x => x.Id == oldRoomId).Select(x => new { x.Number }).FirstOrDefaultAsync();
            var currentEmployee = await Employee
                .Where(x => x.Id == EmployeeId)
                .Select(x => new { x.Firstname, x.Lastname, x.SAPID })
                .FirstOrDefaultAsync();


            returnData.Description = $"{EmployeeId} #{currentEmployee?.SAPID} {currentEmployee?.Firstname}" +
                $" {currentEmployee?.Lastname} {eventDate.ToString("yyyy-MM-dd")} Moved from room {oldRoom?.Number} to room {newRoom?.Number}.";
            returnData.Link = $"/tas/byperson?empId={EmployeeId}&startDate={eventDate.ToString("yyyy-MM-dd")}&endDate={eventDate.ToString("yyyy-MM-dd")}";
            returnData.NotifIndex = Guid.NewGuid().ToString();

            return returnData;

        }

        private async Task<NotificationDetail> NotificationEmployeeStatusAdded( int newRoomId, int EmployeeId, DateTime eventDate)
        {
            var returnData = new NotificationDetail();

            var newRoom = await Room.Where(x => x.Id == newRoomId).Select(x => new { x.Number, x.CampId}).FirstOrDefaultAsync();
            var newRoomCamp = await Camp.Where(x => x.Id == newRoom.CampId).FirstOrDefaultAsync();

            string descrTemplate = "{0} {1} {2} {3} {4}  Entered room {5} {6}";

            var currentEmployee = await Employee
                .Where(x => x.Id == EmployeeId)
                .Select(x => new { x.Firstname, x.Lastname, x.SAPID })
                .FirstOrDefaultAsync();


            returnData.Description = string.Format(descrTemplate, EmployeeId, currentEmployee?.SAPID, currentEmployee?.Firstname, currentEmployee?.Lastname, eventDate.ToString("yyyy-MM-dd"), newRoomCamp?.Description, newRoom?.Number);
            returnData.Link = $"/tas/byperson?empId={EmployeeId}&startDate={eventDate.ToString("yyyy-MM-dd")}&endDate={eventDate.ToString("yyyy-MM-dd")}";
            returnData.NotifIndex = Guid.NewGuid().ToString();



            return returnData;

        }


        private async Task AccommodationNotificationCreate(string NotifIndex, int? createdUserId, List<int> ids)
        {
            //   string roleTag = "AccomAdmin";
            //var  currentRole =await SysRole.Where(x => x.RoleTag == roleTag).FirstOrDefaultAsync();
            //   if (currentRole != null)
            //   {
            //     var roleemployees = await  SysRoleEmployees
            //           .Where(x => x.RoleId == currentRole.Id).Select(x=> new { x.EmployeeId})
            //           .ToListAsync();

            foreach (var item in ids)
            {
                var localdata = NotificationEmployee.Local.Where(x => x.EmployeeId == item && x.NotifIndex == NotifIndex).FirstOrDefault();
                if (localdata == null)
                {
                    var newEmpNotifcation = new NotificationEmployee
                    {
                        Active = 1,
                        DateCreated = DateTime.Now,
                        EmployeeId = item,
                        NotifIndex = NotifIndex,
                        ViewStatus = 0,
                        UserIdCreated = createdUserId.Value
                    };

                    NotificationEmployee.Add(newEmpNotifcation);
                }
            }
            //}

            await AdminNotificationCreate(NotifIndex, createdUserId, ids);
        }

        private async Task AdminNotificationCreate(string NotifIndex, int? createdUserId, List<int> ids)
        {
            //string roleTag = "SystemAdmin";
            //var currentRole = await SysRole.Where(x => x.RoleTag == roleTag).FirstOrDefaultAsync();
            //if (currentRole != null)
            //{
            //    var roleemployees = await SysRoleEmployees
            //          .Where(x => x.RoleId == currentRole.Id).Select(x => new { x.EmployeeId })
            //          .ToListAsync();

            foreach (var item in ids)
            {
                var localdata = NotificationEmployee.Local.Where(x => x.EmployeeId == item && x.NotifIndex == NotifIndex).FirstOrDefault();
                if (localdata == null)
                {
                    var newEmpNotifcation = new NotificationEmployee
                    {
                        Active = 1,
                        DateCreated = DateTime.Now,
                        EmployeeId = item,
                        NotifIndex = NotifIndex,
                        ViewStatus = 0,
                        UserIdCreated = createdUserId.Value
                    };

                    NotificationEmployee.Add(newEmpNotifcation);
                }


            }
           // }
        }



    }



}
