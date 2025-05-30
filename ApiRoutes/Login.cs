using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueManager.ApiRoutes
{
    public static class Login
    {
        public const string Base = "api/auth";
        public static string login() => $"{Base}/login";
       
    }
}
