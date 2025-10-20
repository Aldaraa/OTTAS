using AutoMapper.Internal;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text.Json;
using tas.Application.Common.Exceptions;

namespace tas.WebAPI.Extensions
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
                        NotFoundNoDataException =>499, 
                        ForBiddenException => (int)HttpStatusCode.Forbidden,
                        _ => (int)HttpStatusCode.InternalServerError
                    };

                    string[] ErrorData = default;

                    if (contextFeature.Error.GetBaseException().GetType().Name == "BadRequestException") {
                        if (((BadRequestException)contextFeature.Error.GetBaseException()).GetType().GetFieldOrProperty("Errors") != null)
                        {
                            ErrorData = ((BadRequestException)contextFeature.Error.GetBaseException()).Errors;
                        }
                    }
                    //if (contextFeature.Error.GetBaseException().GetType().Name == "ForBiddenException")
                    //{
                    //    if (((ForBiddenException)contextFeature.Error.GetBaseException()).GetType().GetFieldOrProperty("Error") != null)
                    //    {
                    //        ErrorData.Append(((ForBiddenException)contextFeature.Error.GetBaseException()).Error);
                    //    }
                    //}

                    var errorResponse = new
                    {
                        statusCode = context.Response.StatusCode,
                        message = contextFeature.Error.GetBaseException().Message,
                        data = ErrorData
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                });
            });
        }
    }
}
