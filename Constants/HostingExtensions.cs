namespace IssueManager.Constants
{
    public static class HostingExtensions
    {
        public static IHost BuildAppHost()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(config =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddProjectServices(context.Configuration);
                })
                .Build();
        }
    }
}
