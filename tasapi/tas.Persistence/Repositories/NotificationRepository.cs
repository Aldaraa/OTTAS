using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.SearchEmployee;
using tas.Application.Features.NotificationFeature.EmployeeNoticationShortcut;
using tas.Application.Features.NotificationFeature.NotificationGetAll;
using tas.Application.Features.NotificationFeature.NotificationGetDetail;
using tas.Application.Repositories;
using tas.Application.Utils;
using tas.Domain.CustomModel;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _hTTPUserRepository;
        public NotificationRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
           _hTTPUserRepository = hTTPUserRepository;
        }

        public async Task<NotificationGetDetailResponse> NotificationGetDetail(NotificationGetDetailRequest request, CancellationToken cancellationToken)
        {

            var currentNotification = await Context.Notification
                .Where(x => x.NotifIndex == request.NotifIndex).FirstOrDefaultAsync();

            var returnData = new NotificationGetDetailResponse();
            if (currentNotification != null)
            {
                var employeeId = _hTTPUserRepository.LogCurrentUser()?.Id;
                var employeeNotification = await Context.NotificationEmployee.Where(x => x.NotifIndex == request.NotifIndex &&
                x.EmployeeId == employeeId.Value
                ).FirstOrDefaultAsync();
                if (employeeNotification != null) 
                {
                    employeeNotification.ViewStatus = 1;
                    Context.NotificationEmployee.Update(employeeNotification);
                    await  Context.SaveChangesAsync();
                }
                returnData.link = currentNotification.link;                    
            }
            return returnData;
        }


        public async Task<EmployeeNoticationShortcutResponse> EmployeeNoticationShortcut(EmployeeNoticationShortcutRequest request, CancellationToken cancellationToken)
        {
            var employeeId = request.EmployeeId;

            var empNotif = await (from empNotifs in Context.NotificationEmployee.Where(x => x.EmployeeId == employeeId)
                                  join notifs in Context.Notification on
                                  empNotifs.NotifIndex equals notifs.NotifIndex into notificationData
                                  from notif in notificationData.DefaultIfEmpty()
                                  join createdEmp in Context.Employee on
                                  notif.UserIdCreated equals createdEmp.Id into createdEmpData
                                  from empData in createdEmpData.DefaultIfEmpty()
                                  select new EmployeeNoticationShortcutDetail
                                  {
                                      Id = empNotifs.Id,
                                      Description = notif.Description,
                                      RelativeTime = notif.DateCreated.Value.TimeAgo(),
                                      ViewStatus = empNotifs.ViewStatus,
                                      NotifIndex = notif.NotifIndex,
                                      
                                      ChangeEmployee = $"{empData.Id} {empData.Firstname} {empData.Firstname}"

                                  }).OrderByDescending(x=>x.Id).OrderBy(x=> x.ViewStatus).Take(10).ToListAsync();
         var employeeNotifications =await Context.NotificationEmployee
                
                .Where(x => x.EmployeeId == employeeId)
                .OrderBy(x => x.DateCreated).ToListAsync();
            var returnData = new EmployeeNoticationShortcutResponse();
            returnData.NewNotificationCount = employeeNotifications.Where(x=> x.ViewStatus == 0).Count();
            returnData.EmployeeNoticationShortcutDetails = empNotif;
            return returnData;
        }

        public async Task<NotificationGetAllResponse> NotificationGetAll(NotificationGetAllRequest request, CancellationToken cancellationToken)
        {
            int pageSize = request.pageSize == 0 ? 10 : request.pageSize;
            int pageIndex = request.pageIndex;
            var employeeId = _hTTPUserRepository.LogCurrentUser()?.Id;

            var empNotif = await (from empNotifs in Context.NotificationEmployee.Where(x => x.EmployeeId == employeeId)
                                  join notifs in Context.Notification on
                                  empNotifs.NotifIndex equals notifs.NotifIndex into notificationData
                                  from notif in notificationData.DefaultIfEmpty()
                                  join createdEmp in Context.Employee on
                                  notif.UserIdCreated equals createdEmp.Id into createdEmpData
                                  from empData in createdEmpData.DefaultIfEmpty()
                                  select new NotificationGetAllResult
                                  {
                                      Id = empNotifs.Id,
                                      Description = notif.Description,
                                      RelativeTime = notif.DateCreated.Value.TimeAgo(),
                                      ViewStatus = empNotifs.ViewStatus,
                                      NotifIndex = notif.NotifIndex,
                                      ChangeEmployee = $"{empData.Id} {empData.Firstname} {empData.Firstname}"

                                  }).OrderByDescending(x => x.Id).ToListAsync();

            var returnData = new NotificationGetAllResponse
            {
                data = empNotif
                 .Skip(pageIndex * pageSize)
                 .Take(pageSize)
                 .ToList<NotificationGetAllResult>(),
                        pageSize = pageSize,
                        currentPage = pageIndex,
                        totalcount = empNotif.Count()
            };

            return returnData;
        }




    }
}
