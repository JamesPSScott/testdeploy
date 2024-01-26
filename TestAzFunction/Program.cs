using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureAppConfiguration((context, configBuilder) =>
    {
        configBuilder.AddJsonFile("app.settings.json");
        
    })
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
    })
    .ConfigureAppConfiguration((context, configBuilder) =>
    {
        if (context.HostingEnvironment.IsEnvironment("Production"))
        {
            // if you want to use a system managed identity you can use
            // DefaultAzureCredential() with no client id...
            // using client id means you have your own managed identity you control
            configBuilder.AddAzureKeyVault(
                new Uri("https://test-key-vault-jpss.vault.azure.net/"),
            //    new DefaultAzureCredential()
                new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ManagedIdentityClientId = "d66364cb-427d-46bb-9ba8-be4c8aa6a74a"
                }));
        }
    })
    .Build();

host.Run();