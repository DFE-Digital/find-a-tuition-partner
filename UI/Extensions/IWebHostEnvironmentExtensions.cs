
namespace UI.Extensions
{
    public static class IWebHostEnvironmentExtensions
    {
        public static bool IsTesting(this IWebHostEnvironment environment)
        {
            return environment.EnvironmentName == "Testing";
        }
    }
}

