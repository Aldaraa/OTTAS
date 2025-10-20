using Application.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence.Context;
using Persistence.HostedService;
using Persistence.Repositories;
using Quartz.Spi;

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
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IReportJobRepository, ReportJobRepository>();
            services.AddScoped<IReportTemplateRepository, ReportTemplateRepository>();
            services.AddScoped<IMailSmtpConfigRepository, MailSmtpConfigRepository>();




            services.AddSingleton<JobScopedService>();
            services.AddScoped<JobHostedService>();
            services.AddTransient<IHostedService, JobHostedService>();
            services.AddTransient<IJobExecuteServiceRepository, JobExecuteServiceRepository>();



            services.AddSingleton<IJobFactory, CustomQuartzJobFactory>();
            services.AddTransient<MyJob>();
        }
    }
}