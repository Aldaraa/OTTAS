using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Domain.Enums
{
    public enum AuditType
    {
        None = 0,
        Create = 1,
        Update = 2,
        Delete = 3
    }

    public class AuditConstants
    {
        public static readonly List<string> MASTER_AUDIT_TABLES = new List<string> { "CostCode", "Employer", "Location", "Department", "Nationality", "Position", "PeopleType", "Room", "State", "RosterGroup", "Shift", "Carrier", "TransportMode" };
    }



}
