using GeradorDeDados.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace GeradorDeDados.Routes.Geradores
{
    public static class PlaceholderRoute
    {
        public static void CriarRota(this WebApplication app)
        {
            app.MapPost("/placeholder",
                [Authorize(AuthenticationSchemes = "ApiKey")]
            async (HttpRequest httpContext, [FromServices] Placeholder placeHolder) =>
                {
                    var data = "";
                    using (StreamReader stream = new StreamReader(httpContext.Body))
                    {
                        data = await stream.ReadToEndAsync();
                    }
                    var resultado = placeHolder.ObterTexto(data);
                    return Results.Content(resultado);
                }).WithTags("Geradores")
                 .WithOpenApi(options =>
                 {
                     var placeholder = app.Services.GetRequiredService<Placeholder>();
                     var descricao = placeholder.ObterDescricao();
                     options.Summary = "Permite criar um texto com o uso de placeholders.";
                     options.Description = $"{descricao}";
                     return options;
                 });

        }
    }
}
