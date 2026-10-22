using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace InvoiceFlow.Api.Errors;

public class UniqueConstraintExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<UniqueConstraintExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not DbUpdateException { InnerException: SqlException { Number: 2601 or 2627 } })
        {
            return false;
        }

        logger.LogWarning(exception, "Unique constraint violation on {Path}", httpContext.Request.Path);

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = "This record conflicts with existing data. It may have been created by another request at the same time."
            }
        });
    }
}