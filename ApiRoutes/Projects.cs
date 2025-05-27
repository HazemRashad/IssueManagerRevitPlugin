namespace IssueManager.ApiRoutes
{
    public static class Projects
    {
        public const string Base = "api/projects";

        public static string GetAll() => $"{Base}/all-projects";

        public static string GetById(int id) => $"{Base}/{id}";

        public static string Create() => $"{Base}/create";

        public static string Update(int id) => $"{Base}/{id}";

        public static string Delete(int id) => $"{Base}/{id}";

        public static string OverviewForSubscriber() => $"{Base}/overview/subscriber";

        public static string OverviewForCompany() => $"{Base}/overview/company";

        public static string OverviewForUser() => $"{Base}/overview/user";
    }
}
