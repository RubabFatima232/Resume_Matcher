using ResumeMatcher.Models;

namespace ResumeMatcher.Services;

public class MockMatcherService : IAIMatcherService
{
    public Task<MatchResult> AnalyzeMatchAsync(Resume resume, Job job)
    {
        var random = new Random();
        var score = random.Next(55, 95);

        var allSkills = new[] { "C#", ".NET", "SQL Server", "Docker", "Redis", "Azure", "REST APIs", "Git", "Kubernetes", "React" };
        var matched = allSkills.Take(score > 75 ? 6 : 4).ToList();
        var missing = allSkills.Skip(score > 75 ? 6 : 4).Take(3).ToList();

        return Task.FromResult(new MatchResult
        {
            ResumeId = resume.Id,
            JobId = job.Id,
            MatchScore = score,
            MatchedSkills = string.Join(", ", matched),
            MissingSkills = string.Join(", ", missing),
            AISummary = $"{resume.CandidateName} is a {(score >= 75 ? "strong" : "moderate")} candidate for the {job.Title} role at {job.Company}. " +
                        $"The candidate demonstrates relevant technical expertise with a match score of {score}%. " +
                        $"Key strengths include {matched.Take(3).Aggregate((a, b) => a + ", " + b)}. " +
                        $"To improve the match score, consider gaining experience in {(missing.Any() ? missing.First() : "additional technologies")}. " +
                        $"Overall, this candidate {(score >= 70 ? "meets the core requirements" : "partially meets the requirements")} for this position.",
            Recommendation = score >= 80 ? "Highly Recommended" :
                             score >= 65 ? "Recommended" :
                             score >= 50 ? "Maybe" : "Not Recommended",
            AnalyzedAt = DateTime.UtcNow
        });
    }
}