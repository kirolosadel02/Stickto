namespace Stickto.Shared.Infrastructure.Services.JwtTokenService
{
    public interface IJwtTokenService
    {
        string GenerateToken(Guid userId, string email, string firstName, string lastName, string roleName);
    }
}
