using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Entities;
using tas.Persistence.Repositories;

namespace tas.Persistence.Context
{
    public partial class DataContext : DbContext
    {
        private readonly HTTPUserRepository _hTTPUserRepository;
        private readonly IConfiguration _configuration;

        private readonly CacheService _cacheService;
        public DataContext(HTTPUserRepository hTTPUserRepository)
        {
        }

        public DataContext(DbContextOptions<DataContext> options, HTTPUserRepository hTTPUserRepository, IConfiguration configuration, CacheService cacheService) : base(options)
        {
           
            Database.SetCommandTimeout(TimeSpan.FromSeconds(600));
            _hTTPUserRepository = hTTPUserRepository;
            _configuration = configuration;
            _cacheService = cacheService;
        }



        public virtual async Task<int> SaveChangesAsync()
        {
            try
            {

                AuditSave();
                return await base.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                var message = ex.Message;
                // Log the exception or handle it as needed
                return 0;
            }
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
                optionsBuilder.EnableSensitiveDataLogging();
            }
        }


        public DbSet<User> Users { get; set; }
        public DbSet<CostCode> CostCodes { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<Position> Position { get; set; }
        public DbSet<Camp> Camp { get; set; }
        public DbSet<Nationality> Nationality { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<Employer> Employer { get; set; }
        public DbSet<GroupMaster> GroupMaster { get; set; }
        public DbSet<PeopleType> PeopleType { get; set; }
        public DbSet<RoomType> RoomType { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<Bed> Bed { get; set; }
        public DbSet<RosterGroup> RosterGroup { get; set; }
        public DbSet<Roster> Roster { get; set; }
        public DbSet<Shift> Shift { get; set; }
        public DbSet<FlightGroupMaster> FlightGroupMaster { get; set; }

        public DbSet<Employee> Employee { get; set; }
        public DbSet<GroupDetail> GroupDetail { get; set; }

        public DbSet<SysMenu> SysMenu { get; set; }

        public DbSet<SysTeam> SysTeam { get; set; }

        public DbSet<SysTeamMenu> SysTeamMenu { get; set; }

        public DbSet<SysTeamUser> SysTeamUser { get; set; }

        public DbSet<Carrier> Carrier { get; set; }

        public DbSet<RosterDetail> RosterDetail { get; set; }

        public DbSet<Color> Color { get; set; }
     //   public DbSet<TerminationType> TerminationType { get; set; }

        public DbSet<TransportMode> TransportMode { get; set; }

        public DbSet<ActiveTransport> ActiveTransport { get; set; }

        public DbSet<TransportSchedule> TransportSchedule { get; set; }


        public DbSet<Cluster> Cluster { get; set; }


        public DbSet<FlightGroupDetail> FlightGroupDetail { get; set; }

        public DbSet<ClusterDetail> ClusterDetail { get; set; }

        public DbSet<Transport> Transport { get; set; }

        public DbSet<EmployeeStatus> EmployeeStatus { get; set; }  

        public DbSet<GroupMembers> GroupMembers { get; set; }

        public DbSet<SysRole> SysRole { get; set; }

        public DbSet<SysRoleEmployees> SysRoleEmployees { get; set; }

        public DbSet<SysRoleMenu> SysRoleMenu { get; set; }

        public DbSet<VisitEvent> VisitEvent { get; set; }

        public DbSet<VisitEventlEmployees> VisitEventlEmployees { get; set; }

        public DbSet<TransportNoShow> TransportNoShow { get; set; }


        public DbSet<EmployeeHistory> EmployeeHistory { get; set; }

        public DbSet<Notification> Notification { get; set; }

        public DbSet<NotificationEmployee> NotificationEmployee { get; set; }

        public DbSet<RequestAirport> RequestAirport { get; set; }

        public DbSet<RequestGroup> RequestGroup { get; set; }

        public DbSet<RequestGroupConfig> RequestGroupConfig { get; set; }

        public DbSet<RequestGroupEmployee> RequestGroupEmployee { get; set; }

        public DbSet<SysFile> SysFile { get; set; }

        public DbSet<RequestDocument> RequestDocument { get; set; }

        public DbSet<RequestDocumentHistory> RequestDocumentHistory { get; set; }
        public DbSet<RequestNonSiteTravel> RequestNonSiteTravel { get; set; }

        public DbSet<RequestNonSiteTravelAccommodation> RequestNonSiteTravelAccommodation { get; set; }
        public DbSet<RequestDocumentAttachment> RequestNonSiteTravelAttachment { get; set; }

        public DbSet<RequestNonSiteTravelFlight> RequestNonSiteTravelFlight { get; set; }

        public DbSet<RequestDocumentProfileChangeEmployee> RequestDocumentProfileChangeEmployee { get; set; }

        public DbSet<RequestDeMobilisationType> RequestDeMobilisationType { get; set; }

        public DbSet<RequestDeMobilisation> RequestDeMobilisation { get; set; }

        public DbSet<RequestTravelAgent> RequestTravelAgent { get; set; }

        public DbSet<RequestTravelPurpose> RequestTravelPurpose { get; set; }
        public DbSet<RequestNonSiteTravelOption> RequestNonSiteTravelOption { get; set; }

        public DbSet<RequestDocumentAttachment> RequestDocumentAttachment { get; set; }

        public DbSet<Joblog> Joblog { get; set; }

        public DbSet<StatusChangesEmployeeRequest> StatusChangesEmployeeRequest { get; set; }

        public DbSet<RequestDelegates> RequestDelegates { get; set; }

        public DbSet<RequestSiteTravelAdd> RequestSiteTravelAdd { get; set; }
        public DbSet<RequestSiteTravelReschedule> RequestSiteTravelReschedule { get; set; }

        public DbSet<RequestSiteTravelRemove> RequestSiteTravelRemove { get; set; }

        public DbSet<RequestLineManagerEmployee> RequestLineManagerEmployee { get; set; }


        public DbSet<DepartmentAdmin> DepartmentAdmin { get; set; }

        public DbSet<DepartmentSupervisor> DepartmentSupervisor { get; set; }

        public DbSet<DepartmentManager> DepartmentManager { get; set; }

        public DbSet<Audit> Audit { get; set; }
        public DbSet<RoomAssignment> RoomAssignment { get; set; }

        public DbSet<ProfileField> ProfileField { get; set; }

        public DbSet<Agreement> Agreement { get; set; }


        public DbSet<MailSmtpConfig> MailSmtpConfig { get; set; }


        public DbSet<RoomAudit> RoomAudit { get; set; }

        public DbSet<TransportAudit> TransportAudit { get; set; }

        public DbSet<EmployeeAudit> EmployeeAudit { get; set; }


        public DbSet<RequestLocalHotel> RequestLocalHotel { get; set; }


        public DbSet<RequestDocumentProfileChangeEmployeeTemp> RequestDocumentProfileChangeEmployeeTemp { get; set; }

        public DbSet<SysResponseTime> SysResponseTime { get; set; }

        public DbSet<SysVersion> SysVersion { get; set; }

        public DbSet<SysRoleReportTemplate> SysRoleReportTemplate { get; set; }

        public DbSet<ReportTemplate> ReportTemplate { get; set; }

        public DbSet<SysRoleEmployeeMenu> SysRoleEmployeeMenu { get; set; }

        public DbSet<SysRoleEmployeeReportTemplate> SysRoleEmployeeReportTemplate { get; set; }

        public DbSet<SysRoleEmployeeReportDepartment> SysRoleEmployeeReportDepartment { get; set; }

        public DbSet<SysRoleEmployeeReportEmployer> SysRoleEmployeeReportEmployer { get; set; }

        public DbSet<RequestExternalTravelAdd> RequestExternalTravelAdd { get; set; }

        public DbSet<RequestExternalTravelRemove> RequestExternalTravelRemove { get; set; }

        public DbSet<RequestExternalTravelReschedule> RequestExternalTravelReschedule { get; set; }

        public DbSet<RequestNonSiteTicketConfig> RequestNonSiteTicketConfig { get; set; }

        public DbSet<SysDashboard> SysDashboard { get; set; }

        public DbSet<SysRoleEmployeeDashboard> SysRoleEmployeeDashboard { get; set; }

        public DbSet<GroupMembersAudit> GroupMembersAudit { get; set; }
        public DbSet<DepartmentCostCode> DepartmentCostCode { get; set; }

        public DbSet<EmployerAdmin> EmployerAdmin { get; set; }

        public DbSet<Busstop> Busstop { get; set; }
        public DbSet<TransportScheduleBusstop> TransportScheduleBusstop { get; set; }

        public DbSet<DepartmentGroupConfig> DepartmentGroupConfig { get; set; }


        public DbSet<TransportGoShow> TransportGoShow { get; set; }




    }
}
