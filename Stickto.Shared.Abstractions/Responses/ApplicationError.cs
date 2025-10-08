namespace Stickto.Shared.Abstractions.Responses
{
    /// <summary>
    /// Represents an error with a code and name.
    /// </summary>
    public record ApplicationError
    {
        /// <summary>
        /// Gets the error code.
        /// </summary>
        public string Code { get; init; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string Message { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationError"/> class.
        /// Initializes a new instance of the <see cref="ApplicationError"/> record.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="message">The error message.</param>
        private ApplicationError(string code, string message)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// Represents no error.
        /// </summary>
        public static readonly ApplicationError None = new(string.Empty, string.Empty);

        /// <summary>
        /// Represents an error caused by a null value.
        /// </summary>
        public static readonly ApplicationError NullValue = new("Error.NullValue", "Null value was provided");

        /// <summary>
        /// Represents an error caused by an invalid value.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="message">The error message.</param>
        /// <returns> returns instance of application error with code and message.</returns>
        public static ApplicationError Create(string code, string message) => new(code, message);
    }

}
