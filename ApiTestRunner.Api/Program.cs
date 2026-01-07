using Microsoft.AspNetCore.Http.HttpResults;
using Xunit.SimpleRunner;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapPost("/run-tests", Endpoints.RunTests);

app.Run();


static class Endpoints
{
    public static async Task<Results<Ok<List<string>>, NotFound<string>>> RunTests()
    {
        var testAssemblyPath = Path.Combine(AppContext.BaseDirectory, "ApiTestRunner.Tests.dll");
        if (!File.Exists(testAssemblyPath))
        {
            return TypedResults.NotFound("Test assembly not found.");
        }

        var results = new List<string>();
        var testRunner = new AssemblyRunner(new AssemblyRunnerOptions(testAssemblyPath)
        {
            OnTestFailed = info =>
            {
                results.Add($"Test {info.TestDisplayName} failed: {info.Exception.Message}");
            },
            OnTestPassed =  info =>
            {
                results.Add($"Test {info.TestDisplayName} passed.");
            }
        });
        
        await testRunner.Run();
        
        return TypedResults.Ok(results);
    }
} 