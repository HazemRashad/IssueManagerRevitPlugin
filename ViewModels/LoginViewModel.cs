using DTOs.Login;
using HandyControl.Controls;
using System.Windows.Controls;
using Xunit;
using MessageBox = System.Windows.MessageBox;
using PasswordBox = HandyControl.Controls.PasswordBox;

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
        private async Task LoginAsync(PasswordBox passwordBox)
        {
            
            IsLoading = true;
            ErrorMessage = null;

            try
            {
                var dto = new LoginRequestDto
                {
                    Email = Email,
                    Password = passwordBox.Password
                };

                var response = await _loginService.LoginAsync(dto);

                var status = response.StatusCode.ToString();
                var body = await response.Content.ReadAsStringAsync();

                if (response != null)
                {

                    AppSession.Token = body;

                    await AppSession.LoadUserIdFromTokenAsync(new HttpClient
                    {
                        BaseAddress = new Uri("https://localhost:44374/")
                    });


                    AppSession.IsUserLoggedIn = true;
                    Console.WriteLine(AppSession.Token);

                    MessageBox.Show($"{body}");

                    RibbonManager.OnLoginSuccess();

                    //MessageBox.Show($"UserId: {AppSession.UserId}");
                    MessageBox.Show("Login successful!");
                    CloseAction.Invoke();

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
