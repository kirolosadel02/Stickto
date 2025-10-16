namespace Stickto.Shared.Infrastructure.Options.Application
{
    /// <summary>
    /// Represents the application-specific options.
    /// </summary>
    public class ApplicationOptions
    {
        /// <summary>
        /// Gets the connection string for the database.
        /// </summary>
        public string ConnectionString { get; init; }

        /// <summary>
        /// Gets or sets the list of origins that are allowed to access the application.
        /// </summary>
        public IList<string> Origins { get; set; }

        /// <summary>
        /// Gets or sets the configuration settings for a token.
        /// </summary>
        public TokenConfiguration Token { get; set; }

        /// <summary>
        /// Gets or sets the Google OAuth authentication settings.
        /// </summary>
        public GoogleAuthConfiguration GoogleAuth { get; set; }
    }
}
