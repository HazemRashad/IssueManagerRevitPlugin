

namespace IssueManager
{
    /// <summary>
    ///     Application entry point
    /// </summary>
    [UsedImplicitly]
    public class Application : ExternalApplication
    {
        public static IHost AppHost { get; private set; }   
        public override void OnStartup()
        {
            CreateRibbon();
            AppHost = HostingExtensions.BuildAppHost();
            AppHost.Start();
        }
        private void CreateRibbon()
        {
            var panel = Application.CreatePanel("Commands", "IssueManager");

            panel.AddPushButton<SaveViewPointCommand>("Create Issue")
                .SetImage("/IssueManager;component/Resources/Icons/RibbonIcon16.png")
                .SetLargeImage("/IssueManager;component/Resources/Icons/RibbonIcon32.png");

            panel.AddPushButton<LoadViewPointCommand>("Load Issues")
                .SetImage("/IssueManager;component/Resources/Icons/RibbonIcon16.png")
                .SetLargeImage("/IssueManager;component/Resources/Icons/RibbonIcon32.png");
        }
    }
}