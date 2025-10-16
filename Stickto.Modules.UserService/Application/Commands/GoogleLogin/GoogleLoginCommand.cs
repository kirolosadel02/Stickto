using MediatR;
namespace Stickto.Modules.UserService.Application.Commands.GoogleLogin
{
    public record GoogleLoginCommand(string IdToken) : IRequest<GoogleLoginResponse>;
}