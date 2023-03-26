using Bogus;
using GeradorDeDados.Integrations.ReceitaWS;
using GeradorDeDados.Models;
using GeradorDeDados.Placeholder;
using GeradorDeDados.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestEase.Implementation;
using StringPlaceholder;
using System.Text.Json;

namespace GeradorDeDados.Routes.Geradores
{
    public static class GeradorRoute
    {
        public static void CriarRota(this WebApplication app)
        {
            app.MapGet("/obterCNPJValido/{filtroSocio}/{filtroSituacao}/{normalizado}/{excluirEmpresa}",
          [Authorize(AuthenticationSchemes = "ApiKey")]
            (
              [FromServices] DadosReceitaWS dadosReceitaWS,
              [FromRoute] FiltroSocio filtroSocio,
              [FromRoute] FiltroSituacao filtroSituacao,
              [FromRoute] bool normalizado,
              [FromRoute] bool excluirEmpresa) =>
          {
              var empresaReceitaWS = dadosReceitaWS.ObterDadosEmpresaRegistrada(filtroSocio, filtroSituacao, normalizado, excluirEmpresa);
              return Results.Ok(empresaReceitaWS);
          }).WithTags("Geradores")
              .WithOpenApi(options =>
              {
                  options.Summary = "Obtém um CNPJ aleatório ou filtrado e validado pela ReceitaWS";
                  options.Description = "Obtém um CNPJ aleatório ou filtrado e validado pela ReceitaWS. Cada CNPJ gerado é excluído do cache. Certifique-se que há empresas cadastradas disponíveis.";
                  return options;
              });
        }
    }
}
