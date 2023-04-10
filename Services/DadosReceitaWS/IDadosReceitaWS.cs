using GeradorDeDados.Integrations.ReceitaWS;
using GeradorDeDados.Models;

namespace GeradorDeDados.Services
{
    public interface IDadosReceitaWS
    {
        ReceitaWSResponse ObterDadosEmpresaRegistrada(FiltroSocio filtroSocio, FiltroSituacao filtroSituacao, bool normalizado, bool excluirEmpresa);
    }
}
