﻿using GeradorDeDados.Models.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace GeradorDeDados.Handlers
{
    public static class ExceptionHandler
    {
        public static void AddCustomExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler(builder =>
            builder.Run(async context =>
            {
                context.Response.ContentType = "application/json";
                var exceptionHandlerPathFeature =
                    context.Features.Get<IExceptionHandlerPathFeature>();
                if (exceptionHandlerPathFeature?.Error is CustomException customException)
                {
                    context.Response.StatusCode = customException.StatusCode;
                    await context.Response.WriteAsJsonAsync(customException.ObterResponse());
                }
            }));
        }

    }
}
