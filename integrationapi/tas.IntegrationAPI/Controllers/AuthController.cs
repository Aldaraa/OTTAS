using Application.Features.AuthenticationFeature.Login;
using Application.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;

namespace tas.IntegrationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly IAuthenticationRepository _authenticationRepository;
        public AuthController(IMediator mediator, IConfiguration configuration, ILogger<AuthController> logger,  IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, IAuthenticationRepository authenticationRepository)
        {
            _configuration = configuration;
            _logger = logger;
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _authenticationRepository = authenticationRepository;
        }


        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="loginRequest">The login request containing the username and password.</param>
        /// <returns>A JWT token if the authentication is successful.</returns>
        /// <response code="200">Returns the JWT token and authentication details.</response>
        /// <response code="400">If the login request is invalid.</response>
        /// <response code="401">If the username or password is incorrect.</response>
        /// <response code="403">If the request is forbidden.</response>
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Authenticates a user and returns a JWT token.", Description = "Authenticates a user using their username and password and returns a JWT token if successful.")]
        [SwaggerResponse(200, "Returns the JWT token and authentication details.", typeof(LoginResponse))]
        [SwaggerResponse(400, "If the login request is invalid.")]
        [SwaggerResponse(401, "If the username or password is incorrect.")]
        [SwaggerResponse(403, "If the request is forbidden.")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Invalid login request.");
            }

            try
            {
                var loginResponse = await _mediator.Send(loginRequest);
                return Ok(loginResponse);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid username or password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Forbid();
            }
        }
    }
}
