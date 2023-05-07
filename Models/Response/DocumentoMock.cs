namespace GeradorDeDados.Models.Response
{
    public class DocumentoMock
    {
        public TipoDocumento TipoDocumento { get; set; }
        public string Base64 { get; set; }
        public DocumentoMock(TipoDocumento tipoDocumento, string base64)
        {
            TipoDocumento = tipoDocumento;
            Base64 = base64;
        }
    }
}
