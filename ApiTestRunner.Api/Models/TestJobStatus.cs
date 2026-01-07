namespace ApiTestRunner.Api.Models;

public record TestJobStatus(string JobId, string Status, List<string> Results, bool IsComplete, DateTime StartedAt, DateTime? CompletedAt);