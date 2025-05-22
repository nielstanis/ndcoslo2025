
using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace HomeAutomation.Plugins;

/// <summary>
/// Simple plugin that just returns the time.
/// </summary>
public class MyTimePlugin
{
    [KernelFunction, Description("Get the current time")]
    public DateTimeOffset Time() => DateTimeOffset.Now;
}

public class Def(string data)
{
    public string Data { get; private set; } = data;
    public string? AnotherData { get; private set; }

    public void Test(string input)
    {
        AnotherData = input;
    }
}