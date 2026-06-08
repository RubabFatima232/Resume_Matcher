namespace ResumeMatcher.Services;

public interface IFileParserService
{
    Task<string> ExtractTextAsync(Stream fileStream, string fileName);
}