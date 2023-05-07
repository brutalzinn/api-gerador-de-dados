using GeradorDeDados.Models.Exceptions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;

namespace GeradorDeDados.Mocks.Utils
{
    public class ImageUtil
    {
        /// <summary>
        /// chat gpt me deu uma mãozinha aqui.
        /// </summary>
        public static string GenerateRandomDots(string imagePath)
        {
            if (File.Exists(imagePath) == false)
            {
                throw new CustomException(Models.TipoExcecao.NEGOCIO, "Arquivo não encontrado no caminho informado.");
            }
            using (var image = Image.Load<Rgba32>(imagePath))
            {
                Random random = new Random();
                int x = random.Next(image.Width);
                int y = random.Next(image.Height);
                image[x, y] = Color.Black;
                using (var memoryStream = new MemoryStream())
                {
                    image.Save(memoryStream, new JpegEncoder());
                    var base64String = Convert.ToBase64String(memoryStream.ToArray());
                    return base64String;
                }
            }
        }

    }
}