using GeradorDeDados.Models.Exceptions;
using System;
using System.ComponentModel;
using System.Linq;

namespace GeradorDeDados
{
    public static class Utils
    {
        public static string ObterSomenteNumeros(this string texto)
        {
            if (string.IsNullOrEmpty(texto))
            {
                return "";
            }

            var resultado = "";
            for (var i = 0; i < texto.Count(); i++)
            {
                var letra = texto[i];

                if (char.IsDigit(letra))
                {
                    resultado += letra;
                }
            }

            return resultado;
        }

        public static string ToCasoBaixo(this string texto)
        {
            return texto.ToLower();
        }
        public static string Normalizar(this string texto, bool casoBaixo = true)
        {
            if (casoBaixo)
            {
                texto = texto.ToCasoBaixo();
            }
            return texto.RemoverAcentos();
        }
        private static string RemoverAcentos(this string texto)
        {
            string comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇçÑñ";
            string semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCcNn";

            for (int i = 0; i < comAcentos.Length; i++)
            {
                texto = texto.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());
            }
            return texto;
        }

        public static string ObterDescricao(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null)
            {
                throw new CustomException(Models.TipoExcecao.NEGOCIO, $"Não foi possível obter esse enum.");
            }
            var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

        public static T ObterEnumPorDescricao<T>(string description) where T : Enum
        {
            var enumType = typeof(T);
            var matchingValue = Enum.GetValues(enumType).Cast<T>().FirstOrDefault(enumValue =>
            {
                var attribute = enumType.GetField(enumValue.ToString())
                                         .GetCustomAttributes(typeof(DescriptionAttribute), false)
                                         .Cast<DescriptionAttribute>()
                                         .SingleOrDefault();

                return attribute != null && attribute.Description.Equals(description, StringComparison.OrdinalIgnoreCase);
            });
            return matchingValue ?? throw new CustomException(Models.TipoExcecao.NEGOCIO, $"Não existe {enumType.Name} com essa descrição:'{description}'");
        }
    }
}
