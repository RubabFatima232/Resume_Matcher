using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using DocumentFormat.OpenXml.Packaging;

namespace ResumeMatcher.Services;

public class FileParserService : IFileParserService
{
    public async Task<string> ExtractTextAsync(Stream fileStream, string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        return extension switch
        {
            ".pdf" => await ExtractFromPdfAsync(fileStream),
            ".docx" => await ExtractFromDocxAsync(fileStream),
            _ => throw new NotSupportedException($"File type '{extension}' is not supported.")
        };
    }

    private Task<string> ExtractFromPdfAsync(Stream stream)
    {
        var text = new System.Text.StringBuilder();

        using var pdfReader = new PdfReader(stream);
        using var pdfDoc = new PdfDocument(pdfReader);

        for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
        {
            text.AppendLine(PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i)));
        }

        return Task.FromResult(text.ToString());
    }

    private Task<string> ExtractFromDocxAsync(Stream stream)
    {
        using var doc = WordprocessingDocument.Open(stream, false);
        var text = doc.MainDocumentPart?.Document?.Body?.InnerText ?? string.Empty;
        return Task.FromResult(text);
    }
}