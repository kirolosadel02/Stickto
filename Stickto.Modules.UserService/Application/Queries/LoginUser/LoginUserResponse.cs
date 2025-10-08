namespace Stickto.Modules.UserService.Application.Queries.LoginUser
{
    public record LoginUserResponse(
        string Token,
        string Message,
        UserInfo User
    );

    public record UserInfo(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string RoleName
    );
}