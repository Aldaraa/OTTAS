using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tas.Application.Features.ActiveTransportFeature.ScheduleListActiveTransport;
using tas.Application.Features.BedFeature.DeleteBed;
using tas.Application.Features.GroupMasterFeature.GetAllGroupMaster;
using tas.Application.Features.ShiftFeature.GetAllShift;
using tas.Domain.Common;
using tas.Domain.CustomModel;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.UsefulServices;

namespace tas.Persistence.Context
{
    public partial class DataContext
    {


        private readonly List<string> MASTER_TABLES_SKIPP_CLOUMN = new List<string> { "DateCreated", "DateDeleted", "DateUpdated", "UserIdCreated", "UserIdUpdated", "UserIdDeleted" };


        public async void AuditSave()
        {
            var context = (DbContext)this;
            context.ChangeTracker.DetectChanges();

            var employeeStatusEntries = context.ChangeTracker.Entries<EmployeeStatus>().ToList();
            if (employeeStatusEntries.Count > 0)
            {
                AuditEmployeeStatusSave(employeeStatusEntries);
            }

            var transporEntries = context.ChangeTracker.Entries<Transport>().ToList();
            if (transporEntries.Count > 0)
            {
                AuditTransportSave(transporEntries);
            }


            var employeeEntries = context.ChangeTracker.Entries<Employee>().ToList();
            if (employeeEntries.Count > 0)
            {
                AuditEmployeeSave(employeeEntries);
            }


            var groupmembersEntries = context.ChangeTracker.Entries<GroupMembers>().ToList();
            if (groupmembersEntries.Count > 0)
            {
                  await   AuditGroupMembersSave(groupmembersEntries);
            }


            var auditEntries = new List<AuditEntry>();
            foreach (var entry in context.ChangeTracker.Entries())
            {

                if(AuditConstants.MASTER_AUDIT_TABLES.Contains(entry.Entity.GetType().Name))
                {
                    var entityType = entry.Entity.GetType();

                    //var auditAttribute = entityType.GetCustomAttribute<AuditAttribute>();
                    //if (auditAttribute == null || entry.Entity == null || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    //{
                    //    continue;
                    //}


                    var auditEntry = new AuditEntry(entry)
                    {
                        TableName = entry.Entity.GetType().Name
                    };


                    foreach (var property in entry.Properties)
                    {
                        var propertyName = property.Metadata.Name;

                        if (!MASTER_TABLES_SKIPP_CLOUMN.Contains(propertyName))
                        {

                            if (property.Metadata.IsPrimaryKey())
                            {
                                auditEntry.KeyValues[propertyName] = property.CurrentValue;
                                continue;
                            }

                            switch (entry.State)
                            {
                                case EntityState.Added:
                                    auditEntry.AuditType = AuditType.Create;
                                    auditEntry.NewValues[propertyName] = property.CurrentValue;
                                    auditEntry.UserId = _hTTPUserRepository.LogCurrentUser()?.Id;
                                    auditEntry.UserName = _hTTPUserRepository.LogCurrentUser()?.Name;

                                    break;
                                case EntityState.Deleted:
                                    auditEntry.AuditType = AuditType.Delete;
                                    auditEntry.UserId = _hTTPUserRepository.LogCurrentUser()?.Id;
                                    auditEntry.UserName = _hTTPUserRepository.LogCurrentUser()?.Name;

                                    auditEntry.OldValues[propertyName] = property.OriginalValue;
                                    break;
                                case EntityState.Modified:
                                    auditEntry.ChangedColumns.Add(propertyName);
                                    auditEntry.AuditType = AuditType.Update;
                                    auditEntry.UserId = _hTTPUserRepository.LogCurrentUser()?.Id;
                                    auditEntry.UserName = _hTTPUserRepository.LogCurrentUser()?.Name;
                                    auditEntry.OldValues[propertyName] = entry.OriginalValues[propertyName];
                                    auditEntry.NewValues[propertyName] = entry.CurrentValues[propertyName];
                                    break;
                            }

                        }
                    
                    }
                    auditEntries.Add(auditEntry);


                }
            }


            foreach (var auditEntry in auditEntries)
            {
                Audit.Add(auditEntry.ToAudit());
            }
        }





        #region ROOMAUDIT

