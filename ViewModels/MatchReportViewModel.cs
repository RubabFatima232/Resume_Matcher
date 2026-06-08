using ResumeMatcher.Models;

namespace ResumeMatcher.ViewModels;

public class MatchReportViewModel
{
    public Resume Resume { get; set; } = null!;
    public Job Job { get; set; } = null!;
    public MatchResult Result { get; set; } = null!;
    public List<string> MatchedSkillsList { get; set; } = new();
    public List<string> MissingSkillsList { get; set; } = new();
}