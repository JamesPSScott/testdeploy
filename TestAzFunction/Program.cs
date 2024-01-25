using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureAppConfiguration((context, configBuilder) =>
    {
        configBuilder.AddJsonFile("app.settings.json");
        
        if (context.HostingEnvironment.IsEnvironment("Production"))
        {
            configBuilder.AddAzureKeyVault(
                new Uri("https://test-key-vault-jpss.vault.azure.net/"),
                new DefaultAzureCredential());
        }

    })
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
    })
    .Build();

host.Run();