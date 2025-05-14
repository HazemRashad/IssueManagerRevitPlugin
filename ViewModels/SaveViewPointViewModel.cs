
using System.Collections.ObjectModel;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace IssueManager.ViewModels
{
    [INotifyPropertyChanged]
    public sealed partial class SaveViewPointViewModel 
    {
        public ObservableCollection<string> MasterList { get; set; }   
        public string SelectedMaster { get; set; }
        public string TaskName { get; set; }
        public string Comment { get; set; }
        public bool IsLinked { get; set; }
        public bool IsDependent { get; set; } = true;
        public string AssignedTo { get; set; }
        public bool IsFlagged { get; set; }

        public RelayCommand SaveCommand => new(OnSave);

        public void OnSave()
        {
            string taskName = TaskName;
            string comment = Comment;
            string type = IsLinked ? "Linked" : "Dependant";
            string masterId = SelectedMaster;
            string assignedTo = AssignedTo;
            bool flagged = IsFlagged;

            List<string> SelectedElementsIds = Context.ActiveUiDocument.Selection.GetElementIds()
                                               .Select(id => id.IntegerValue.ToString()).ToList();
            try
            {
                string path = @"C:\\Users\\Ahmed Moenes\\Desktop";
                string baseFileName = $"Snapshot_{Context.ActiveView.Name}_{DateTime.Now:yyyyMMdd_HHmmss}";
                ImageExportOptions options = new ImageExportOptions
                {
                    ZoomType = ZoomFitType.FitToPage,
                    PixelSize = 1920,
                    FilePath = Path.Combine(path, baseFileName),
                    FitDirection = FitDirectionType.Horizontal,
                    ExportRange = ExportRange.CurrentView,
                    ViewName = Context.ActiveView.Name,
                    HLRandWFViewsFileType = ImageFileType.PNG,
                    ShadowViewsFileType = ImageFileType.PNG,
                    ImageResolution = ImageResolution.DPI_150
                };
                Context.Document.ExportImage(options);
                TaskDialog.Show("Success", $"Snapshot exported");
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", $"Snapshot failed: {ex.Message}");
            }
        }
    }
}