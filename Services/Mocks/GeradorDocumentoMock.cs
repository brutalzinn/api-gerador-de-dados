using GeradorDeDados.Mocks.Utils;
using GeradorDeDados.Models;
using GeradorDeDados.Models.Response;
using System.IO;

namespace GeradorDeDados.Services.Mocks
{
    public class GeradorDocumentoMock : IGeradorDocumentoMock
    {
        public DocumentoMock GerarDocumento(NomeDocumento nomeDocumento)
        {
            var documento = Path.Combine("Mocks", "Documento", nomeDocumento.ObterDescricao());
            var documentoBase64 = ImageUtil.AddRandomDots(documento);
            return new DocumentoMock
            {
                TipoDocumento = nomeDocumento,
                Base64 = documentoBase64
            };
        }
    }
}
