using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Domain.Enums;

namespace tas.Persistence.UsefulServices
{
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }
        public EntityEntry Entry { get; }
        public int? UserId { get; set; }

        public string? UserName { get; set; }



        public string TableName { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public AuditType AuditType { get; set; }
        public List<string> ChangedColumns { get; } = new List<string>();
        public Audit ToAudit()
        {
            var audit = new Audit();
            audit.Type = AuditType.ToString();
            audit.TableName = TableName;
            audit.DateCreated = DateTime.Now;
            audit.DateUpdated = DateTime.Now;
            audit.UserName = UserName;
            audit.UserId = UserId;


            audit.Active = 1;
            //     UserId = OldValues["UserIdCreated"];
            audit.DateTime = DateTime.Now;
            audit.PrimaryKey =Convert.ToString(KeyValues["Id"]); //JsonConvert.SerializeObject(KeyValues);
            audit.OldValues = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues);
            audit.NewValues = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues);
            audit.AffectedColumns = ChangedColumns.Count == 0 ? null : JsonConvert.SerializeObject(ChangedColumns);
            return audit;
        }
    }


    public class ProfileAuditEntry
    {
        public ProfileAuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }
        public EntityEntry Entry { get; }
        public int? UserId { get; set; }

        public int? EmployeeId { get; set; }

        public AuditType AuditType { get; set; }

        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public List<string> ChangedColumns { get; } = new List<string>();
        public EmployeeAudit ToAudit()
        {
            var audit = new EmployeeAudit();
            audit.Type = AuditType.ToString();
            audit.DateCreated = DateTime.Now;
            audit.DateUpdated = DateTime.Now;
            audit.UserId = UserId;


            audit.Active = 1;
            audit.EmployeeId = EmployeeId;
            audit.DateTime = DateTime.Now;
            audit.UserIdUpdated = UserId;
            audit.UserIdCreated = UserId;
            audit.DateUpdated = DateTime.Now;
            audit.DateCreated = DateTime.Now;
            audit.PrimaryKey = JsonConvert.SerializeObject(KeyValues);
            audit.OldValues = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues);
            audit.NewValues = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues);
            audit.AffectedColumns = ChangedColumns.Count == 0 ? null : JsonConvert.SerializeObject(ChangedColumns);
            return audit;
        }
    }
}
