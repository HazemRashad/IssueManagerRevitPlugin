namespace IssueManager.ApiRoutes
{
    public static class Users
    {
        public const string Base = "api/Users";
        public static string GetAll() => Base;

        public static string GetById(string id) => $"{Base}/{id}";

        public static string Register() => $"{Base}/register";

        public static string RegisterWithProjects() => $"{Base}/register-with-project";

        public static string Update(string id) => $"{Base}/{id}";

        public static string Delete(string id) => $"{Base}/{id}";
    }
}
