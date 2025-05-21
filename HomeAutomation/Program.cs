
using HomeAutomation.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;


namespace HomeAutomation;

internal static class Program
{
    internal static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        //We need to use user-secrets, demo purpose only.
        builder.Configuration.AddUserSecrets<Worker>();

        // Actual code to execute is found in Worker class
        builder.Services.AddHostedService<Worker>();
        builder.Services.AddSingleton<IChatCompletionService>(sp =>
        {
            #pragma warning disable CS8604 // Possible null reference argument.
            return new AzureOpenAIChatCompletionService("gpt-4o", builder.Configuration["AzureOpenAI:Endpoint"], builder.Configuration["AzureOpenAI:ApiKey"]);
            #pragma warning restore CS8604 // Possible null reference argument.
        });

        // Add plugins that can be used by kernels
        // The plugins are added as singletons so that they can be used by multiple kernels
        builder.Services.AddSingleton<MyTimePlugin>();
        builder.Services.AddSingleton<MyAlarmPlugin>();
        builder.Services.AddSingleton<MyConfigurationPlugin>();
        builder.Services.AddKeyedSingleton<MyLightPlugin>("OfficeLight");
        builder.Services.AddKeyedSingleton<MyLightPlugin>("PorchLight", (sp, key) =>
        {
            return new MyLightPlugin(turnedOn: true);
        });


        // Add a home automation kernel to the dependency injection container
        builder.Services.AddKeyedTransient<Kernel>("HomeAutomationKernel", (sp, key) =>
        {
            // Create a collection of plugins that the kernel will use
            KernelPluginCollection pluginCollection = [];
            pluginCollection.AddFromObject(sp.GetRequiredService<MyTimePlugin>());
            pluginCollection.AddFromObject(sp.GetRequiredService<MyAlarmPlugin>());
            pluginCollection.AddFromObject(sp.GetRequiredService<MyConfigurationPlugin>());
            pluginCollection.AddFromObject(sp.GetRequiredKeyedService<MyLightPlugin>("OfficeLight"), "OfficeLight");
            pluginCollection.AddFromObject(sp.GetRequiredKeyedService<MyLightPlugin>("PorchLight"), "PorchLight");

            // When created by the dependency injection container, Semantic Kernel logging is included by default
            return new Kernel(sp, pluginCollection);
        });

        using IHost host = builder.Build();

        await host.RunAsync();
    }
}
