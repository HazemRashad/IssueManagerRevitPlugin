using Autodesk.Revit.UI;

namespace IssueManager.Constants
{
    public static class RibbonManager
    {
        public static void CreateRibbon(this UIControlledApplication app)
        {
            var panel = app.CreatePanel("Commands", "IssueManager");

            panel.AddPushButton<SaveViewPointCommand>("Create Issue")
                .SetImage("/IssueManager;component/Resources/Icons/RibbonIcon16.png")
                .SetLargeImage("/IssueManager;component/Resources/Icons/RibbonIcon32.png");

            panel.AddPushButton<LoadViewPointCommand>("Load Issues")
                .SetImage("/IssueManager;component/Resources/Icons/RibbonIcon16.png")
                .SetLargeImage("/IssueManager;component/Resources/Icons/RibbonIcon32.png");
        }
    }
}
