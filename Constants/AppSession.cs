using DTOs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IssueManager.Constants
{
    public static class AppSession
    {
        public static bool IsUserLoggedIn { get; set; } = false;
        public static string? Token { get; set; } = null;
        public static string? UserId { get; set; } = null;
        public static int? ProjectId { get; set; } = null;


        public static string? GetEmailFromToken()
        {
            if (string.IsNullOrWhiteSpace(Token)) return null;

            var parts = Token.Split('.');
            if (parts.Length < 2) return null;

            var payload = parts[1];
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');

            var bytes = Convert.FromBase64String(payload);
            var json = Encoding.UTF8.GetString(bytes);

            var claims = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return claims != null && claims.TryGetValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", out var email)
                ? email?.ToString()
                : null;
        }





        public static async Task<bool> LoadUserIdFromTokenAsync(HttpClient client)
        {
            string? email = GetEmailFromToken();
            if (string.IsNullOrWhiteSpace(email)) return false;

            try
            {
                var response = await client.GetAsync($"api/Users/get-id-by-email/{email}");
                if (!response.IsSuccessStatusCode) return false;

                UserId = await response.Content.ReadAsStringAsync();

                // Get full user info to extract ProjectId
                var projectresponse = await client.GetAsync($"api/ProjectTeamMembers/user/{UserId}");
                if (projectresponse.IsSuccessStatusCode)
                {
                    ProjectId = int.Parse(await projectresponse.Content.ReadAsStringAsync());
                }

                return true;
            }
            catch
            {
                return false;
            }
        }



    }
}
