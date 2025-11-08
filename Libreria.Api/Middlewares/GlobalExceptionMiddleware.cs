using Libreria.Core.Exceptions;
using Libreria.Core.CustomEntities;
using Libreria.Core.Enums;
using Libreria.Api.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
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
                await WriteErrorResponse(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (DomainValidationException ex)
            {
                await WriteErrorResponse(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (BusinessRuleException ex)
            {
                await WriteErrorResponse(context, HttpStatusCode.Conflict, ex.Message);
            }
            catch (FluentValidation.ValidationException ex)
            {
                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                await WriteErrorResponse(context, HttpStatusCode.BadRequest, "Error de validación.", errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción no controlada");
                await WriteErrorResponse(context, HttpStatusCode.InternalServerError,
                    "Ocurrió un error interno del servidor.");
            }
        }

        // ==================================================
        // Versión actualizada: formato estandarizado JSON
        // ==================================================
        private static async Task WriteErrorResponse(
            HttpContext context,
            HttpStatusCode statusCode,
            string message,
            IDictionary<string, string[]>? errors = null)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            // Crear mensaje de error estándar
            var messages = new[]
            {
                new Message
                {
                    Type = TypeMessage.error.ToString(),
                    Description = message
                }
            };

            // Crear ApiResponse con Messages
            var response = new ApiResponse<object>(null)
            {
                Messages = messages
            };

            // Si hay errores de validación, los añadimos como metadato adicional
            if (errors != null && errors.Any())
            {
                response.Messages = response.Messages
                    .Concat(new[]
                    {
                        new Message
                        {
                            Type = TypeMessage.warning.ToString(),
                            Description = "Errores de validación detectados en los campos enviados."
                        }
                    })
                    .ToArray();
            }

            // Serializar
            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await context.Response.WriteAsync(json);
        }
    }
}
