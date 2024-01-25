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
                new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ManagedIdentityClientId = "ccba3238-7fe7-4fdb-a30c-9bec1f5dac45"//"e2df078a-ae8a-48ee-a296-5ad8f5e4d99b"//"c901d444-6b59-4ff7-a5b2-6b0fa3f6fe17"
                }));
        }

    })
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
    })
    .Build();

host.Run();