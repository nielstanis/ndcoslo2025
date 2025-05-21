
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

public class Def
{
    public string Data { get; private set; }
    public string AnotherData { get; private set; }
    public Def(string data)
    {
        Data = data;
    }

    public void Test(string input)
    {
        AnotherData = input;
    }
}