using System;
using System.IO;
using Bogus;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Drawing;
using System.Text;
using PdfSharpCore.Drawing.Layout;

public class PdfUtil
{
    public static string Generate(int loremIpsumSizeInKbs = 80)
    {
        var dataAtual = DateTime.Now.ToString().Replace("/", "-").Replace(" ", "-");
        var caminhoPdf = Path.Combine("Mocks", "Documento", "tmp_" + dataAtual + ".pdf");
        PdfDocument documento = new PdfDocument();
        PdfPage page = documento.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);
        XFont titleFont = new XFont("Arial", 14);
        XFont font = new XFont("Arial", 12);
        XTextFormatter textFormatter = new XTextFormatter(gfx);
        XRect layoutRect = new XRect(10, 70, page.Width.Point - 20, page.Height.Point - 80);
        XStringFormat format = new XStringFormat
        {
            Alignment = XStringAlignment.Near,
            LineAlignment = XLineAlignment.Near
        };
        var loremIpsumText = GetLoremIpsumText(loremIpsumSizeInKbs);
        var titleText = $"Test PDF with lorem ipsum of {loremIpsumSizeInKbs} KB";
        gfx.DrawString(titleText, titleFont, XBrushes.Black, new XRect(10, 10, page.Width.Point - 20, 50), XStringFormats.Center);
        textFormatter.DrawString(loremIpsumText, font, XBrushes.Black, layoutRect, format);
        documento.Save(caminhoPdf);
        var fileByte = File.ReadAllBytes(caminhoPdf);
        var base64String = Convert.ToBase64String(fileByte);
        File.Delete(caminhoPdf);
        return base64String;
    }

    private static string GetLoremIpsumText(int fileSizeInKb)
    {
        var _faker = new Faker("pt_BR");
        int loremIpsumLength = fileSizeInKb * 1024;
        var loremIpsumText = _faker.Lorem.Text();
        while (loremIpsumText.Length < loremIpsumLength)
        {
            loremIpsumText += " " + _faker.Lorem.Text();
        }
        loremIpsumText = loremIpsumText.Substring(0, loremIpsumLength);
        loremIpsumText = loremIpsumText.Replace(". ", ".\n");
        return loremIpsumText;
    }
}