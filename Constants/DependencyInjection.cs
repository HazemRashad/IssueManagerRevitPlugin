namespace IssueManager.Constants
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind ApiSettings
            services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));

            // Configure HttpClient with dynamic base URI
            services.AddHttpClient<ApiService>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
            });

            services.TryAddTransient<ApiService>();
            services.TryAddTransient<IssueApiService>();
            services.TryAddTransient<CompanyApiService>();
            services.TryAddTransient<ProjectApiService>();
            services.TryAddTransient<UserApiService>();

            return services;
        }
    }
}
