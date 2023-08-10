using Azure.Identity;
using Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AppEnvironmentVariables = Infrastructure.Constants.EnvironmentVariables;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((context, configurationBuilder) =>
    {
        if (context.HostingEnvironment.IsDevelopment())
        {
            configurationBuilder.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
            configurationBuilder.AddUserSecrets<Program>(optional: true, reloadOnChange: true);
        }

        var builtConfig = configurationBuilder.Build();

        var keyVaultName = builtConfig[AppEnvironmentVariables.FatpAzureKeyVaultName];

        if (!string.IsNullOrEmpty(keyVaultName))
        {
            var credential = new DefaultAzureCredential();
            configurationBuilder.AddAzureKeyVault(new Uri($"https://{keyVaultName}.vault.azure.net/"), credential);
        }
    })
    .ConfigureServices(builder =>
    {
        builder.AddHttpClient();
    })
    .AddLogging()
    .Build();

await host.RunAsync();
