using MediatR;

namespace Stickto.Modules.UserService.Application.Queries.LoginUser
{
    public record LoginUserQuery(
        string Email,
        string Password
    ) : IRequest<LoginUserResponse>;
}