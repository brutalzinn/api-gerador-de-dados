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
        public ReceitaWSResponse ObterDadosEmpresaRegistrada(FiltroSocio filtroSocio, FiltroSituacao filtroSituacao, bool normalizado, bool excluirEmpresa = false)
        {
            var listaCNPJValido = redisService.Get<List<ReceitaWSResponse>>("cnpjs");
            ReceitaWSResponse CNPJEncontrado = null;

            if (listaCNPJValido == null)
            {
                throw new CustomException(TipoExcecao.NEGOCIO, "Não há empresas disponíveis");
            }
            switch (filtroSituacao)
            {
                case FiltroSituacao.Ativa:
                    listaCNPJValido = listaCNPJValido.Where(x => x.Situacao.Equals("ATIVA")).ToList();
                    break;
                case FiltroSituacao.Baixada:
                    listaCNPJValido = listaCNPJValido.Where(x => x.Situacao.Equals("BAIXADA")).ToList();
                    break;
            }
            switch (filtroSocio)
            {
                case FiltroSocio.Aleatorio:
                    CNPJEncontrado = listaCNPJValido.FirstOrDefault();
                    break;
                case FiltroSocio.VariosSocios:
                    CNPJEncontrado = listaCNPJValido.FirstOrDefault(x => x.Qsa.Count > 1);
                    break;
                case FiltroSocio.UnicoSocio:
                    CNPJEncontrado = listaCNPJValido.FirstOrDefault(x => x.Qsa != null && x.Qsa.Count == 1);
                    break;
            }
            if(CNPJEncontrado == null)
            {
                throw new CustomException(TipoExcecao.NEGOCIO, "Não há empresas disponíveis para esse filtro.");
            }
            if (excluirEmpresa)
            {
                var removeIndex = listaCNPJValido.IndexOf(CNPJEncontrado);
                redisService.ItemRemove<ReceitaWSResponse>("cnpjs", removeIndex);
            }
            if (normalizado)
            {
                return CNPJEncontrado.ObterResponseSalinizado();
            }
            return CNPJEncontrado;
        }
    }
}
