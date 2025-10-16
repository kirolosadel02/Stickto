namespace Stickto.Modules.UserService.Application.Commands.GoogleLogin
{
    public record GoogleLoginResponse(
        string Token,
        string Message,
        GoogleUserInfo UserInfo
    );

    public record GoogleUserInfo(
        Guid UserId,
        string FirstName,
        string LastName,
        string Email,
        string Role
    );
}
