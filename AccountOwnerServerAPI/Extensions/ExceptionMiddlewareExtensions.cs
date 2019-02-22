using Entities.ExtendedModels;
using LoggerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AccountOwnerServerAPI.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILoggerManager logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        var ExceptionIdNew = Guid.NewGuid();

                        logger.LogError($"Id Excepcion: {ExceptionIdNew} - Something went wrong: {contextFeature.Error} ");

                        await context.Response.WriteAsync(new ErrorModel()
                        {
                            ExceptionId = ExceptionIdNew,
                            StatusCode = context.Response.StatusCode,
                            ErrorMessage = "Internal Server Error."
                        }.ToString());
                    }
                });
            });

        }
    }
}
