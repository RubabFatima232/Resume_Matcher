using ResumeMatcher.Models;

namespace ResumeMatcher.Services;

public interface IAIMatcherService
{
    Task<MatchResult> AnalyzeMatchAsync(Resume resume, Job job);
}