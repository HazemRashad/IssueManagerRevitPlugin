namespace IssueManager.Srevices
{
    public class ApiService
    {
        private readonly HttpClient client;

        public ApiService(HttpClient client)
        {
            this.client = client;
        }

        protected async Task<T> GetAsync<T>(string url)
        {
            return await client.GetFromJsonAsync<T>(url);
        }

        protected async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
        {
            return await client.PostAsJsonAsync(url, data);
        }

        protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var response = await client.PostAsJsonAsync(url, data);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<TResponse>();
            return default;
        }

        protected async Task<HttpResponseMessage> PutAsync<T>(string url, T data)
        {
            return await client.PutAsJsonAsync(url, data);
        }

        protected async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return await client.DeleteAsync(url);
        }

    }
}
