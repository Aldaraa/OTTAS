using Microsoft.Extensions.FileProviders;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using tas.Application.Service;
using tas.Application;
using tas.Persistence;
using tas.WebAPI.Extensions;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System.Net;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();
builder.Services.ConfigureCorsPolicy(builder.Configuration);
builder.Services.ConfigureSecurityAuth(builder.Configuration);
builder.Services.ConfigureApplication();
builder.Services.ConfigurePersistence(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<CacheService>();

//builder.Services.AddSignalR();

//builder.Services.AddSignalR(options =>
//{
//    options.KeepAliveInterval = TimeSpan.FromSeconds(15); // Ping interval
//    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60); // How long to wait before disconnecting inactive clients
//});

/*builder.Services.AddHostedService<CronJob>();*/

builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    hubOptions.HandshakeTimeout = TimeSpan.FromSeconds(30);
    hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(15);
    hubOptions.EnableDetailedErrors = false; // Set to false in production
});


builder.Services.AddTransient<RoleMiddleware>();
builder.Services.AddScoped<SignalrHub>();
//builder.Services.AddTransient<RequestResponseTimeMiddleware>();

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseWebSockets();

var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
if (!Directory.Exists(logDirectory))
{
    Directory.CreateDirectory(logDirectory);
}


app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"logs")),
    RequestPath = new PathString("/log"),
    ServeUnknownFileTypes = true,
    DefaultContentType = "text/plain"
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.DisplayRequestDuration();
    c.DocExpansion(DocExpansion.None);
    c.DocumentTitle = "TAS REST API";
    c.EnableFilter();
});

app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;

    headers["X-Content-Type-Options"] = "nosniff";
    headers["X-Frame-Options"] = "DENY";
    headers["X-XSS-Protection"] = "1; mode=block";
    headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self'; upgrade-insecure-requests;";

    await next();
});



var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".msg"] = "application/vnd.ms-outlook";

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
    RequestPath = new PathString("/Resources"),
    ContentTypeProvider = provider,
    OnPrepareResponse = ctx =>
    {
        // Add security headers
        ctx.Context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        ctx.Context.Response.Headers.Append("X-Frame-Options", "DENY");
        ctx.Context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        ctx.Context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' data: blob:; upgrade-insecure-requests;");
    }
});



app.UseMiddleware<RoleMiddleware>();
app.UseMiddleware<FileAccessMiddleware>();


app.UseResponseCaching();

app.MapControllers();
app.UseErrorHandler();

int route = 0;
app.UseEndpoints(endpoints =>
{
    // endpoints.MapHub<SignalrHub>("/screenhub");

    //endpoints.MapHub<SignalrHub>("/screenhub", options =>
    //{
    //    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets |
    //                         Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
    //});
});

app.ConfigureDefaultRoute();
app.Run();
