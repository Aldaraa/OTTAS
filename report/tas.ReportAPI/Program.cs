using Application;
using Application.Service;
using Microsoft.Extensions.FileProviders;
using Persistence;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using tas.ReportAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache();


builder.Services.ConfigureCorsPolicy(builder.Configuration);
builder.Services.ConfigureSecurityAuth(builder.Configuration);
builder.Services.ConfigurePersistence(builder.Configuration);
builder.Services.ConfigureApplication();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<CacheService>();
builder.ConfigureLogManagement();

//builder.Services.AddScoped<AuthMiddleware>();


var app = builder.Build();

app.UseStaticFiles();

app.UseFileStaticAccess();
app.UseStaticConfiguration();

app.Run();
