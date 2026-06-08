using System.ComponentModel.DataAnnotations;

namespace ResumeMatcher.Models;

public class MatchResult
{
    public int Id { get; set; }

    public int ResumeId { get; set; }
    public Resume? Resume { get; set; }

    public int JobId { get; set; }
    public Job? Job { get; set; }

    [Range(0, 100)]
    public double MatchScore { get; set; }

    public string? MatchedSkills { get; set; }
    public string? MissingSkills { get; set; }
    public string? AISummary { get; set; }
    public string? Recommendation { get; set; }

    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
}