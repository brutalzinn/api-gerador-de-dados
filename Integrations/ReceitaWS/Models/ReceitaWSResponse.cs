using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeradorDeDados.Integrations.ReceitaWS.Models
{
    public class ReceitaWSResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("ultima_atualizacao")]
        public DateTime UltimaAtualizacao { get; set; }

        [JsonProperty("cnpj")]
        public string Cnpj { get; set; }

        [JsonProperty("tipo")]
        public string Tipo { get; set; }

        [JsonProperty("porte")]
        public string Porte { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("fantasia")]
        public string Fantasia { get; set; }

        [JsonProperty("abertura")]
        public string Abertura { get; set; }

        [JsonProperty("atividade_principal")]
        public List<AtividadePrincipal> AtividadePrincipal { get; set; }

        [JsonProperty("atividades_secundarias")]
        public List<AtividadesSecundaria> AtividadesSecundarias { get; set; }

        [JsonProperty("natureza_juridica")]
        public string NaturezaJuridica { get; set; }

        [JsonProperty("logradouro")]
        public string Logradouro { get; set; }

        [JsonProperty("numero")]
        public string Numero { get; set; }

        [JsonProperty("complemento")]
        public string Complemento { get; set; }

        [JsonProperty("cep")]
        public string Cep { get; set; }

        [JsonProperty("bairro")]
        public string Bairro { get; set; }

        [JsonProperty("municipio")]
        public string Municipio { get; set; }

        [JsonProperty("uf")]
        public string Uf { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("telefone")]
        public string Telefone { get; set; }

        [JsonProperty("efr")]
        public string Efr { get; set; }

        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        [JsonProperty("data_situacao")]
        public string DataSituacao { get; set; }

        [JsonProperty("motivo_situacao")]
        public string MotivoSituacao { get; set; }

        [JsonProperty("situacao_especial")]
        public string SituacaoEspecial { get; set; }

        [JsonProperty("data_situacao_especial")]
        public string DataSituacaoEspecial { get; set; }

        [JsonProperty("capital_social")]
        public string CapitalSocial { get; set; }

        [JsonProperty("qsa")]
        public List<Qsa> Qsa { get; set; }

        [JsonProperty("billing")]
        public Billing Billing { get; set; }

        public ReceitaWSResponse ObterRespostaNormalizada()
        {
            return new ReceitaWSResponse()
            {
                Abertura = Abertura.Normalizar(),
                Situacao = Situacao.Normalizar(),
                SituacaoEspecial = SituacaoEspecial.Normalizar(),
                Efr = Efr.Normalizar(),
                Cnpj = Cnpj.ObterSomenteNumeros(),
                NaturezaJuridica = NaturezaJuridica.ObterSomenteNumeros(),
                Tipo = Tipo.Normalizar(),
                Cep = Cep.ObterSomenteNumeros(),
                MotivoSituacao = string.IsNullOrEmpty(MotivoSituacao) ? "" : MotivoSituacao.Split(" ")[0],
                Status = Status.Normalizar(),
                Porte = Porte.Normalizar(),
                Fantasia = Fantasia.Normalizar(),
                Logradouro = Logradouro.Normalizar(),
                Numero = Numero.Normalizar(),
                Complemento = Complemento.Normalizar(),
                Bairro = Bairro.Normalizar(),
                Municipio = Municipio.Normalizar(),
                Uf = Uf.Normalizar(),
                Email = Email,
                Telefone = Telefone.Replace(" ", ""),
                DataSituacao = DataSituacao,
                DataSituacaoEspecial = DataSituacaoEspecial,
                CapitalSocial = CapitalSocial,
                Qsa = Qsa,
                Billing = Billing,
                Nome = Nome,
                AtividadePrincipal = AtividadePrincipal.Select(x =>
                {
                    x.Code = x.Code.ObterSomenteNumeros();
                    x.Text = x.Text.Normalizar();
                    return x;
                }).ToList(),
                AtividadesSecundarias = AtividadesSecundarias.Select(x =>
                {
                    x.Code = x.Code.ObterSomenteNumeros();
                    x.Text = x.Text.Normalizar();
                    return x;
                }).ToList(),


            };
        }

    }

    public class AtividadePrincipal
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class AtividadesSecundaria
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class Billing
    {
        [JsonProperty("free")]
        public bool Free { get; set; }

        [JsonProperty("database")]
        public bool Database { get; set; }
    }

    public class Qsa
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("qual")]
        public string Qual { get; set; }

        [JsonProperty("pais_origem")]
        public string PaisOrigem { get; set; }

        [JsonProperty("nome_rep_legal")]
        public string NomeRepLegal { get; set; }

        [JsonProperty("qual_rep_legal")]
        public string QualRepLegal { get; set; }
    }
}
