using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Configuration;
using tas.Application.Features.RequestLineManagerEmployeeFeature.GetRequestLineManagerEmployee;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Entities;
using tas.Persistence.Context;
using tas.Persistence.Repositories;
using tas.Persistence.UsefulServices;

namespace tas.Persistence
{
    public static class ServiceExtensions
    {
        public static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
        {

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            //    services.AddDbContextFactory<DataContext>(opt => opt.UseSqlServer(connectionString), ServiceLifetime.Scoped);


            services.AddDbContextFactory<DataContext>(opt =>
            {
                opt.UseSqlServer(connectionString);
                                            //  opt.EnableSensitiveDataLogging();
            }, ServiceLifetime.Scoped);



            services.AddScoped<NotificationHub>();
            services.AddScoped<SignalrHub>();

            services.AddScoped<DataContext>();


            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICostCodeRepository, CostCodeRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<ICampRepository, CampRepository>();
            services.AddScoped<INationalityRepository, NationalityRepository>();
            services.AddScoped<IStateRepository, StateRepository>();
            services.AddScoped<IEmployerRepository, EmployerRepository>();
            services.AddScoped<IGroupMasterRepository, GroupMasterRepository>();
            services.AddScoped<IPeopleTypeRepository, PeopleTypeRepository>();
            services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IBedRepository, BedRepository>();
            services.AddScoped<IRosterGroupRepository, RosterGroupRepository>();
            services.AddScoped<IRosterRepository, RosterRepository>();
            services.AddScoped<IShiftRepository, ShiftRepository>();
            services.AddScoped<IFlightGroupMasterRepository, FlightGroupMasterRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IGroupDetailRepository, GroupDetailRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<ISysTeamRepository, SysTeamRepository>();
            services.AddScoped<ICarrierRepository, CarrierRepository>();
            services.AddScoped<IRosterDetailRepository, RosterDetailRepository>();
            services.AddScoped<IColorRepository, ColorRepository>();
            services.AddScoped<ITransportModeRepository, TransportModeRepository>();
            services.AddScoped<IActiveTransportRepository, ActiveTransportRepository>();
            services.AddScoped<ITransportScheduleRepository, TransportScheduleRepository>();
            services.AddScoped<IClusterRepository, ClusterRepository>();
            services.AddScoped<IFlightGroupDetailRepository, FlightGroupDetailRepository>();
            services.AddScoped<IClusterDetailRepository, ClusterDetailRepository>();
            services.AddScoped<ITransportRepository, TransportRepository>();
            services.AddScoped<IGroupMembersRepository, GroupMembersRepository>();
            services.AddScoped<IEmployeeStatusRepository, EmployeeStatusRepository>();
            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            services.AddScoped<ISysRoleRepository, SysRoleRepository>();
            services.AddScoped<ISysRoleMenuRepository, SysRoleMenuRepository>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<IVisitEventRepository, VisitEventRepository>();
            services.AddScoped<IRosterExecuteRepository, RosterExecuteRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IRequestAirportRepository, RequestAirportRepository>();
            services.AddScoped<IRequestGroupRepository, RequestGroupRepository>();
            services.AddScoped<IRequestGroupConfigRepository, RequestGroupConfigRepository>();
            services.AddScoped<IRequestGroupEmployeeRepository, RequestGroupEmployeeRepository>();
            services.AddScoped<ISysFileRepository, SysFileRepository>();
            services.AddScoped<IRequestDocumentNonSiteTravelRepository, RequestDocumentNonSiteTravelRepository>();
            services.AddScoped<IRequestDocumentRepository, RequestDocumentRepository>();
            services.AddScoped<IRequestNonSiteTravelFlightRepository, RequestNonSiteTravelFlightRepository>();
            services.AddScoped<IRequestNonSiteTravelAccommodationRepository, RequestNonSiteTravelAccommodationRepository>();
            services.AddScoped<IRequestDocumentAttachmentRepository, RequestDocumentAttachmentRepository>();
            services.AddScoped<IRequestDocumentProfileChangeEmployeeRepository, RequestDocumentProfileChangeEmployeeRepository>();
            services.AddScoped<IRequestDeMobilisationTypeRepository, RequestDeMobilisationTypeRepository>();
            services.AddScoped<IRequestDeMobilisationRepository, RequestDeMobilisationRepository>();
            services.AddScoped<IRequestTravelAgentRepository, RequestTravelAgentRepository>();
            services.AddScoped<IRequestTravelPurposeRepository, RequestTravelPurposeRepository>();
            services.AddScoped<IRequestNonSiteTravelOptionRepository, RequestNonSiteTravelOptionRepository>();
            services.AddScoped<IStatusChangesEmployeeRequestRepository, StatusChangesEmployeeRequestRepository>();
            services.AddScoped<IRequestDelegateRepository, RequestDelegateRepository>();
            services.AddScoped<IRequestSiteTravelAddRepository, RequestSiteTravelAddRepository>();
            services.AddScoped<IRequestDocumentHistoryRepository, RequestDocumentHistoryRepository>();
            services.AddScoped<IRequestSiteTravelRescheduleRepository, RequestSiteTravelRescheduleRepository>();
            services.AddScoped<IRequestSiteTravelRemoveRepository, RequestSiteTravelRemoveRepository>();
            services.AddScoped<IJoblogRepository, JoblogRepository>();
            services.AddScoped<IRequestLineManagerEmployeeRepository, RequestLineManagerEmployeeRepository>();

            services.AddScoped<ICheckDataRepository, CheckDataRepository>();
            services.AddScoped<IRoomAssignmentRepository, RoomAssignmentRepository>();
            services.AddScoped<IProfileFieldRepository, ProfileFieldRepository>();
            services.AddScoped<IAgreementRepository, AgreementRepository>();
            services.AddScoped<IMailSmtpConfigRepository, MailSmtpConfigRepository>();

            services.AddScoped<IAuditRepository, AuditRepository>();

            services.AddScoped<IRequestLocalHotelRepository, RequestLocalHotelRepository>();
            services.AddScoped<ITransportCheckerRepository, TransportCheckerRepository>();
            services.AddScoped<ISysResponseTimeRepository, SysResponseTimeRepository>();
            services.AddScoped<ISysVersionRepository, SysVersionRepository>();
            services.AddScoped<ISysRoleReportTemplateRepository, SysRoleReportTemplateRepository>();
            services.AddScoped<IDashboardDataAdminRepository, DashboardDataAdminRepository>();
            services.AddScoped<ISysRoleEmployeeMenuRepository, SysRoleEmployeeMenuRepository>();

            services.AddScoped<ISysRoleEmployeeReportTemplateRepository, SysRoleEmployeeReportTemplateRepository>();
            services.AddScoped<ISysRoleEmployeeReportDepartmentRepository, SysRoleEmployeeReportDepartmentRepository>();



            services.AddScoped<ISysRoleEmployeeReportEmployerRepository, SysRoleEmployeeReportEmployerRepository>();



            services.AddScoped<IRequestExternalTravelReScheduleRepository, RequestExternalTravelReScheduleRepository>();
            services.AddScoped<IRequestExternalTravelAddRepository, RequestExternalTravelAddRepository>();
            services.AddScoped<IRequestExternalTravelRemoveRepository, RequestExternalTravelRemoveRepository>();

            services.AddScoped<IRequestNonSiteTicketConfigRepository, RequestNonSiteTicketConfigRepository>();


            services.AddScoped<ITransportScheduleCalculateRepository, TransportScheduleCalculateRepository>();

            services.AddScoped<IDashboardAccomAdminRepository, DashboardAccomAdminRepository>();
            services.AddScoped<IDashboardTransportAdminRepository, DashboardTransportAdminRepository>();
            services.AddScoped<IDashboardSystemAdminRepository, DashboardSystemAdminRepository>();
            services.AddScoped<IDashboardRequestRepository, DashboardRequestRepository>();

            services.AddScoped<ISafeModeEmployeeStatusRepository, SafeModeEmployeeStatusRepository>();
            services.AddScoped<ISafeModeTransportRepository, SafeModeTransportRepository>();

            services.AddScoped<ISysRoleEmployeeDashboardRepository, SysRoleEmployeeDashboardRepository>();
            services.AddScoped<IDepartmentCostCodeRepository, DepartmentCostCodeRepository>();
            services.AddScoped<IEmployerAdminRepository, EmployerAdminRepository>();

            services.AddScoped<IBusstopRepository, BusstopRepository>();
            services.AddScoped<IDepartmentGroupConfigRepository, DepartmentGroupConfigRepository>();

            
        }
    }
}
