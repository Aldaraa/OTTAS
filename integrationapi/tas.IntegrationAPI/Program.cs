using Application;
using Microsoft.Extensions.FileProviders;
using Persistence;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using tas.IntegrationAPI.Extensions;
using tas.IntegrationAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddControllers();
// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.ConfigureCorsPolicy(builder.Configuration);
builder.Services.ConfigureSecurityAuth(builder.Configuration);
builder.Services.ConfigureApplication();
builder.Services.ConfigurePersistence(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.ConfigureLog();


var app = builder.Build();


//app.UseMiddleware<IpRestrictionMiddleware>();

app.UseRouting();

app.UseErrorHandler();
app.MapControllers();

app.UseFileStaticAccess();
app.UseStaticConfiguration();


app.Run();