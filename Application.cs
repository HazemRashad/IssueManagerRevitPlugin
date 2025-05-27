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
            RibbonManager.CreateRibbon(Application);
            AppHost = HostingExtensions.BuildAppHost();
            AppHost.Start();
        }
      
    }
}