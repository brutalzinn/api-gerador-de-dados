using GeradorDeDados.Integrations.ReceitaWS.Models;
using GeradorDeDados.Models;
using GeradorDeDados.Models.Exceptions;
using GeradorDeDados.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeradorDeDados.Services
{
    public class DadosReceitaWS : IDadosReceitaWS
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
            var listaEmpresas = new List<ReceitaWSResponse>();
            var listaEmpresasCache = redisService.Get<List<ReceitaWSResponse>>("cnpjs");
            ReceitaWSResponse empresaSelecionada = null;
            if (listaEmpresasCache == null || listaEmpresasCache.Count() == 0)
            {
                throw new CustomException(TipoExcecao.NEGOCIO, "Não há empresas disponíveis.");
            }
            switch (filtroSituacao)
            {
                case FiltroSituacao.Ativa:
                    listaEmpresas = listaEmpresasCache.Where(x => x.Situacao.ToLower().Equals("ativa")).ToList();
                    break;
                case FiltroSituacao.Baixada:
                    listaEmpresas = listaEmpresasCache.Where(x => x.Situacao.ToLower().Equals("baixada")).ToList();
                    break;
                default:
                    listaEmpresas = listaEmpresasCache;
                    break;
            }
            switch (filtroSocio)
            {
                case FiltroSocio.VariosSocios:
                    listaEmpresas = listaEmpresasCache.Where(x => x.Qsa.Count() > 1).ToList();
                    break;
                case FiltroSocio.UnicoSocio:
                    listaEmpresas = listaEmpresasCache.Where(x => x.Qsa != null && x.Qsa.Count() == 1).ToList();
                    break;
                default:
                    listaEmpresas = listaEmpresasCache;
                    break;
            }
            if (listaEmpresas.Count() == 0)
            {
                throw new CustomException(TipoExcecao.NEGOCIO, "Não há empresas disponíveis para esse filtro.");
            }
            var randomIndexEmpresa = new Random().Next(0, listaEmpresas.Count() - 1);
            empresaSelecionada = listaEmpresas[randomIndexEmpresa];
            if (excluirEmpresa)
            {
                var excluirEmpresaIndex = listaEmpresasCache.IndexOf(empresaSelecionada);
                redisService.ItemRemove<ReceitaWSResponse>("cnpjs", excluirEmpresaIndex);
            }
            if (normalizado)
            {
                return empresaSelecionada.ObterRespostaNormalizada();
            }
            return empresaSelecionada;
        }

        public DadosEmpresasRegistradas ObterDadosEmpresasRegistradas()
        {
            int quantidadeCnpjs = 0;
            int quantidadeUnicoSocio = 0;
            int quantidadeVariosSocios = 0;
            var listaEmpresasCache = redisService.Get<List<ReceitaWSResponse>>("cnpjs");
            if (listaEmpresasCache != null)
            {
                quantidadeCnpjs = listaEmpresasCache.Count();
                quantidadeUnicoSocio = listaEmpresasCache.Count(x => x.Qsa != null && x.Qsa.Count() == 1);
                quantidadeVariosSocios = listaEmpresasCache.Count(x => x.Qsa.Count() > 1);
            }
            return new DadosEmpresasRegistradas
            {
                QuantidadeCnpjs = quantidadeCnpjs,
                QuantidadeUnicoSocio = quantidadeUnicoSocio,
                QuantidadeVariosSocios = quantidadeVariosSocios
            };
        }
    }
}
