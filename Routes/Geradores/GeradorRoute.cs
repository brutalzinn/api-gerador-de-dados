using Bogus;
using GeradorDeDados.Integrations.ReceitaWS;
using GeradorDeDados.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StringPlaceholder;
using System.Text.Json;

namespace GeradorDeDados.Routes.Geradores
{
    public static class GeradorRoute
    {
        public static void CriarRota(this WebApplication app)
        {
            app.MapGet("/obterCNPJValido/{filtroSocio}/{filtroSituacao}/{normalizado}",
          [Authorize(AuthenticationSchemes = "ApiKey")]
            (
      [FromServices] IRedisService redisService,
      [FromRoute] FiltroSocio filtroSocio,
      [FromRoute] Situacao filtroSituacao,
      [FromRoute] bool normalizado) =>
          {
              var listaCNPJValido = redisService.Get<List<ReceitaWSResponse>>("cnpjs");


              switch (filtroSituacao)
              {
                  case Situacao.Ativa:
                      listaCNPJValido = listaCNPJValido.Where(x => x.Situacao.Equals("ATIVA")).ToList();
                      break;
                  case Situacao.Baixada:
                      listaCNPJValido = listaCNPJValido.Where(x => x.Situacao.Equals("BAIXADA")).ToList();
                      break;
              }
              if (listaCNPJValido == null)
              {
                  return Results.Ok("Nenhuma empresa disponível.");
              }
              ReceitaWSResponse CNPJEncontrado = null;
              switch (filtroSocio)
              {
                  case FiltroSocio.Aleatorio:
                      CNPJEncontrado = listaCNPJValido.FirstOrDefault();
                      break;
                  case FiltroSocio.VariosSocios:
                      CNPJEncontrado = listaCNPJValido.FirstOrDefault(x => x.Qsa.Count > 1);
                      break;
                  case FiltroSocio.UnicoSocio:
                      CNPJEncontrado = listaCNPJValido.FirstOrDefault(x => x.Qsa != null && x.Qsa.Count == 1);
                      break;
              }

              if (CNPJEncontrado == null)
              {
                  return Results.Ok("Nenhuma empresa disponível para esse filtro.");
              }

              var removeIndex = listaCNPJValido.IndexOf(CNPJEncontrado);
              if (removeIndex != -1)
              {
                  redisService.ItemRemove<ReceitaWSResponse>("cnpjs", removeIndex);
              }
              if (normalizado)
              {
                  return Results.Ok(CNPJEncontrado.ObterResponseSalinizado());
              }
              return Results.Ok(CNPJEncontrado);
          }).WithTags("Geradores")
      .WithOpenApi(options =>
      {
          options.Summary = "Obtém um CNPJ aleatório ou filtrado e validado pela ReceitaWS";
          options.Description = "Obtém um CNPJ aleatório ou filtrado e validado pela ReceitaWS. Cada CNPJ gerado é excluído do cache. Certifique-se que há empresas cadastradas disponíveis.";
          return options;
      });

            app.MapPost("/placeholder", ([FromBody] JsonElement data) =>
            {
                var stringPlaceholder = new PlaceholderCreator();


                var dictionary = new Dictionary<string, string>();
                var resultado = "";

                if (data.ValueKind == JsonValueKind.Object)
                {
                    var enumerate = data.EnumerateObject();
                    foreach (JsonProperty p in enumerate)
                    {
                        if (p.Value.ValueKind == JsonValueKind.String)
                        {
                            dictionary.Add(p.Name, p.Value.GetString());
                        }
                    }
                    resultado = JsonSerializer.Serialize(dictionary);
                }
                else
                {
                    resultado = JsonSerializer.Serialize(data);
                }
                var placeHolders = PlaceHolders.ObterPlaceholders();
                var result = stringPlaceholder.Creator(resultado, placeHolders);
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
               descricaoPlaceholders += $"[{chave}] " +
                   "<br/>" +
                   $"Uso: {usoDescricao}" +
                   "<br/>" +
                   $"Descrição: {descricao}<br/>";
           }

           options.Summary = "Permite criar um json ou um string com o uso de placeholders e gerar dados aleatórios.";
           options.Description = $"{descricaoPlaceholders}";
           return options;
       });


        }
    }
}
