using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeMatcher.Models;
using ResumeMatcher.Services;
using ResumeMatcher.ViewModels;

namespace ResumeMatcher.Controllers;

public class MatchController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IAIMatcherService _matcher;

    public MatchController(ApplicationDbContext db, IAIMatcherService matcher)
    {
        _db = db;
        _matcher = matcher;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Resumes = await _db.Resumes.ToListAsync();
        ViewBag.Jobs = await _db.Jobs.Where(j => j.IsActive).ToListAsync();
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Analyze(int resumeId, int jobId)
    {
        var resume = await _db.Resumes.FindAsync(resumeId);
        var job = await _db.Jobs.FindAsync(jobId);

        if (resume == null || job == null)
            return NotFound();

        var result = await _matcher.AnalyzeMatchAsync(resume, job);
        _db.MatchResults.Add(result);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Report), new { id = result.Id });
    }

    public async Task<IActionResult> Report(int id)
    {
        var result = await _db.MatchResults
            .Include(m => m.Resume)
            .Include(m => m.Job)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (result == null) return NotFound();

        var vm = new MatchReportViewModel
        {
            Resume = result.Resume!,
            Job = result.Job!,
            Result = result,
            MatchedSkillsList = result.MatchedSkills?.Split(',').Select(s => s.Trim()).ToList() ?? new(),
            MissingSkillsList = result.MissingSkills?.Split(',').Select(s => s.Trim()).ToList() ?? new()
        };

        return View(vm);
    }
}