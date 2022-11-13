using RestEase;

namespace GeradorDeDados.Integrations.ReceitaWS
{
    public interface IReceitaWS
    {
        [Get("v1/cnpj/{cnpj}")]
        Task<ReceitaWSResponse> ObterDadoEmpresa([Path] string cnpj);
    }
}
