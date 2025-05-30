using DTOs.Login;

namespace IssueManager.Services
{
    public class LoginApiService : ApiService
    {
        public LoginApiService(HttpClient client) : base(client) { }

        public Task<HttpResponseMessage> LoginAsync(LoginRequestDto dto)
           => PostAsync(Login.login(), dto);

    }
}
