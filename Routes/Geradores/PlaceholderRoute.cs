using GeradorDeDados.Placeholder;
using Microsoft.AspNetCore.Authorization;
using StringPlaceholder;

namespace GeradorDeDados.Routes.Geradores
{
    public static class PlaceholderRoute
    {
        public static void CriarRota(this WebApplication app)
        {
            app.MapPost("/placeholder",
                [Authorize(AuthenticationSchemes = "ApiKey")]
            async (HttpRequest httpContext) =>
                {
                    var data = "";
                    using (StreamReader stream = new StreamReader(httpContext.Body))
                    {
                        data = await stream.ReadToEndAsync();
                    }
                    var stringPlaceholder = new PlaceholderCreator();
                    var placeHolders = PlaceHolders.ObterPlaceholders();
                    var result = stringPlaceholder.Creator(data, placeHolders);
                    return Results.Text(result, contentType: "application/json");
                }).WithTags("Geradores")
                 .WithOpenApi(options =>
                 {
                     var placeHolders = PlaceHolders.ObterPlaceholders();
                     var descricaoPlaceholders = "Placeholders disponíveis:<br/>";
                     foreach (var placeholder in placeHolders)
                     {
                         var aceitaParametros = placeholder.EnabledMultipleParams;
                         var chave = placeholder.Key;
                         var descricao = placeholder.Description;
                         var args = placeholder.Args;
                         var usoDescricao = args != null && args.Count() > 0 ? $"[{chave}({string.Join(",", args)})]" : $"[{chave}]";
                         descricaoPlaceholders += $"{chave}" +
                             "<br/>" +
                             $"Uso: {usoDescricao}" +
                             "<br/>" +
                             $"Descrição: {descricao}<br/>";
                     }
                     options.Summary = "Permite criar um json ou um string com o uso de placeholders.";
                     options.Description = $"{descricaoPlaceholders}";
                     return options;
                 });

        }
    }
}
