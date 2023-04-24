using GeradorDeDados.Integrations.ReceitaWS.Models;
using RestEase;
using System.Threading.Tasks;

namespace GeradorDeDados.Integrations.ReceitaWS
{
    public interface IReceitaWS
    {
        [Get("v1/cnpj/{cnpj}")]
        Task<ReceitaWSResponse> ObterDadoEmpresa([Path] string cnpj);
    }
}
