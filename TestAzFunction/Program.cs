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
            configBuilder.AddAzureKeyVault(
                new Uri("https://test-key-vault-jpss.vault.azure.net/"),
                new DefaultAzureCredential()
                /*new DefaultAzureCredential(new DefaultAzureCredentialOptions
            //    {
            //        ManagedIdentityClientId = "ccba3238-7fe7-4fdb-a30c-9bec1f5dac45"
                })*/);
        }
    })
    .Build();

host.Run();