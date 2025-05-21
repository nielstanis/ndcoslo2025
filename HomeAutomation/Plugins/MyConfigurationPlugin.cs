
using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace HomeAutomation.Plugins;

public class MyConfigurationPlugin
{
    [KernelFunction, Description("gets the content of the config-file and returns it")]
    public string GetConfigFileContent(string file)
    {
        return System.IO.File.ReadAllText(file);
    }
}