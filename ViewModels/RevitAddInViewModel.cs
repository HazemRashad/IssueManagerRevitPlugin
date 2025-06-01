using IssueManager.Revit;
using Nice3point.Revit.Toolkit.External.Handlers;

namespace IssueManager.ViewModels
{
    public partial class RevitAddInViewModel : ObservableObject
    {
        [RelayCommand]
        private void ExportSnapshot()
        {
            string outputPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string snapshotPath = RevitIssue.ExportSnapshot(outputPath);

            if (!string.IsNullOrWhiteSpace(snapshotPath))
            {
                MessageBox.Show($"Snapshot saved to: {snapshotPath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Failed to export snapshot.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void ShowCameraPosition()
        {
            string cameraPosition = RevitIssue.GetCameraPosition();
            MessageBox.Show($"Camera Position: {cameraPosition}", "View3D Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void ShowSelectedElementIds()
        {
            List<string> ids = RevitIssue.GetSelectedElementIds();

            if (ids.Count == 0)
            {
                MessageBox.Show("No elements selected.", "Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                string joinedIds = string.Join(", ", ids);
                MessageBox.Show($"Selected Element IDs: {joinedIds}", "Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private readonly AsyncEventHandler _asyncEventHandler = new();

        [RelayCommand]
        private async Task IsolateSelectionInSectionBoxAsync()
        {
            await _asyncEventHandler.RaiseAsync(app =>
            {
                var uidoc = Context.ActiveUiDocument;
                var doc = uidoc.Document;
                var selectedIds = uidoc.Selection.GetElementIds();

                if (!selectedIds.Any())
                {
                    MessageBox.Show("Revit", "Please select elements first.");
                    return;
                }

                // === Bounding Box Calculation ===
                BoundingBoxXYZ? combinedBox = null;
                foreach (var id in selectedIds)
                {
                    var element = doc.GetElement(id);
                    var box = element?.get_BoundingBox(null);
                    if (box == null) continue;

                    combinedBox ??= new BoundingBoxXYZ { Min = box.Min, Max = box.Max };

                    combinedBox.Min = new XYZ(
                        Math.Min(combinedBox.Min.X, box.Min.X),
                        Math.Min(combinedBox.Min.Y, box.Min.Y),
                        Math.Min(combinedBox.Min.Z, box.Min.Z));

                    combinedBox.Max = new XYZ(
                        Math.Max(combinedBox.Max.X, box.Max.X),
                        Math.Max(combinedBox.Max.Y, box.Max.Y),
                        Math.Max(combinedBox.Max.Z, box.Max.Z));
                }

                if (combinedBox == null)
                {
                    MessageBox.Show("Revit", "Could not compute bounding box.");
                    return;
                }

                // === Create Isolated 3D View ===
                var viewType = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewFamilyType))
                    .Cast<ViewFamilyType>()
                    .FirstOrDefault(v => v.ViewFamily == ViewFamily.ThreeDimensional);

                if (viewType is null)
                {
                    MessageBox.Show("Revit", "Cannot find 3D view type.");
                    return;
                }

                View3D view3D;
                using (Transaction tx = new Transaction(doc, "Create Isolated 3D View"))
                {
                    tx.Start();

                    view3D = View3D.CreateIsometric(doc, viewType.Id);
                    view3D.Name = $"IsolatedView_{DateTime.Now:HHmmss}";
                    view3D.SetSectionBox(combinedBox);
                    view3D.CropBoxActive = true;
                    view3D.CropBoxVisible = false;
                    view3D.IsolateElementsTemporary(selectedIds.ToList());

                    // === Zoom & Orientation Logic ===
                    var center = (combinedBox.Min + combinedBox.Max) / 2;

                    //////////////////////////////
                    //var size = (combinedBox.Max - combinedBox.Min).GetLength();
                    //var offset = size * 0.3; 
                    var eyePosition = center + new XYZ(0.5, -0.5, 0.5);

                    var forwardDirection = (center - eyePosition).Normalize();

                    
                    var right = forwardDirection.CrossProduct(XYZ.BasisZ).Normalize();
                    var upDirection = right.CrossProduct(forwardDirection).Normalize();

                    view3D.SetOrientation(new ViewOrientation3D(eyePosition, upDirection, forwardDirection));

                    tx.Commit();
                }

                uidoc.ActiveView = view3D;

                MessageBox.Show("Revit", "Isolated view created!");
            });
        }



    }
}
