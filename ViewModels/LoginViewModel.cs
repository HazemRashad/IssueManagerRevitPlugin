using DTOs.Login;
using Xunit;

namespace IssueManager.ViewModels
{
    
    public partial class LoginViewModel : ObservableObject
    {
        private readonly LoginApiService _loginService;

        public Action? CloseAction { get; set; }

        public LoginViewModel(LoginApiService loginService)
        {
            _loginService = loginService;
        }

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string? errorMessage;

        

        [RelayCommand]
        private async Task LoginAsync()
        {

            IsLoading = true;
            ErrorMessage = null;

            try
            {
                var dto = new LoginRequestDto
                {
                    Email = Email,
                    Password = Password
                };

                var response = await _loginService.LoginAsync(dto);

                var status = response.StatusCode.ToString();
                var body = await response.Content.ReadAsStringAsync();

                if (response != null)
                {

                    MessageBox.Show("Login successful!");
                    CloseAction.Invoke();

                    AppSession.IsUserLoggedIn = true;
                    AppSession.Token = body;
                    MessageBox.Show($"{body}");

                }

                else
                {
                    ErrorMessage = "Invalid email or password.";
                    AppSession.IsUserLoggedIn = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                AppSession.IsUserLoggedIn = false;
            }
            finally
            {
                IsLoading = false;

            }
        }
    }
}
