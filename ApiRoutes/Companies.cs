namespace IssueManager.ApiRoutes
{
    public static class Companies
    {
        public const string Base = "api/companies";

        public static string GetAll() => Base;

        public static string GetById(int id) => $"{Base}/{id}";

        public static string Create() => $"{Base}/create";

        public static string CreateWithAdmin() => $"{Base}/create-with-admin";

        public static string Update(int id) => $"{Base}/{id}";

        public static string Delete(int id) => $"{Base}/{id}";

        public static string GetOverviewForUser() => $"{Base}/overview";
    }
}
