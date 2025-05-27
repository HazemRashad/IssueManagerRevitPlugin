namespace IssueManager.Resources
{
    public static class WindowRevitHandler
    {
        /// <summary>
             /// Shows a WPF window of the given type, setting the Revit window as its owner.
             /// </summary>
             /// <typeparam name="T">The type of the window to show.</typeparam>
             /// <param name="getService">A function to resolve the window instance of type T.</param>
        public static void ShowWindow (Window window)
        {
            // Force software rendering to avoid potential rendering issues in Revit's WPF host
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            // Ensure a WPF Application instance is available
            if (System.Windows.Application.Current == null)
            {
                var app = new System.Windows.Application
                {
                    ShutdownMode = ShutdownMode.OnExplicitShutdown
                };
            }

            // Obtain the main Revit window handle
            IntPtr revitHandle = ComponentManager.ApplicationWindow;

            if (window != null)
            {
                // Set the Revit window as the owner of the custom window
                var windowInteropHelper = new WindowInteropHelper(window);
                windowInteropHelper.Owner = revitHandle;


                window.Show();
            }
        }

        /// <summary>
             /// Shows a WPF window of the given type as a dialog, setting the Revit window as its owner.
             /// </summary>
             /// <typeparam name="T">The type of the window to show as a dialog.</typeparam>
             /// <param name="getService">A function to resolve the window instance of type T.</param>
             /// <returns>Nullable bool indicating the dialog result.</returns>
        public static bool? ShowDialog<T>(Func<T> getService) where T : Window
        {
            // Force software rendering to avoid potential rendering issues in Revit's WPF host
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            // Ensure a WPF Application instance is available
            if (System.Windows.Application.Current == null)
            {
                var app = new System.Windows.Application
                {
                    ShutdownMode = ShutdownMode.OnExplicitShutdown
                };
            }

            // Obtain the main Revit window handle
            IntPtr revitHandle = ComponentManager.ApplicationWindow;

            // Resolve the window instance using the provided function
            var window = getService();
            if (window != null)
            {
                // Set the Revit window as the owner of the custom dialog window
                var windowInteropHelper = new WindowInteropHelper(window);
                windowInteropHelper.Owner = revitHandle;

                // Show the dialog window and return the result
                return window.ShowDialog();
            }

            // Return null if the window could not be created
            return null;
        }
    }
}
