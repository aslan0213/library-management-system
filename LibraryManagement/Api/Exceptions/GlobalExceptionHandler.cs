using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace LibraryManagement.Api.Exceptions
{
	public class GlobalExceptionHandler :IExceptionHandler
	{
		public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
		{
			var (statusCode, title, detail, extensions) = MapException(exception);

			var problemDetails = new ProblemDetails
			{
				Status = statusCode,
				Title = title,
				Detail = detail,
				Instance = httpContext.Request.Path
			};

			if (extensions is not null)
			{
				foreach (var (key, value) in extensions)
				{
					problemDetails.Extensions[key] = value;
				}
			}

			httpContext.Response.StatusCode = statusCode;
			await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

			return true;
		}

		private static (int StatusCode, string Title, string Detail, Dictionary<string, object?>? Extensions) MapException(Exception exception) =>
			exception switch
			{
				NotFoundException notFound => (
					StatusCodes.Status404NotFound,
					"Resource not found",
					notFound.Message,
					null),

				BusinessRuleViolationException businessRule => (
					StatusCodes.Status409Conflict,
					"Business rule violation",
					businessRule.Message,
					null),

				ValidationException validation => (
					StatusCodes.Status400BadRequest,
					"Validation failed",
					"One or more validation errors occurred.",
					new Dictionary<string, object?>
					{
						["errors"] = validation.Errors
							.GroupBy(e => e.PropertyName)
							.ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
					}),

				_ => (
					StatusCodes.Status500InternalServerError,
					"An unexpected error occurred",
					"Please try again later or contact support if the problem persists.",
					null)
			};
	}
}
