using GeradorDeDados.Models;
using GeradorDeDados.Models.Response;

namespace GeradorDeDados.Services.Mocks
{
    public interface IGeradorDocumentoMock
    {
        DocumentoMock GerarDocumento(NomeDocumento tipoDocumento);
    }
}
