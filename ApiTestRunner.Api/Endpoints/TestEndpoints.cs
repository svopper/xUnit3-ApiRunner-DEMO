using ApiTestRunner.Api.Models;
using ApiTestRunner.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ApiTestRunner.Api.Endpoints;

public static class TestEndpoints
{
    public static WebApplication RegisterTestEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("tests");
        group.MapPost("/run-all", RunTests);
        group.MapGet("/status/{jobId}", GetTestStatus);
        group.MapPost("/cancel/{jobId}", CancelTests);
        
        return app;
    }

    private static Accepted<TestJobResponse> RunTests(TestService jobService)
    {
        var jobId = Guid.NewGuid().ToString();
        jobService.StartTestRun(jobId);
        return TypedResults.Accepted($"/tests/status/{jobId}", new TestJobResponse(jobId, "Test run started"));
    }

    private static Results<Ok<TestJobStatus>, NotFound<string>> GetTestStatus(string jobId, TestService jobService)
    {
        var status = jobService.GetStatus(jobId);
        return status != null 
            ? TypedResults.Ok(status) 
            : TypedResults.NotFound($"Job with ID '{jobId}' not found");
    }

    private static Results<Ok<string>, NotFound<string>, BadRequest<string>> CancelTests(string jobId, TestService jobService)
    {
        var result = jobService.CancelTestRun(jobId);
        return result switch
        {
            CancellationResult.Success => TypedResults.Ok($"Test run '{jobId}' has been cancelled"),
            CancellationResult.NotFound => TypedResults.NotFound($"Job with ID '{jobId}' not found"),
            CancellationResult.AlreadyCompleted => TypedResults.BadRequest($"Job with ID '{jobId}' is already completed"),
            _ => TypedResults.BadRequest("Unknown error occurred")
        };
    }
}