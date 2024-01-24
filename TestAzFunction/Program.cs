using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureAppConfiguration((context, configBuilder) =>
    {
        configBuilder.AddJsonFile("app.settings.json");
        
        /*if (context.HostingEnvironment.IsEnvironment("Production"))
        {
            configBuilder.AddAzureKeyVault(
                new Uri("https://test-key-vault-jpss.vault.azure.net/"),
                new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ManagedIdentityClientId = "c901d444-6b59-4ff7-a5b2-6b0fa3f6fe17"
                }));
        }*/

    })
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        
    })
    .Build();

host.Run();