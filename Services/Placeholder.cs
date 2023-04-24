using Bogus;
using Bogus.Extensions;
using Bogus.Extensions.Brazil;
using StringPlaceholder;
using StringPlaceholder.FluentPattern;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace GeradorDeDados.Services
{
    public class Placeholder
    {
        private readonly IExecutorCreator executorCreator;

        public Placeholder(IExecutorCreator executorCreator)
        {
            this.executorCreator = executorCreator;
        }

        public string ObterTexto(string texto)
        {
            var novoTexto = executorCreator.Build(texto)
                .Result();
            return novoTexto;
        }

        public string ObterDescricao()
        {
            var descricaoPlaceholders = "Placeholders disponíveis:<br/><br/>";
            var descricoes = executorCreator.GetDescription();
            foreach (var item in descricoes)
            {
                var aceitaParametros = item.MultipleParams;
                var chave = item.Key;
                var descricao = item.Description;
                var args = item.Args;
                var listaParametros = string.Join(",", args);
                var casoDeUso = $"[{chave}]";
                if (aceitaParametros)
                {
                    casoDeUso = $"[{chave}({listaParametros})]";
                }
                descricaoPlaceholders += $"{chave}" +
                    "<br/>" +
                    $"Uso: {casoDeUso}" +
                    "<br/>" +
                    $"Descrição: {descricao}<br/>" +
                    "<br/>";

            }
            return descricaoPlaceholders;
        }

        public static List<StringExecutor> ObterExecutores(IDadosReceitaWS dadosReceitaWS)
        {
            Faker faker = new Faker();
            return new List<StringExecutor>()
            {
                new StringExecutor("GUID", ()=> Guid.NewGuid().ToString(), "Cria um UUID padrão"),
                new StringExecutor("LOGRADOURO", ()=> faker.Address.StreetAddress(false), "Cria um logradouro"),
                new StringExecutor("CEP", ()=> faker.Address.ZipCode("########"), "Cria um cep"),
                new StringExecutor("EMAIL", ()=> faker.Person.Email.ToLower(), "Cria um email"),
                new StringExecutor("CIDADE", ()=> faker.Address.City(), "Cria uma cidade"),
                new StringExecutor("BAIRRO", ()=> faker.Address.City(), "Cria um bairro"),
                new StringExecutor("DINHEIRO", ()=>  faker.Random.Decimal2(100,5000).ToString("0.##")),
                new StringExecutor("NOME_COMPLETO", ()=> faker.Name.FullName(), "Cria um nome completo de pessoa"),
                new StringExecutor("RG", ()=> faker.Random.String2(8, "0123456789"), "Cria um RG"),
                new StringExecutor("CPF", ()=> faker.Person.Cpf(false), "Cria um CPF"),
                new StringExecutor("CNPJ",()=> faker.Company.Cnpj(false), "Cria um CNPJ"),
                new StringExecutor("CNPJ_RECEITAWS_QSA_UM", () => dadosReceitaWS.ObterDadosEmpresaRegistrada(Models.FiltroSocio.UnicoSocio, Models.FiltroSituacao.Ativa, true, false).Cnpj, "Cria um CNPJ usando a ReceitaWS com único sócio"),
                new StringExecutor("CNPJ_RECEITAWS_QSA_VARIOS", () => dadosReceitaWS.ObterDadosEmpresaRegistrada(Models.FiltroSocio.VariosSocios, Models.FiltroSituacao.Ativa, true, false).Cnpj, "Cria um CNPJ usando a ReceitaWS com mais de um sócio"),
                new StringExecutor("CNPJ_RECEITAWS_ALEATORIO", () => dadosReceitaWS.ObterDadosEmpresaRegistrada(Models.FiltroSocio.VariosSocios, Models.FiltroSituacao.Ativa, true, false).Cnpj, "Cria um CNPJ usando a ReceitaWS com mais de um sócio"),
                new StringExecutor("COMPANY_NAME",()=> faker.Company.CompanyName(), "Cria um nome de empresa aleatório."),
                new StringExecutor("FILE_URL_BASE64", GetFileUrlBase64, "Cria um base64 dado a url de um arquivo como parâmetro.",
                new List<string>()
                {
                "imageUrl"
                })
            };
        }
        private static string GetFileUrlBase64(string[] strParams)
        {
            using (var handler = new HttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                var bytes = client.GetByteArrayAsync(strParams[0]).Result;
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
