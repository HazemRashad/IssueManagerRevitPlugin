namespace IssueManager.ApiRoutes
{
    public static class Issues
    {
        public const string Base = "api/issues";

        public static string GetAll() => Base;

        public static string GetById(int id) => $"{Base}/{id}";

        public static string Create() => $"{Base}/create";

        public static string Update(int id) => $"{Base}/{id}";

        public static string Delete(int id) => $"{Base}/{id}";

    }
}
