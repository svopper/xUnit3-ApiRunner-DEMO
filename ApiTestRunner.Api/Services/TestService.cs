using System.Collections.Concurrent;
using ApiTestRunner.Api.Models;
using ApiTestRunner.Tests;
using Xunit.SimpleRunner;

namespace ApiTestRunner.Api.Services;

public class TestService
{
    private readonly ConcurrentDictionary<string, TestJobStatus> _jobs = new();
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _cancellationTokens = new();

    public void StartTestRun(string jobId)
    {
        var startedAt = DateTime.UtcNow;
        var cts = new CancellationTokenSource();
        _cancellationTokens[jobId] = cts;
        _jobs[jobId] = new TestJobStatus(jobId, "Running", [], false, startedAt, null);

        // Run tests in background without blocking
        Task.Run(async () =>
        {
            var results = new List<string>();
            var assemblyName = typeof(UnitTest1).Assembly.GetName().Name;

            if (assemblyName == null)
            {
                _jobs[jobId] = new TestJobStatus(
                    jobId, 
                    "Failed",
                    ["Test assembly name could not be determined"], 
                    true, 
                    startedAt, 
                    DateTime.UtcNow);
                CleanupCancellationToken(jobId);
                return;
            }
            
            var testAssemblyPath = Path.Combine(AppContext.BaseDirectory, assemblyName + ".dll");

            if (!File.Exists(testAssemblyPath))
            {
                _jobs[jobId] = new TestJobStatus(
                    jobId, 
                    "Failed",
                    ["Test assembly not found"], 
                    true, 
                    startedAt, 
                    DateTime.UtcNow);
                CleanupCancellationToken(jobId);
                return;
            }

            try
            {
                cts.Token.ThrowIfCancellationRequested();
                
                var testRunner = new AssemblyRunner(new AssemblyRunnerOptions(testAssemblyPath)
                {
                    OnTestFailed = info =>
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        results.Add($"Test {info.TestDisplayName} failed: {info.Exception.Message}");
                    },
                    OnTestPassed = info =>
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        results.Add($"Test {info.TestDisplayName} passed.");
                    }
                });

                await testRunner.Run();

                _jobs[jobId] = new TestJobStatus(
                    jobId, 
                    "Completed", 
                    results, 
                    true, 
                    startedAt, 
                    DateTime.UtcNow);
            }
            catch (OperationCanceledException)
            {
                _jobs[jobId] = new TestJobStatus(
                    jobId, 
                    "Cancelled", 
                    results, 
                    true, 
                    startedAt, 
                    DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _jobs[jobId] = new TestJobStatus(
                    jobId, 
                    "Failed",
                    [$"Error running tests: {ex.Message}"], 
                    true, 
                    startedAt, 
                    DateTime.UtcNow);
            }
            finally
            {
                CleanupCancellationToken(jobId);
            }
        }, cts.Token);
    }

    public TestJobStatus? GetStatus(string jobId)
    {
        return _jobs.GetValueOrDefault(jobId);
    }

    public CancellationResult CancelTestRun(string jobId)
    {
        if (!_jobs.TryGetValue(jobId, out var status))
        {
            return CancellationResult.NotFound;
        }

        if (status.IsComplete)
        {
            return CancellationResult.AlreadyCompleted;
        }

        if (_cancellationTokens.TryGetValue(jobId, out var cts))
        {
            cts.Cancel();
            return CancellationResult.Success;
        }

        return CancellationResult.NotFound;
    }

    private void CleanupCancellationToken(string jobId)
    {
        if (_cancellationTokens.TryRemove(jobId, out var cts))
        {
            cts.Dispose();
        }
    }
}