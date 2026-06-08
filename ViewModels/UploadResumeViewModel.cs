using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ResumeMatcher.ViewModels;

public class UploadResumeViewModel
{
    [Required(ErrorMessage = "Candidate name is required")]
    [Display(Name = "Candidate Name")]
    public string CandidateName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a file")]
    [Display(Name = "Resume File (PDF or DOCX)")]
    public IFormFile? ResumeFile { get; set; }
}