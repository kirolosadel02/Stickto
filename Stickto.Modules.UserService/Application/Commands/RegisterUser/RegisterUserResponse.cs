namespace Stickto.Modules.UserService.Application.Commands.RegisterUser
{
    public record RegisterUserResponse(
        Guid UserId,
        string Message
    );
}