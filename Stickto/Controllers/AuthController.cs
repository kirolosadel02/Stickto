using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stickto.Modules.UserService.Application.Commands.GoogleLogin;
using Stickto.Modules.UserService.Application.Commands.RegisterUser;
using Stickto.Modules.UserService.Application.Queries.LoginUser;

namespace Stickto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IMediator mediator) : ControllerBase
    {
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
                var response = await mediator.Send(command, cancellationToken);
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
                var response = await mediator.Send(query, cancellationToken);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Authenticates a user using Google OAuth and returns a JWT token
        /// </summary>
        /// <param name="command">Google ID token</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Login response with JWT token</returns>
        /// <remarks>
        /// To test this endpoint without a frontend:
        /// 
        /// 1. Go to https://developers.google.com/oauthplayground
        /// 2. Click on the settings gear icon (top right)
        /// 3. Check "Use your own OAuth credentials"
        /// 4. Enter your Google Client ID and Client Secret from your Google Cloud Console
        /// 5. In Step 1, find and select "Google OAuth2 API v2" -> https://www.googleapis.com/auth/userinfo.email
        /// 6. Click "Authorize APIs"
        /// 7. After authorization, click "Exchange authorization code for tokens"
        /// 8. Copy the "id_token" value (not the access_token)
        /// 9. Use that id_token in the request body: { "idToken": "YOUR_ID_TOKEN_HERE" }
        /// 
        /// Note: The id_token expires quickly (usually within an hour)
        /// </remarks>
        [HttpPost("google-login")]
        [AllowAnonymous]
        public async Task<ActionResult<GoogleLoginResponse>> GoogleLogin(
            [FromBody] GoogleLoginCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await mediator.Send(command, cancellationToken);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
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