using GeradorDeDados.Models;
using GeradorDeDados.Services.Mocks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeradorDeDados.Routes.Mocks.Documento
{
    public static class DocumentoMockRoute
    {
        public static void CriarRota(this WebApplication app)
        {
            app.MapGet("/mocks/obterdocumento/{TipoDocumento}",
            ([FromRoute] TipoDocumento TipoDocumento, IGeradorDocumentoMock geradorDocumentoMock) =>
            {
                var documento = geradorDocumentoMock.GerarDocumento(TipoDocumento);
                return documento;
            }).WithTags("Mocks")
            .WithOpenApi(options =>
            {
                options.Summary = "Obtém uma imagem base64 de um documento com pontos aleatórios na imagem.";
                return options;
            });
        }





    }

}