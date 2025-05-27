namespace IssueManager.Srevices
{
    public class ApiService
    {
        private readonly HttpClient _client;

        public ApiService(HttpClient client)
        {
            _client = client;
        }

        protected async Task<T> GetAsync<T>(string url)
        {
            return await _client.GetFromJsonAsync<T>(url);
        }

        protected async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
        {
            return await _client.PostAsJsonAsync(url, data);
        }

        protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var response = await _client.PostAsJsonAsync(url, data);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<TResponse>();
            return default;
        }

        protected async Task<HttpResponseMessage> PutAsync<T>(string url, T data)
        {
            return await _client.PutAsJsonAsync(url, data);
        }

        protected async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return await _client.DeleteAsync(url);
        }

    }
}
