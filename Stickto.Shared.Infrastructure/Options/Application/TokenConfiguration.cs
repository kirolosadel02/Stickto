namespace Stickto.Shared.Infrastructure.Options.Application
{
    /// <summary>
    /// Represents the configuration settings for a token.
    /// </summary>
    public class TokenConfiguration
    {
        /// <summary>
        /// Gets or sets the key used to sign the token.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the issuer of the token.
        /// </summary>
        public string Issuer { get; set; }
    }
}
