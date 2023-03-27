using GeradorDeDados.Integrations.ReceitaWS;
using GeradorDeDados.Models;

namespace GeradorDeDados.Services
{
    public class DadosReceitaWS
    {
        private IRedisService redisService { get; set; }
        public DadosReceitaWS(IRedisService redisService)
        {
            this.redisService = redisService;
        }
        /// <summary>
        /// Obtem uma empresa da lista de cache do redis.
        /// </summary>
        /// <param name="filtroSocio">Tipo do sócio</param>
        /// <param name="filtroSituacao">Tipo da situação da empresa</param>
        /// <param name="normalizado">Normalização do dado recebido.</param>
        /// <param name="excluirCache">Deve excluir o item do cache?</param>
        /// <returns></returns>
        /// <exception cref="CustomException"></exception>
        public ReceitaWSResponse ObterDadosEmpresaRegistrada(FiltroSocio filtroSocio, FiltroSituacao filtroSituacao, bool normalizado, bool excluirEmpresa)
        {
            var listaCNPJValido = redisService.Get<List<ReceitaWSResponse>>("cnpjs");
            List<ReceitaWSResponse> listaEmpresas = new List<ReceitaWSResponse>();
            ReceitaWSResponse empresaSelecionada = null;

            if (listaCNPJValido == null)
            {
                throw new CustomException(TipoExcecao.NEGOCIO, "Não há empresas disponíveis");
            }
            switch (filtroSituacao)
            {
                case FiltroSituacao.Ativa:
                    listaEmpresas = listaEmpresas.Where(x => x.Situacao.Equals("ATIVA")).ToList();
                    break;
                case FiltroSituacao.Baixada:
                    listaEmpresas = listaEmpresas.Where(x => x.Situacao.Equals("BAIXADA")).ToList();
                    break;
            }
            switch (filtroSocio)
            {
                case FiltroSocio.VariosSocios:
                    listaEmpresas = listaEmpresas.Where(x => x.Qsa.Count > 1).TakeLast(5).ToList();
                    break;
                case FiltroSocio.UnicoSocio:
                    listaEmpresas = listaEmpresas.Where(x => x.Qsa != null && x.Qsa.Count == 1).TakeLast(5).ToList();
                    break;
            }
            if(listaEmpresas.Count() == 0)
            {
                throw new CustomException(TipoExcecao.NEGOCIO, "Não há empresas disponíveis para esse filtro.");
            }
            var randomIndexEmpresa = new Random().Next(0, listaEmpresas.Count() - 1);
            empresaSelecionada = listaEmpresas[randomIndexEmpresa];
            if (excluirEmpresa)
            {
                redisService.ItemRemove<ReceitaWSResponse>("cnpjs", randomIndexEmpresa);
            }
            if (normalizado)
            {
                return empresaSelecionada.ObterResponseSalinizado();
            }
            return empresaSelecionada;
        }
    }
}
