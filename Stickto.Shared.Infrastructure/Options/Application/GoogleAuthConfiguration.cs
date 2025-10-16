namespace Stickto.Shared.Infrastructure.Options.Application
{
    /// <summary>
    /// Represents the Google OAuth authentication configuration.
    /// </summary>
    public class GoogleAuthConfiguration
    {
        /// <summary>
        /// Gets or sets the Google OAuth Client ID.
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Google OAuth Client Secret.
        /// </summary>
        public string ClientSecret { get; set; } = string.Empty;
    }
}
