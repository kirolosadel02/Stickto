using System.Diagnostics.CodeAnalysis;
using System.Net;


namespace Stickto.Shared.Abstractions.Responses
{
    /// <summary>
    /// Represents the result of an operation.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="isSuccess">Indicates whether the operation was successful.</param>
        /// <param name="statusCode">The status code of the operation.</param>
        /// <param name="message">The message associated with the operation.</param>
        /// <param name="errors">The errors occurred during the operation.</param>
        /// <exception cref="InvalidOperationException">Thrown when the provided arguments are invalid.</exception>
        public Result(bool isSuccess, HttpStatusCode statusCode, string message, IEnumerable<ApplicationError> errors)
        {
            var errorList = errors.ToList();

            if ((isSuccess && errorList.Count > 0) || (!isSuccess && errorList.Count == 0))
            {
                throw new InvalidOperationException("Invalid result state.");
            }

            IsSuccess = isSuccess;
            Errors = errorList;
            Message = message;
            StatusCode = statusCode;
        }

        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets the message associated with the operation.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the errors occurred during the operation.
        /// </summary>
        public IReadOnlyList<ApplicationError> Errors { get; }

        /// <summary>
        /// Gets the status code of the operation.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <returns>A successful result.</returns>
        public static Result Success() => new(true, HttpStatusCode.OK, string.Empty, Array.Empty<ApplicationError>());

        /// <summary>
        /// Creates a successful result with a value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="data">The value associated with the successful result.</param>
        /// <returns>A successful result with a value.</returns>
        public static Result<TValue> Success<TValue>(TValue data) =>
            new(data, true, HttpStatusCode.OK, string.Empty, Array.Empty<ApplicationError>());

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="statusCode">The status code of the operation.</param>
        /// <param name="message">The message associated with the operation.</param>
        /// <param name="errors">The errors occurred during the operation.</param>
        /// <returns>A failed result.</returns>
        public static Result Failure(HttpStatusCode statusCode, string message, IEnumerable<ApplicationError> errors) =>
            new(false, statusCode, message, errors);

        /// <summary>
        /// Creates a failed result with a specified type.
        /// </summary>
        /// <typeparam name="TValue">The type of the value associated with the result.</typeparam>
        /// <param name="statusCode">The status code of the operation.</param>
        /// <param name="message">The message associated with the operation.</param>
        /// <param name="errors">The errors occurred during the operation.</param>
        /// <returns>A failed result of type <typeparamref name="TValue"/>.</returns>
        public static Result<TValue> Failure<TValue>(HttpStatusCode statusCode, string message, IEnumerable<ApplicationError> errors) =>
            new(default, false, statusCode, message, errors);

        /// <summary>
        /// Creates a successful result if the condition is met, otherwise a failed result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="data">The value to be included in the success result.</param>
        /// <param name="statusCode">The status code for the failure result.</param>
        /// <param name="message">The message for the failure result.</param>
        /// <param name="errors">The errors for the failure result.</param>
        /// <returns>A successful result if the condition is met, otherwise a failed result.</returns>
        public static Result<TValue> SuccessIf<TValue>(
            bool condition,
            TValue data,
            HttpStatusCode statusCode,
            string message,
            IList<ApplicationError> errors)
        {
            return condition ? Success(data)
                : Failure<TValue>(statusCode, message, errors);
        }
    }

    /// <summary>
    /// Represents the result of an operation with a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public sealed class Result<TValue> : Result
    {
        private readonly TValue? _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TValue}"/> class.
        /// </summary>
        /// <param name="value">The value associated with the result.</param>
        /// <param name="isSuccess">Indicates whether the operation was successful.</param>
        /// <param name="statusCode">The status code of the operation.</param>
        /// <param name="message">The message associated with the operation.</param>
        /// <param name="errors">The errors occurred during the operation.</param>
        public Result(TValue? value, bool isSuccess, HttpStatusCode statusCode, string message, IEnumerable<ApplicationError> errors)
            : base(isSuccess, statusCode, message, errors)
        {
            _value = value;
        }

        /// <summary>
        /// Gets the value associated with the result.
        /// </summary>
        [AllowNull]
        public TValue? Data => IsSuccess ? _value : default;

        /// <summary>
        /// Creates a failed result with a value.
        /// </summary>
        /// <param name="statusCode">The status code of the operation.</param>
        /// <param name="message">The message associated with the operation.</param>
        /// <param name="errors">The errors occurred during the operation.</param>
        /// <returns>A failed result with a value.</returns>
        public static Result<TValue> WithErrors(
            HttpStatusCode statusCode,
            string message,
            IEnumerable<ApplicationError> errors) => new(default, false, statusCode, message, errors);

        /// <summary>
        /// Converts a <see cref="Result"/> to a <see cref="Result{TValue}"/>.
        /// </summary>
        /// <param name="result">The result to convert.</param>
        /// <returns>A new <see cref="Result{TValue}"/> instance.</returns>
        public static Result<TValue> FromResult(Result result)
        {
            return new Result<TValue>(default, result.IsSuccess, result.StatusCode, result.Message, result.Errors);
        }

        /// <summary>
        /// Converts the given value to a successful result.
        /// </summary>
        /// <param name="data">The value to convert.</param>
        /// <returns>A successful result with the given value.</returns>
        public Result<TValue?> ToResult(TValue? data)
        {
            return Success(data);
        }
    }
}
