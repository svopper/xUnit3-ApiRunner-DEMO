using System.Text.Json;

namespace ApiTestRunner.Tests;

public class UnitTest1
{
    [Fact]
    public void AssertTrueOnTrue_ShouldSucceed()
    {
        Assert.True(true);
    }
    
    [Fact]
    public void AssertTrueOnFalse_ShouldFail()
    {
        Assert.True(false);
    }
    
    [Fact]
    public void LoadJsonData_ShouldSucceed()
    {
        var path = Path.Combine(Environment.CurrentDirectory, "TestData", "case_001.json");
        var file = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<UseCase>(file, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.Equal("case_001", data?.Key);
    }
    
    [Fact]
    public async Task LongRunningTest_ShouldSucceed()
    {
        // Simulate a long-running test
        await Task.Delay(5000, CancellationToken.None);
        Assert.True(true);
    }
}

record UseCase(string Key);