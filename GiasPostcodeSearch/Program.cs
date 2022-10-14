using System.Net.Http.Headers;
using System.Text;
using GiasPostcodeSearch;
using Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

var username = "";
var usernameParam = args.FirstOrDefault(a => a.Contains("username"));
if (usernameParam != null && usernameParam.Contains("="))
{
    username = usernameParam.Split("=")[1];
}

var password = "";
var passwordParam = args.FirstOrDefault(a => a.Contains("password"));
if (passwordParam != null && passwordParam.Contains("="))
{
    password = passwordParam.Split("=")[1];
}

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddNtpDbContext(hostContext.Configuration);
        services.AddHttpClient<IHostedService, GiasPostcodeSearchService>(client =>
        {
            client.BaseAddress = new Uri(url);

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                var authenticationString = $"{username}:{password}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationString));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
            }
        });
    })
    .AddLogging()
    .Build();

await host.RunAsync();