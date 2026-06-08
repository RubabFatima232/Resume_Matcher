namespace ResumeMatcher.ViewModels;

public class DashboardViewModel
{
    public int TotalResumes { get; set; }
    public int TotalJobs { get; set; }
    public int TotalMatches { get; set; }
    public double AverageMatchScore { get; set; }
    public List<RecentMatchItem> RecentMatches { get; set; } = new();
}

public class RecentMatchItem
{
    public string CandidateName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public double Score { get; set; }
    public DateTime AnalyzedAt { get; set; }
}