using System.ComponentModel.DataAnnotations;

namespace ResumeMatcher.Models;

public class Job
{
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(300)]
    public string Company { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public string? RequiredSkills { get; set; }
    public string? RequiredExperience { get; set; }
    public string? Location { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public ICollection<MatchResult> MatchResults { get; set; } = new List<MatchResult>();
}