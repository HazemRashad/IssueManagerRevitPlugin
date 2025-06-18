using Autodesk.Revit.UI;
using IssueManager.Revit;

public static class RibbonManager
{
    private static PushButton _loginButton;
    private static PushButton _saveButton;
    private static PushButton _loadButton;
    private static PushButton _issuesListButton;
  

    public static void CreateRibbon(this UIControlledApplication app)
    {
        var panel = app.CreatePanel("Commands", "Issue Manager");

        

        // ⬅️ Login
        _loginButton = panel.AddPushButton<LoginCommand>("Login");
        _loginButton.SetLargeImage("/IssueManager;component/Resources/Icons/login.png");

        // ⬅️ Save
        var saveBtnData = new PushButtonData("SaveIssuesButton", "Add Issue",
            typeof(SaveViewPointCommand).Assembly.Location,
            typeof(SaveViewPointCommand).FullName)
        {
            AvailabilityClassName = typeof(RequiresLoginAvailability).FullName
        };

        _saveButton = panel.AddItem(saveBtnData) as PushButton;
        _saveButton.Enabled = false;
        _saveButton.SetLargeImage("/IssueManager;component/Resources/Icons/SaveIssue.png");

        // ⬅️ Load
        var loadBtnData = new PushButtonData("LoadIssuesButton", "Load Issues",
            typeof(LoadViewPointCommand).Assembly.Location,
            typeof(LoadViewPointCommand).FullName)
        {
            AvailabilityClassName = typeof(RequiresLoginAvailability).FullName
        };

        _loadButton = panel.AddItem(loadBtnData) as PushButton;
        _loadButton.Enabled = false;
        _loadButton.SetLargeImage("/IssueManager;component/Resources/Icons/Load.png");

        // ⬅️ Issues List
        var issuesListBtnData = new PushButtonData("IssuesListButton", "Issues List",
            typeof(IssuesListCommand).Assembly.Location,
            typeof(IssuesListCommand).FullName)
        {
            AvailabilityClassName = typeof(RequiresLoginAvailability).FullName
        };
        _issuesListButton = panel.AddItem(issuesListBtnData) as PushButton;
        _issuesListButton.Enabled = true;
        _issuesListButton.SetLargeImage("/IssueManager;component/Resources/Icons/RibbonIcon32.png");

    }


    public static void OnLoginSuccess()
    {
        _loginButton.Enabled = false;
        _saveButton.Enabled = true;
        _loadButton.Enabled = true;
        _issuesListButton.Enabled = true;
    }
}
