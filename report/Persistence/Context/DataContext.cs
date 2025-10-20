using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Context
{
    public partial class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            Database.SetCommandTimeout(TimeSpan.FromSeconds(300));

        }

        public DbSet<ReportTemplate> ReportTemplate { get; set; }

        public DbSet<MailSmtpConfig> MailSmtpConfig { get; set; }

        public DbSet<ReportTemplateParameter> ReportTemplateParameter { get; set; }

        public DbSet<ReportTemplateColumn> ReportTemplateColumn { get; set; }

        public DbSet<ReportJob> ReportJob { get; set; }


        public DbSet<ReportJobParameter> ReportJobParameter { get; set; }

        public DbSet<ReportJobColumn> ReportJobColumn { get; set; }


        public DbSet<SysRoleReportTemplate> SysRoleReportTemplate { get; set; }


        public DbSet<ReportJobLog> ReportJobLog { get; set; }


        public DbSet<SysRoleEmployeeReportTemplate> SysRoleEmployeeReportTemplate { get; set; }

        public DbSet<Employee> Employee { get; set; }








    }
}
