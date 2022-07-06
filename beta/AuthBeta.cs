﻿using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Net.Http.Headers;

namespace beta
{
    class Auth_beta
    {
        public GraphServiceClient graphAuth(ILogger log)
        {

            IConfiguration config = new ConfigurationBuilder()

           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
           .AddEnvironmentVariables()
           .Build();

            log.LogInformation("C# HTTP trigger function processed a request.");
            var scopes = new string[] { "https://graph.microsoft.com/.default" };
            //var keyVaultUrl = config["keyVaultUrl"];
            //var keyname = config["keyname"];
            //var tenantid = config["tenantid"];
            //var cliendID = config["clientid"];

            //SecretClientOptions options = new SecretClientOptions()
            //{
            //    Retry =
            //    {
            //        Delay= TimeSpan.FromSeconds(2),
            //        MaxDelay = TimeSpan.FromSeconds(16),
            //        MaxRetries = 5,
            //        Mode = RetryMode.Exponential
            //     }
            //};
            //var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential(), options);

            //KeyVaultSecret secret = client.GetSecret(keyname);

            //value for local testing only
            var secret = "";
            var tenantid = "";
            var cliendID = "";

            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
            .Create(cliendID)
            .WithTenantId(tenantid)
            .WithClientSecret(secret)
            .Build();

            // Build the Microsoft Graph client. As the authentication provider, set an async lambda
            // which uses the MSAL client to obtain an app-only access token to Microsoft Graph,
            // and inserts this access token in the Authorization header of each API request. 
            GraphServiceClient graphServiceClient =
                new GraphServiceClient(new DelegateAuthenticationProvider(async (requestMessage) => {

                    // Retrieve an access token for Microsoft Graph (gets a fresh token if needed).
                    var authResult = await confidentialClientApplication
                        .AcquireTokenForClient(scopes)
                        .ExecuteAsync();

                    // Add the access token in the Authorization header of the API request.
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
                })
                );
            return graphServiceClient;
        }

    }
}
