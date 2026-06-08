using System.Text;
using System.Text.Json;
using ResumeMatcher.Models;

namespace ResumeMatcher.Services;

public class OpenAIMatcherService : IAIMatcherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public OpenAIMatcherService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<MatchResult> AnalyzeMatchAsync(Resume resume, Job job)
    {
        var apiKey = _config["OpenAI:ApiKey"]!;
        var model = _config["OpenAI:Model"] ?? "gpt-4o";

        var prompt = "You are an expert HR analyst. Analyze this resume against the job description.\n\n" +
              "RESUME:\n" +
              "Candidate: " + resume.CandidateName + "\n" +
              "Content: " + resume.ExtractedText[..Math.Min(3000, resume.ExtractedText.Length)] + "\n\n" +
              "JOB:\n" +
              "Title: " + job.Title + " at " + job.Company + "\n" +
              "Description: " + job.Description + "\n" +
              "Required Skills: " + job.RequiredSkills + "\n\n" +
              "Respond ONLY with valid JSON, no markdown, no extra text:\n" +
              "{\"matchScore\": 85, \"matchedSkills\": \"C#, .NET\", \"missingSkills\": \"Docker\", \"summary\": \"Strong candidate...\", \"recommendation\": \"Highly Recommended\"}\n\n" +
              "recommendation must be one of: Highly Recommended, Recommended, Maybe, Not Recommended";

        var requestBody = new
        {
            model,
            max_tokens = 1000,
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        request.Headers.Add("Authorization", $"Bearer {apiKey}");
        request.Content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseText = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseText);
        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "{}";

        using var aiResult = JsonDocument.Parse(content);
        var root = aiResult.RootElement;

        return new MatchResult
        {
            ResumeId = resume.Id,
            JobId = job.Id,
            MatchScore = root.GetProperty("matchScore").GetDouble(),
            MatchedSkills = root.GetProperty("matchedSkills").GetString(),
            MissingSkills = root.GetProperty("missingSkills").GetString(),
            AISummary = root.GetProperty("summary").GetString(),
            Recommendation = root.GetProperty("recommendation").GetString(),
            AnalyzedAt = DateTime.UtcNow
        };
    }
}