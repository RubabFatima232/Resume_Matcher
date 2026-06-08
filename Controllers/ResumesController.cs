using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeMatcher.Models;
using ResumeMatcher.Services;
using ResumeMatcher.ViewModels;

namespace ResumeMatcher.Controllers;

public class ResumesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IFileParserService _parser;

    public ResumesController(ApplicationDbContext db, IFileParserService parser)
    {
        _db = db;
        _parser = parser;
    }

    public async Task<IActionResult> Index()
    {
        var resumes = await _db.Resumes.OrderByDescending(r => r.UploadedAt).ToListAsync();
        return View(resumes);
    }

    public IActionResult Upload() => View(new UploadResumeViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(UploadResumeViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        if (vm.ResumeFile == null || vm.ResumeFile.Length == 0)
        {
            ModelState.AddModelError("ResumeFile", "Please select a valid file.");
            return View(vm);
        }

        await using var stream = vm.ResumeFile.OpenReadStream();
        var text = await _parser.ExtractTextAsync(stream, vm.ResumeFile.FileName);

        var resume = new Resume
        {
            CandidateName = vm.CandidateName,
            FileName = vm.ResumeFile.FileName,
            ExtractedText = text
        };

        _db.Resumes.Add(resume);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Resume uploaded and parsed successfully!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var resume = await _db.Resumes.FindAsync(id);
        if (resume == null) return NotFound();
        _db.Resumes.Remove(resume);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}