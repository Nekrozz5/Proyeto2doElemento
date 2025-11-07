using Libreria.Core.Exceptions;
using Libreria.Core.Responses; // ✅ importa tu ApiResponse
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;

namespace Libreria.Api.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                await WriteErrorResponse(context, StatusCodes.Status404NotFound, ex.Message);
            }
            catch (DomainValidationException ex)
            {
                await WriteErrorResponse(context, StatusCodes.Status400BadRequest, ex.Message, ex.Errors);
            }
            catch (BusinessRuleException ex)
            {
                await WriteErrorResponse(context, StatusCodes.Status409Conflict, ex.Message);
            }
            catch (FluentValidation.ValidationException ex)
            {
                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                await WriteErrorResponse(context, StatusCodes.Status400BadRequest, "Error de validación.", errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción no controlada");
                await WriteErrorResponse(context, StatusCodes.Status500InternalServerError,
                    "Ocurrió un error interno del servidor.");
            }
        }

        // ================================
        // ✅ Respuesta unificada ApiResponse
        // ================================
        private static async Task WriteErrorResponse(
            HttpContext context,
            int statusCode,
            string message,
            IDictionary<string, string[]>? errors = null)
        {
            var apiError = new ApiError
            {
                StatusCode = statusCode,
                Type = ReasonPhrases.GetReasonPhrase(statusCode),
                Detail = message,
                Errors = errors
            };

            var response = new ApiResponse<object>(message, apiError);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}
