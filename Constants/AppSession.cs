using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueManager.Constants
{
    public static class AppSession
    {
        public static bool IsUserLoggedIn { get; set; } = false;
        public static string? Token { get; set; } = null;
    }
}
