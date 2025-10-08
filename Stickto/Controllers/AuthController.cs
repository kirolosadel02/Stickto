using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stickto.Modules.UserService.Application.Commands.RegisterUser;
using Stickto.Modules.UserService.Application.Queries.LoginUser;

namespace Stickto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="command">Registration details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Registration response</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisterUserResponse>> Register(
            [FromBody] RegisterUserCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _mediator.Send(command, cancellationToken);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token
        /// </summary>
        /// <param name="query">Login credentials</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Login response with JWT token</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginUserResponse>> Login(
            [FromBody] LoginUserQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _mediator.Send(query, cancellationToken);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Gets current user information (requires authentication)
        /// </summary>
        /// <returns>Current user info</returns>
        [HttpGet("me")]
        [Authorize]
        public ActionResult<object> GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var firstName = User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value;
            var lastName = User.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value;
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            return Ok(new
            {
                UserId = userId,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Role = role
            });
        }

        /// <summary>
        /// Admin-only endpoint for testing authorization
        /// </summary>
        /// <returns>Admin message</returns>
        [HttpGet("admin-only")]
        [Authorize(Policy = "AdminOnly")]
        public ActionResult<object> AdminOnly()
        {
            return Ok(new { message = "This is an admin-only endpoint!" });
        }
    }
}