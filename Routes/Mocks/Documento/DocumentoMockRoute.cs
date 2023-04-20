using GeradorDeDados.Mocks.Utils;
using GeradorDeDados.Models;
using GeradorDeDados.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace GeradorDeDados.Routes.Mocks.Documento
{
    public static class DocumentoMockRoute
    {
        public static void CriarRota(this WebApplication app)
        {
            app.MapGet("/mocks/obterdocumento/{TipoDocumento}",

            (
            [FromRoute] TipoDocumento tipoDocumento
                ) =>
        {
            var nomeDocumento = "";
            switch (tipoDocumento)
            {
                case TipoDocumento.RG_VERSO:
                    nomeDocumento = "verso.jpg";
                    break;
                case TipoDocumento.RG_FRENTE:
                    nomeDocumento = "frente.jpg";
                    break;
                default:
                    nomeDocumento = "selfie.jpg";
                    break;
            }

            var documento = Path.Combine("Mocks", "Documento", nomeDocumento);
            var documentoBase64 = ImageUtil.AddRandomDots(documento);
            return new DocumentoMock
            {
                TipoDocumento = tipoDocumento,
                Base64 = documentoBase64
            };
        }).WithTags("Mocks")
            .WithOpenApi(options =>
            {
                options.Summary = "Obtém uma imagem base64 de um documento com pontos aleatórios na imagem.";
                return options;
            });
        }





    }

}