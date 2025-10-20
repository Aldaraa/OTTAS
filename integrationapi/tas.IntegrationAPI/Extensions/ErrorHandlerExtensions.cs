using Application.Common.Exceptions;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace tas.IntegrationAPI.Extensions
{
    public static class ErrorHandlerExtensions
    {
        public static void UseErrorHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature == null) return;

                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    context.Response.ContentType = "application/json";

                    context.Response.StatusCode = contextFeature.Error switch
                    {
                        BadRequestException => (int)HttpStatusCode.BadRequest,
                        OperationCanceledException => (int)HttpStatusCode.ServiceUnavailable,
                        NotFoundException => (int)HttpStatusCode.NotFound,
                        ForBiddenException => (int)HttpStatusCode.Forbidden,
                        _ => (int)HttpStatusCode.InternalServerError
                    };

                    string[]? ErrorData = default;

                    if (contextFeature.Error.GetBaseException().GetType().Name == "BadRequestException")
                    {

                        if (((BadRequestException)contextFeature.Error.GetBaseException()).GetType().GetFieldOrProperty("Errors") != null)
                        {
                            ErrorData = ((BadRequestException)contextFeature.Error.GetBaseException()).Errors;
                        }
                    }

                    var errorResponse = new
                    {
                        statusCode = context.Response.StatusCode,
                        message = contextFeature.Error.GetBaseException().Message,
                        data = contextFeature.Error.GetBaseException().Message
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                });
            });
        }
    }
}
