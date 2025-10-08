using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Stickto.OpenApi
{
    /// <summary>
    /// An operation filter to include authorization information in the OpenAPI documentation.
    /// </summary>
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Applies the security definition to the given operation in the OpenAPI documentation.
        /// </summary>
        /// <param name="operation">The operation to which security requirements will be applied.</param>
        /// <param name="context">The context of the current filter operation.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Define the Bearer token scheme for security
            var bearerScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
            };

            // Define the security requirement for the Bearer token
            var requirement = new OpenApiSecurityRequirement
            {
                [bearerScheme] = Array.Empty<string>(),
            };

            // Apply the security requirement to the operation
            operation.Security = new List<OpenApiSecurityRequirement> { requirement };
        }
    }
}
