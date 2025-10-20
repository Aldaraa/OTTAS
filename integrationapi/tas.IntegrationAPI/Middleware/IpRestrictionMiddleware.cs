namespace tas.IntegrationAPI.Middleware
{
    public class IpRestrictionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<IpRestrictionMiddleware> _logger;
        private readonly HashSet<string> _allowedIps;
        private readonly bool _secureMode = true;

        public IpRestrictionMiddleware(RequestDelegate next, ILogger<IpRestrictionMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _allowedIps = configuration.GetSection("AppSettings:Secure:AllowedIps").Get<HashSet<string>>();
            _secureMode = configuration.GetSection("AppSettings:Secure:SecureMode").Get<bool>();

        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (_secureMode)
            {
                var remoteIp = context.Connection.RemoteIpAddress?.ToString();
                if (string.IsNullOrEmpty(remoteIp) || !_allowedIps.Contains(remoteIp))
                {
                    _logger.LogWarning("Forbidden request from IP: {RemoteIp}", remoteIp);
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Forbidden: Your IP address is not allowed to access this resource.");
                    return;
                }

            }


            await _next(context);
        }
    }
}
