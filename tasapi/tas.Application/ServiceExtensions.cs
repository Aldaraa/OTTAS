using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Behaviors;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Application.Utils;

namespace tas.Application
{
    public static class ServiceExtensions
    {
        public static void ConfigureApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient<HTTPUserRepository, HTTPUserRepository>();
            services.AddScoped<BulkImportExcelService>();
            services.AddScoped<EmailSender>();
            services.AddTransient<FileAccessMiddleware>();

            services.AddControllers()

        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            options.SerializerSettings.MaxDepth = 5; // Adjust the maximum depth as needed
            options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        });
          //  services.AddSingleton<MonitoringService>();

        }
    }
}
