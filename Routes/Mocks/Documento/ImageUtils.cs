

using SixLabors.ImageSharp.Formats.Png;

namespace DocumentoMock
{
    public class ImageUtils
    {
        /// <summary>
        ///     thanks chat gpt!
        /// </summary>
        public static string AddRandomDotToImage(string imagePath)
        {
            // Load the image file using ImageSharp
            using (var image = Image.Load<Rgba32>(imagePath))
            {
                // Generate a random x-coordinate and y-coordinate within the image
                Random random = new Random();
                int x = random.Next(image.Width);
                int y = random.Next(image.Height);

                // Set the pixel at the specified x-coordinate and y-coordinate to black
                image[x, y] = Color.Black;

                // Save the modified image to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    image.Save(memoryStream, new PngEncoder());

                    // Convert the memory stream to a base64 string
                    var base64String = Convert.ToBase64String(memoryStream.ToArray());

                    // Return the base64 string
                    return base64String;
                }
            }
        }

    }
}