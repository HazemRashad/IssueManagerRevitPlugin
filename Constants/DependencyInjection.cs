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

            services.TryAddTransient<IssueApiService,IssueApiService>();
            services.TryAddTransient<CompanyApiService,CompanyApiService>();
            services.TryAddTransient<ProjectApiService,ProjectApiService>();
            services.TryAddTransient<UserApiService,UserApiService>();

            return services;
        }
    }
}
