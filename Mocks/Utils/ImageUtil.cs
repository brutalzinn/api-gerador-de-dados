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
        /// thanks chat gpt!
        /// </summary>
        public static string AddRandomDots(string imagePath)
        {
            // Load the image file using ImageSharp
            using (var image = Image.Load<Rgba32>(imagePath))
            {
                // Generate some random pixels to image

                Random random = new Random();
                int x = random.Next(image.Width);
                int y = random.Next(image.Height);
                image[x, y] = Color.Black;

                // Save the modified image to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    image.Save(memoryStream, new JpegEncoder());

                    // Convert the memory stream to a base64 string
                    var base64String = Convert.ToBase64String(memoryStream.ToArray());

                    // Return the base64 string
                    return base64String;
                }
            }
        }

    }
}