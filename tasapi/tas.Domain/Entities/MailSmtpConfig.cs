using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class MailSmtpConfig : BaseEntity
    {
        [Required]
        public string smtpServer { get; set; }

        public int smtpPort { get; set; }

        public string? email { get; set; }

        public string? password { get; set; }


    }
}