        public void AuditEmployeeStatusSave(IEnumerable<EntityEntry<EmployeeStatus>> employeeStatusEntries)
        {
            if (employeeStatusEntries == null || !employeeStatusEntries.Any())
            {
                return;
            }
            var auditEntries = new List<RoomAudit>();

            foreach (var entry in employeeStatusEntries)
            {
                var entity = entry.Entity;

                // Skip if the entity is null
                if (entity == null)
                {
                    continue;
                }




                if (entry.State == EntityState.Added)
                {
                    if (entity.RoomId.HasValue) { 
                        auditEntries.Add(new RoomAudit
                        {
                            Active = 1,
                            RoomId = entity.RoomId,
                            BedId = entity.BedId,
                            EventDate = entity.EventDate,
                            ShiftId = entity.ShiftId,
                            DateCreated = DateTime.Now,
                            UpdateSource = entity.ChangeRoute,
                            UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                            EmployeeId = entity.EmployeeId,
                        });
                    }
                }

                if (entry.State == EntityState.Modified)
                {
                    var originalBedId = entry.OriginalValues["BedId"] as int?;
                    var originalRoomId = entry.OriginalValues["RoomId"] as int?;

                    if (originalRoomId != entity.RoomId)
                    {
                        auditEntries.Add(new RoomAudit
                        {
                            Active = 1,
                            RoomId = entity.RoomId,
                            BedId = entity.BedId,
                            EventDate = entity.EventDate,
                            ShiftId = entity.ShiftId,
                            DateCreated = DateTime.Now,
                            OldBedId = originalBedId,
                            OldRoomId = originalRoomId,
                            UpdateSource = entity.ChangeRoute,

                            UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                            EmployeeId = entity.EmployeeId,
                        });
                    }

                }


                if (entry.State == EntityState.Deleted)
                {
                    auditEntries.Add(new RoomAudit
                    {
                        Active = 1,
                        RoomId = entity.RoomId,
                        BedId = entity.BedId,
                        EventDate = entity.EventDate,
                        ShiftId = entity.ShiftId,
                        DateCreated = DateTime.Now,
                        OldBedId = entry.OriginalValues["BedId"] as int?,
                        OldRoomId = entry.OriginalValues["RoomId"] as int?,
                        UpdateSource = entity.ChangeRoute,

                        UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                        EmployeeId = entity.EmployeeId,
                    });


                }



            }

            if (auditEntries.Count > 0)
            {
                RoomAudit.AddRange(auditEntries);
            }



        }

        #endregion

        #region TRANSPORT AUDIT




        public void AuditTransportSave(IEnumerable<EntityEntry<Transport>> transportEntries)
        {
            if (transportEntries == null || !transportEntries.Any())
            {
                return;
            }

            var auditEntries = new List<TransportAudit>();

            foreach (var entry in transportEntries)
            {
                var entity = entry.Entity;

                // Skip if the entity is null
                if (entity == null)
                {
                    continue;
                }

                TransportAudit auditEntry = null;

                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    auditEntry = new TransportAudit
                    {
                        Active = 1,
                        Direction = entity.Direction,
                        EventDate = entity.EventDate,
                        ScheduleId = entity.ScheduleId,
                        DateCreated = DateTime.Now,
                        NoShow = entity.NoShow,
                        UpdateSource = entity.ChangeRoute,
                        UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                        EmployeeId = entity.EmployeeId,
                    };

                    if (entry.State == EntityState.Modified)
                    {
                        auditEntry.OldDirection = entry.OriginalValues["Direction"] as string;
                        auditEntry.OldScheduleId = entry.OriginalValues["ScheduleId"] as int?;
                        auditEntry.OldTransportDate = entry.OriginalValues["EventDate"] as DateTime?;

                        if (entity.ScheduleId != auditEntry.OldScheduleId) {
                            auditEntries.Add(auditEntry);
                        }
                    }
                    else {
                        auditEntries.Add(auditEntry);
                    }

                    
                }

                if (entry.State == EntityState.Deleted)
                {
                    auditEntry = new TransportAudit
                    {
                        Active = 1,
                        Direction = entity.Direction,
                        EventDate = entity.EventDate,
                        ScheduleId = entity.ScheduleId,
                        DateCreated = DateTime.Now,
                        UpdateSource = entity.ChangeRoute,
                        UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                        EmployeeId = entity.EmployeeId,
                        NoShow = entity.NoShow,
                        OldDirection = entry.OriginalValues["Direction"] as string,
                        OldScheduleId = entry.OriginalValues["ScheduleId"] as int?,
                        OldTransportDate = entry.OriginalValues["EventDate"] as DateTime?
                    };

                    auditEntries.Add(auditEntry);
                }
            }

            if (auditEntries.Count > 0)
            {
                TransportAudit.AddRange(auditEntries);
            }
        }

    

        #endregion

        #region PROFILE AUDIT

