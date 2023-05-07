using GeradorDeDados.Models;
using GeradorDeDados.Models.Response;

namespace GeradorDeDados.Services.Mocks
{
    public interface IGeradorDocumentoMock
    {
        DocumentoMock GerarDocumento(TipoDocumento tipoDocumento);
        DocumentoMock GerarImagemComPixelsRandom(TipoDocumento tipoDocumento);
        DocumentoMock GerarPDFComTamanhoDeTextoFixo(TipoDocumento tipoDocumento, int loremIpsumEmKbs);

    }
}
