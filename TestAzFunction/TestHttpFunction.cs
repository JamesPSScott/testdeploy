using System.Collections.Generic;
using System.Net;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
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

        _logger.LogInformation($"LETS SEE IF AddAzureKeyVault works as expected: {_config["secret1"]}");
        Response<KeyVaultSecret>? secret = null;
        try
        {
            var test = new SecretClient(new Uri("https://test-key-vault-jpss.vault.azure.net/"), new DefaultAzureCredential( new DefaultAzureCredentialOptions()
            {
                ManagedIdentityClientId = "d66364cb-427d-46bb-9ba8-be4c8aa6a74a"
            }));

            secret = await test.GetSecretAsync("secret1");
            
            _logger.LogInformation("I tried to retrieve someVal: " + secret.Value.Value);
        }
        catch (Exception e)
        {
            _logger.LogError("Error retrieving vault: " + e.Message);
        }
        
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        try
        {
            _logger.LogInformation("About to test blob functinoality");
            BlobServiceClient blobClient = new BlobServiceClient(
                new Uri("https://functionstorage011224.blob.core.windows.net/"), new DefaultAzureCredential(
                    new DefaultAzureCredentialOptions()
                    {
                        ManagedIdentityClientId = "d66364cb-427d-46bb-9ba8-be4c8aa6a74a"
                    }));

            MemoryStream msTest = new MemoryStream();
            
            BlobContainerClient containerClient = blobClient.GetBlobContainerClient("test");
            var testBlobClient = containerClient.GetBlobClient("Items for Kleanthis.txt");
            var downloadResult = await testBlobClient.DownloadToAsync(msTest);

            if (!downloadResult.IsError)
            {
                msTest.Seek(0, SeekOrigin.Begin);
                byte[] buffer = new byte [msTest.Length];
                await msTest.ReadAsync(buffer);

                string textInFile = System.Text.Encoding.UTF8.GetString(buffer);
                _logger.LogInformation("file has: " + textInFile);
            }
            
            var result = containerClient.GetBlobsAsync();
            await foreach (var test in result.AsPages())
            {
                foreach (var blobItem in test.Values)
                {
                    _logger.LogInformation("blob: " + blobItem.Name);
                }
            }
        }
        catch (Exception x)
        {
            _logger.LogError("Error with BLOBS: " + x.Message);
        }
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        
        
        response.WriteString($"Welcome to Azure Functions this is v2! Value1: { _config["secret1"]} - Value 2: {_config["secret2"]}");

        
        
        return response;
        
    }
}