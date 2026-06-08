using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeMatcher.Models;

namespace ResumeMatcher.Controllers;

public class JobsController : Controller
{
    private readonly ApplicationDbContext _db;

    public JobsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var jobs = await _db.Jobs.OrderByDescending(j => j.CreatedAt).ToListAsync();
        return View(jobs);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Job job)
    {
        if (!ModelState.IsValid) return View(job);
        _db.Jobs.Add(job);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Job added successfully!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var job = await _db.Jobs.FindAsync(id);
        if (job == null) return NotFound();
        _db.Jobs.Remove(job);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}