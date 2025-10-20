using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Domain.CustomModel
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AuditAttribute : Attribute
    {
    }
}
