using Application.Repositories;
using Application.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence.Context;
using Persistence.HostedService;
using Persistence.Repositories;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Persistence.HostedService.JobHostedService;

namespace Persistence
{
    public static class ServiceExtensions
    {
        public static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContextFactory<DataContext>(opt => opt.UseSqlServer(connectionString), ServiceLifetime.Transient);
            services.AddScoped<DataContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<PasswordService>();
            services.AddSingleton<TokenService>();

            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();

            services.AddScoped<ITransportInfoRepository, TransportInfoRepository>();
            services.AddScoped<IHotDeskRepository, HotDeskRepository>();



            services.AddSingleton<JobScopedService>();
            services.AddScoped<JobHostedService>();
            services.AddTransient<IHostedService, JobHostedService>();



            services.AddSingleton<IJobFactory, CustomQuartzJobFactory>();
            services.AddTransient<MyJob>();
            services.AddTransient<OTDeskEmployeeSyncJob>();
            services.AddTransient<OTDeskEmployeeStatusSyncJob>();
            services.AddTransient<OTDeskDepartmentSyncJob>();





        }

    }
}
