using GiasPostcodeSearch;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var argsValid = true;

var url = "";
var urlParam = args.FirstOrDefault(a => a.Contains("url"));
if (string.IsNullOrWhiteSpace(urlParam) || !urlParam.Contains("="))
{
    Console.WriteLine("--url=<FIND_A_TUITION_PARTNER_URL> parameter is required");
    argsValid = false;
}
else
{
    url = urlParam.Split("=")[1];
}

if (!argsValid) return;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHttpClient<ISchoolDataProvider, GiasSchoolDataProvider>(client =>
        {
            client.BaseAddress = new Uri("https://ea-edubase-api-prod.azurewebsites.net/edubase/downloads/public/");
        });
        services.AddHttpClient<IHostedService, GiasPostcodeSearchService>(client =>
        {
            client.BaseAddress = new Uri(url);
        });
    })
    .Build();

await host.RunAsync();