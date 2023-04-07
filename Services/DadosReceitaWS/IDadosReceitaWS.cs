using GeradorDeDados.Integrations.ReceitaWS;
using GeradorDeDados.Models;

namespace GeradorDeDados.Services.DadosReceitaWS
{
    public interface IDadosReceitaWS
    {
        public ReceitaWSResponse ObterDadosEmpresaRegistrada(FiltroSocio filtroSocio, FiltroSituacao filtroSituacao, bool normalizado, bool excluirEmpresa);
    }
}
