using System.Collections.Generic;
using System.Net;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TestAzFunction;

public class TestHttpFunction
{
    private readonly ILogger _logger;

    private readonly IConfiguration _config;
    
    public TestHttpFunction(ILoggerFactory loggerFactory, IConfiguration config)
    {
        _logger = loggerFactory.CreateLogger<TestHttpFunction>();

        _config = config;
    }
    
    // test push
    [Function("TestHttpFunction")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        _logger.LogInformation("about to retrieve keyvault val");
        try
        {
            var test = new SecretClient(new Uri("https://test-key-vault-jpss.vault.azure.net/"), new DefaultAzureCredential());

            var someVal = await test.GetSecretAsync("secret1");
            
            _logger.LogInformation("I tried to retrieve someVal: " + someVal.Value);
        }
        catch (Exception e)
        {
            _logger.LogError("Error retrieving vault: " + e.Message);
        }
        
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString($"Welcome to Azure Functions this is v2! Value1: {_config["secret1"]} - Value 2: {_config["secret2"]}");

        return response;
        
    }
}