using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Stickto.Shared.Abstractions.Responses;
using System.Net;

namespace Stickto.Shared.Infrastructure.Handlers
{
    /// <summary>
    /// Represents a global exception handler.
    /// This class is responsible for handling all unhandled exceptions.
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private static readonly Action<ILogger, string, string, Exception> _logError =
        LoggerMessage.Define<string, string>(
            LogLevel.Error,
            new EventId(1, nameof(LogError)),
            "Exception Message: {Message}, StackTrace: {StackTrace}");

        private readonly ILogger<GlobalExceptionHandler> logger;
        private readonly IWebHostEnvironment webHostEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalExceptionHandler"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="webHostEnvironment">The web host environment.</param>
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment webHostEnvironment)
        {
            this.logger = logger;
            this.webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Tries to handle the exception asynchronously.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="exception">The exception to handle.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A value indicating whether the exception was handled.</returns>
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            LogError(logger, exception);

            var exceptionMessage = webHostEnvironment.IsDevelopment() || webHostEnvironment.IsStaging()
                ? $"Exception Message: {exception.Message}, \n " +
                    $"Inner Exception Message: {exception.InnerException?.Message}, \n" +
                    $"Stack Trace: {exception.StackTrace}"
                : Abstractions.Resources.Common.ServerError;

            //httpContext.SetContextException(exception);

            var statusCode = HttpStatusCode.InternalServerError;
            httpContext.Response.StatusCode = (int)statusCode;
            var errorResponse = Result.Failure(
                statusCode,
                Abstractions.Resources.Common.ServerError,
                [
                    ApplicationError.Create("ServerError", exceptionMessage),
                ]);

            await httpContext.Response.WriteAsJsonAsync(errorResponse);

            return true;
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception to log.</param>
        private static void LogError(ILogger logger, Exception exception)
        {
            _logError(logger, exception.Message, exception.StackTrace ?? string.Empty, exception);
        }
    }

}
