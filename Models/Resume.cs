using System.ComponentModel.DataAnnotations;

namespace ResumeMatcher.Models;

public class Resume
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string CandidateName { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    public string ExtractedText { get; set; } = string.Empty;

    public string? Skills { get; set; }
    public string? Experience { get; set; }
    public string? Education { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public ICollection<MatchResult> MatchResults { get; set; } = new List<MatchResult>();
}