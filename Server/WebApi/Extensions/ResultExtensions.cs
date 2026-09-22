using Domain.Common;
using DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Extensions;

/// <summary>
/// Translates failed results into the HTTP responses that match their kind.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Builds the error response for a failed result.
    /// </summary>
    /// <param name="failure">The failed result to translate.</param>
    /// <returns>404 for a missing entity; 409 for a rule violation or a lost race.</returns>
    public static ObjectResult ToErrorResult(this Result failure)
    {
        var error = new ErrorResponse { Error = failure.Error! };

        return failure.ErrorKind switch
        {
            ResultErrorKind.NotFound => new NotFoundObjectResult(error),
            _ => new ConflictObjectResult(error)
        };
    }
}
