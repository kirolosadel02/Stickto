namespace Stickto.Modules.UserService.Domain.Extensions
{
    public static class UserExtensions
    {
        /// <summary>
        /// Validates that the user entity is in a valid state based on authentication type
        /// </summary>
        public static void ValidateUserState(this Entities.User user)
        {
            // If user is using local authentication, password is required
            if (string.IsNullOrEmpty(user.AuthProvider) && string.IsNullOrEmpty(user.Password))
            {
                throw new InvalidOperationException(
                    "Password is required for users not using external authentication providers.");
            }

            // If user is using OAuth, password should be null and external info should be present
            if (!string.IsNullOrEmpty(user.AuthProvider))
            {
                if (string.IsNullOrEmpty(user.ExternalUserId))
                {
                    throw new InvalidOperationException(
                        $"ExternalUserId is required when using {user.AuthProvider} authentication.");
                }
            }

            // Email is always required
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new InvalidOperationException("Email is required.");
            }
        }

        /// <summary>
        /// Checks if the user is using OAuth authentication
        /// </summary>
        public static bool IsOAuthUser(this Entities.User user)
        {
            return !string.IsNullOrEmpty(user.AuthProvider);
        }

        /// <summary>
        /// Checks if the user is using local authentication
        /// </summary>
        public static bool IsLocalUser(this Entities.User user)
        {
            return string.IsNullOrEmpty(user.AuthProvider);
        }
    }
}

