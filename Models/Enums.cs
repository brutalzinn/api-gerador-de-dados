using System.ComponentModel;

namespace GeradorDeDados.Models
{
    public enum FiltroSocio
    {
        Aleatorio = 0,
        UnicoSocio,
        VariosSocios
    }
    public enum FiltroSituacao
    {
        Aleatorio = 0,
        Ativa,
        Baixada
    }
    public enum TipoExcecao
    {
        VALIDACAO = 400,
        NEGOCIO = 406,
        NOTFOUND = 404,
        AUTORIZACAO = 401,
        INTERNO = 500
    }
    public enum NomeDocumento
    {
        [Description("selfie.jpg")]
        SELFIE = 1,
        [Description("rg_frente.jpg")]
        RG_FRENTE,
        [Description("rg_verso.jpg")]
        RG_VERSO,
        [Description("lorem-ipsum.pdf")]
        PDF
    }
}
