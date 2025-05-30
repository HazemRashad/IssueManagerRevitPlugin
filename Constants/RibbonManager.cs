using Autodesk.Revit.UI;
using IssueManager.Revit;


namespace IssueManager.Constants
{
    public static class RibbonManager
    {
        public static void CreateRibbon(this UIControlledApplication app)
        {
            var panel = app.CreatePanel("Commands", "IssueManager");


            panel.AddPushButton<LoginViewPointCommand>("Login")
                .SetImage("/IssueManager;component/Resources/Icons/RibbonIcon16.png")
                .SetLargeImage("/IssueManager;component/Resources/Icons/RibbonIcon32.png");


            var loadBtnData = new PushButtonData(
                "LoadIssuesButton",
                "Load Issues",
                typeof(LoadViewPointCommand).Assembly.Location,
                typeof(LoadViewPointCommand).FullName
            )
            {
                AvailabilityClassName = typeof(RequiresLoginAvailability).FullName
            };

            var loadBtn = panel.AddItem(loadBtnData) as PushButton;

            loadBtn?.SetImage("/IssueManager;component/Resources/Icons/RibbonIcon16.png");
            loadBtn?.SetLargeImage("/IssueManager;component/Resources/Icons/RibbonIcon32.png");
        }
    }
}
