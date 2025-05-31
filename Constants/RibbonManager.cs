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

            #region Load View Point Button
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
            #endregion


            #region Save View Point Button
            var saveBtnData = new PushButtonData(
                "SaveIssuesButton",
                "Add Issue",
                typeof(SaveViewPointCommand).Assembly.Location,
                typeof(SaveViewPointCommand).FullName
            )
            {
                AvailabilityClassName = typeof(RequiresLoginAvailability).FullName
            };

            var saveBtn = panel.AddItem(saveBtnData) as PushButton;

            saveBtn?.SetImage("/IssueManager;component/Resources/Icons/RibbonIcon16.png");
            saveBtn?.SetLargeImage("/IssueManager;component/Resources/Icons/RibbonIcon32.png");
            #endregion

            #region Revit Button
            panel.AddPushButton<RevitAddinCommand>("Revit Logic")
                .SetImage("/IssueManager;component/Resources/Icons/RibbonIcon16.png")
                .SetLargeImage("/IssueManager;component/Resources/Icons/RibbonIcon32.png");
            #endregion
        }
    }
}
