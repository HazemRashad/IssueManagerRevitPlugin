using IssueManager.Commands;
using Nice3point.Revit.Toolkit.External;

namespace IssueManager
{
    /// <summary>
    ///     Application entry point
    /// </summary>
    [UsedImplicitly]
    public class Application : ExternalApplication
    {
        public override void OnStartup()
        {
            CreateRibbon();
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