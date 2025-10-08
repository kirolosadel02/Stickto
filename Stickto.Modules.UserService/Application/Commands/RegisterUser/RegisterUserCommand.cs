using MediatR;

namespace Stickto.Modules.UserService.Application.Commands.RegisterUser
{
    public record RegisterUserCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string Street,
        string City,
        string State,
        string Country,
        string ZipCode,
        int RoleId = 1 // Default to User role
    ) : IRequest<RegisterUserResponse>;
}