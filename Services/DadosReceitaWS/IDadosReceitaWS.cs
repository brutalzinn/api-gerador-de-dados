using GeradorDeDados.Integrations.ReceitaWS;
using GeradorDeDados.Models;
using GeradorDeDados.Models.Response;

namespace GeradorDeDados.Services
{
    public interface IDadosReceitaWS
    {
        ReceitaWSResponse ObterDadosEmpresaRegistrada(FiltroSocio filtroSocio, FiltroSituacao filtroSituacao, bool normalizado, bool excluirEmpresa);
        DadosEmpresasRegistradas ObterDadosEmpresasRegistradas();
    }
}