        public void AuditEmployeeSave(IEnumerable<EntityEntry<Employee>> employeeEntries)
        {

            if (employeeEntries == null || !employeeEntries.Any())
            {
                return;
            }
            var auditEntries = new List<ProfileAuditEntry>();


            foreach (var entry in employeeEntries)
            {
                if (entry.Entity == null || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                {
                    continue;
                }


                var auditEntry = new ProfileAuditEntry(entry);

                foreach (var property in entry.Properties)
                {
                    var propertyName = property.Metadata.Name;

                    if (propertyName == "DateCreated" || propertyName == "DateUpdated" || propertyName == "UserIdUpdated" || propertyName == "UserIdCreated")
                    {
                        continue;
                    }
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                      //  continue;
                    }

                    if (entry.State == EntityState.Modified)
                    {
                        if (property.IsModified)
                        {


                            auditEntry.AuditType = AuditType.Update;
                            auditEntry.UserId = _hTTPUserRepository.LogCurrentUser()?.Id;
                            if (property.Metadata.Name.ToLower() == "departmentid")
                            {
                                var aa = 0;
                            }
                           

                            if (property.Metadata.GetColumnType() == "datetime2")
                            {
                                DateTime? originalDateTime = ((DateTime?)property.OriginalValue);
                                DateTime? currentDateTime = ((DateTime?)property.CurrentValue);

                                if (originalDateTime.HasValue && currentDateTime.HasValue)
                                {
                                    // Compare only if both original and current values are not null
                                    if ((originalDateTime - currentDateTime).Value.Days != 0)
                                    {
                                        auditEntry.OldValues[propertyName] = originalDateTime.Value.Date;
                                        auditEntry.NewValues[propertyName] = currentDateTime.Value.Date;
                                        auditEntry.ChangedColumns.Add(propertyName);
                                    }
                                }
                                else if (originalDateTime.HasValue || currentDateTime.HasValue)
                                {
                                    // One value is null and the other is not
                                    auditEntry.OldValues[propertyName] = originalDateTime.GetValueOrDefault().Date == DateTime.MinValue ? null : originalDateTime.GetValueOrDefault().Date;
                                    auditEntry.NewValues[propertyName] = currentDateTime.GetValueOrDefault().Date == DateTime.MinValue ? null : currentDateTime.GetValueOrDefault().Date;
                                    auditEntry.ChangedColumns.Add(propertyName);
                                }

                            }
                           else if (Convert.ToString(property.OriginalValue) != Convert.ToString(property.CurrentValue))
                            {

                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                                auditEntry.ChangedColumns.Add(propertyName);
                            }

                        }

                        if (propertyName == "Id")
                        {
                            auditEntry.EmployeeId = Convert.ToInt32(property.CurrentValue);
                        }
                    }
                }

                auditEntries.Add(auditEntry);
            }

            foreach (var auditEntry in auditEntries)
            {

                if (auditEntry.ChangedColumns.Count > 0 && auditEntry.NewValues.Count > 0)
                {
                    EmployeeAudit.Add(auditEntry.ToAudit());
                }

            }
        }

        #endregion


        #region GROUPMEMBERS_AUDIT

        public async Task AuditGroupMembersSave(IEnumerable<EntityEntry<GroupMembers>> groupMembersEntries)
        {
            if (groupMembersEntries == null || !groupMembersEntries.Any())
            {
                return;
            }

            var auditEntries = new List<GroupMembersAudit>();

            foreach (var entry in groupMembersEntries)
            {
                var entity = entry.Entity;

                // Skip if the entity is null
                if (entity == null)
                {
                    continue;
                }
                if (entity.GroupMasterId == null) { 
                    continue;
                }

                var logSaveStatus = GroupMasterLogStatus(entity.GroupMasterId.Value);

                if (!logSaveStatus) {
                    continue;
                }

                GroupMembersAudit memberEntry = null;

                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    memberEntry = new GroupMembersAudit
                    {
                        Active = 1,
                        GroupMasterId = entity.GroupMasterId,
                        GroupDetailId = entity.GroupDetailId,
                        DateCreated = DateTime.Now,
                        UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                        EmployeeId = entity.EmployeeId,
                    };

                    if (entry.State == EntityState.Modified)
                    {
                        memberEntry.GroupMasterId = entry.OriginalValues["GroupMasterId"] as int?;
                        memberEntry.GroupDetailId = entry.OriginalValues["GroupDetailId"] as int?;

                        if (entity.GroupDetailId!= memberEntry.GroupDetailId)
                        {
                            memberEntry.Action = "Updated";
                            auditEntries.Add(memberEntry);
                        }
                    }
                    else
                    {
                        memberEntry.Action = "Created";
                        auditEntries.Add(memberEntry);
                    }


                }

                if (entry.State == EntityState.Deleted)
                {
                    memberEntry = new GroupMembersAudit
                    {
                        Active = 1,
                        GroupMasterId = entity.GroupMasterId,
                        GroupDetailId = entity.GroupDetailId,
                        DateCreated = DateTime.Now,
                        Action = "Deleted",
                       
                        UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                        EmployeeId = entity.EmployeeId
                    };
                    auditEntries.Add(memberEntry);
                }
            }

            if (auditEntries.Any())
            {
                 GroupMembersAudit.AddRange(auditEntries);
            }
        }


        private bool GroupMasterLogStatus(int GroupMasterId) 
        {
            string cacheEntityName = "GroupMasterAudit";
            var cacheKey = $"API::{cacheEntityName}";
            List<GroupMaster> outData;

            if (_cacheService.TryGetValue(cacheKey, out outData))
            {
                return outData.Any(x => x.Id == GroupMasterId && x.CreateLog == 1);
            }
            else {
                return false;
            }
        }
    



        #endregion


    }

}
