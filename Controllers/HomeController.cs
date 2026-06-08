using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ResumeMatcher.Controllers
{
    public class JobSpaceModel
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public List<string> Skills { get; set; } = new List<string>();
        public string HighRules { get; set; } = string.Empty;
        public string LowRules { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class ResumeModel
    {
        public string Id { get; set; } = string.Empty;
        public string CandidateName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> ExtractedSkills { get; set; } = new List<string>();
        public string AppliedJobId { get; set; } = string.Empty;
        public string AppliedJobTitle { get; set; } = string.Empty;
        public int MatchScore { get; set; }
    }

    public class HomeController : Controller
    {
        private static readonly List<JobSpaceModel> StoredJobs = new List<JobSpaceModel>
        {
            new JobSpaceModel { Id = "J1", Title = "Senior C# Developer", Skills = new List<string> { "C#", "ASP.NET Core", "SQL Server" }, HighRules = "Deep semantic execution of architectures.", LowRules = "Basic object modeling.", CreatedDate = DateTime.Now },
            new JobSpaceModel { Id = "J2", Title = "DevOps Engineer", Skills = new List<string> { "Docker", "Kubernetes", "Linux" }, HighRules = "Advanced cluster design.", LowRules = "Basic linux profiling.", CreatedDate = DateTime.Now }
        };

        private static readonly List<ResumeModel> StoredResumes = new List<ResumeModel>
        {
            new ResumeModel { Id = "R1", CandidateName = "Ali Ahmed", Email = "ali@example.com", ExtractedSkills = new List<string>{"C#", "SQL Server"}, AppliedJobId = "J1", AppliedJobTitle = "Senior C# Developer", MatchScore = 66 },
            new ResumeModel { Id = "R2", CandidateName = "Zainab Khan", Email = "zainab@example.com", ExtractedSkills = new List<string>{"Docker", "Linux", "Kubernetes"}, AppliedJobId = "J2", AppliedJobTitle = "DevOps Engineer", MatchScore = 100 }
        };

        public IActionResult Index()
        {
            ViewBag.TotalJobs = StoredJobs.Count;
            ViewBag.TotalResumes = StoredResumes.Count;
            return View();
        }

        public IActionResult Jobs()
        {
            ViewBag.JobsList = StoredJobs ?? new List<JobSpaceModel>();
            return View();
        }

        [HttpPost]
        public IActionResult AddJob(string title, string skills, string highRules, string lowRules)
        {
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(skills))
            {
                var coreSkills = skills.Split(',')
                                       .Select(s => s.Trim())
                                       .Where(s => !string.IsNullOrEmpty(s))
                                       .ToList();

                StoredJobs.Add(new JobSpaceModel
                {
                    Id = "J" + (StoredJobs.Count + 1),
                    Title = title.Trim(),
                    Skills = coreSkills,
                    HighRules = string.IsNullOrEmpty(highRules) ? "N/A" : highRules.Trim(),
                    LowRules = string.IsNullOrEmpty(lowRules) ? "N/A" : lowRules.Trim(),
                    CreatedDate = DateTime.Now
                });
            }
            return RedirectToAction("Jobs");
        }

        public IActionResult Resumes()
        {
            ViewBag.JobsList = StoredJobs ?? new List<JobSpaceModel>();
            ViewBag.ResumesList = StoredResumes ?? new List<ResumeModel>();
            return View();
        }

        [HttpPost]
        public IActionResult AddResume(string candidateName, string email, string skills, string appliedJobId)
        {
            if (string.IsNullOrEmpty(candidateName) || string.IsNullOrEmpty(appliedJobId))
                return RedirectToAction("Resumes");

            var targetJob = StoredJobs.FirstOrDefault(j => j.Id == appliedJobId);
            if (targetJob == null) return RedirectToAction("Resumes");

            var candidateSkills = string.IsNullOrEmpty(skills)
                ? new List<string>()
                : skills.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();

            int score = 0;
            if (targetJob.Skills != null && targetJob.Skills.Count > 0)
            {
                int matchedCount = candidateSkills.Intersect(targetJob.Skills, StringComparer.OrdinalIgnoreCase).Count();
                score = (int)((double)matchedCount / targetJob.Skills.Count * 100);
            }
            if (score > 100) score = 100;

            StoredResumes.Add(new ResumeModel
            {
                Id = "R" + (StoredResumes.Count + 1),
                CandidateName = candidateName.Trim(),
                Email = string.IsNullOrEmpty(email) ? "no-email@domain.com" : email.Trim(),
                ExtractedSkills = candidateSkills,
                AppliedJobId = appliedJobId,
                AppliedJobTitle = targetJob.Title,
                MatchScore = score
            });

            return RedirectToAction("Resumes");
        }

        public IActionResult Match()
        {
            ViewBag.ResumesList = StoredResumes ?? new List<ResumeModel>();
            return View();
        }

        [HttpGet]
        public JsonResult GetReportData(string resumeId)
        {
            var resume = StoredResumes.FirstOrDefault(r => r.Id == resumeId);
            if (resume == null) return Json(new { success = false });

            var job = StoredJobs.FirstOrDefault(j => j.Id == resume.AppliedJobId);
            string rulesToExecute = (resume.MatchScore >= 80) ? (job?.HighRules ?? "N/A") : (job?.LowRules ?? "N/A");

            int syntaxScore = resume.MatchScore;
            int architectureScore = Math.Max(10, resume.MatchScore - 15);
            int domainScore = Math.Min(100, resume.MatchScore + 10);

            return Json(new
            {
                success = true,
                candidateName = resume.CandidateName ?? "Unknown",
                jobTitle = resume.AppliedJobTitle ?? "Not Assigned",
                score = resume.MatchScore,
                reasoning = rulesToExecute,
                skills = resume.ExtractedSkills ?? new List<string>(),
                chartData = new[] { syntaxScore, architectureScore, domainScore }
            });
        }

        [HttpGet]
        public JsonResult GetAIRules(string title, string skills)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(skills)) return Json(new { success = false });
            return Json(new
            {
                success = true,
                high = $"Deep semantic execution of architectures deploying {skills}.",
                low = $"Basic object modeling and setup logic using {skills} layers."
            });
        }
    }
}