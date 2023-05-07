using GeradorDeDados.Mocks.Utils;
using GeradorDeDados.Models;
using GeradorDeDados.Models.Response;
using System.Collections.Generic;
using System.IO;

namespace GeradorDeDados.Services.Mocks
{
    public class GeradorDocumentoMock : IGeradorDocumentoMock
    {
        private List<TipoDocumento> documentosImagem = new List<TipoDocumento> { TipoDocumento.RG_FRENTE, TipoDocumento.RG_VERSO, TipoDocumento.SELFIE };
        private List<TipoDocumento> documentosPDF = new List<TipoDocumento> { TipoDocumento.PDF };

        public DocumentoMock GerarDocumento(TipoDocumento tipoDocumento)
        {
            if (documentosImagem.Contains(tipoDocumento))
            {
                var imagemComPixelsRandom = GerarImagemComPixelsRandom(tipoDocumento);
                return imagemComPixelsRandom;
            }
            var documentoPdfRandom = GerarPDFComTamanhoDeTextoFixo(tipoDocumento);
            return documentoPdfRandom;
        }

        public DocumentoMock GerarImagemComPixelsRandom(TipoDocumento tipoDocumento)
        {
            var caminhoDocumento = Path.Combine("Mocks", "Documento", tipoDocumento.ObterDescricao() + ".jpg");
            var imagemBase64 = ImageUtil.GenerateRandomDots(caminhoDocumento);
            return new DocumentoMock(tipoDocumento, imagemBase64);
        }
        public DocumentoMock GerarPDFComTamanhoDeTextoFixo(TipoDocumento tipoDocumento, int loremIpsumEmKbs = 80)
        {
            var pdfBase64 = PdfUtil.Generate(loremIpsumEmKbs);
            return new DocumentoMock(tipoDocumento, pdfBase64);
        }
    }
}
