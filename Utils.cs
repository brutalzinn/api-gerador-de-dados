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

                if (Char.IsDigit(letra))
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
    }
}
