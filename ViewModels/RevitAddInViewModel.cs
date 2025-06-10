using IssueManager.Revit;
using Nice3point.Revit.Toolkit.External.Handlers;

namespace IssueManager.ViewModels
{
    public partial class RevitAddInViewModel : ObservableObject
    {
        [RelayCommand]
        private async Task ExportSnapshotAsync()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), $"Snapshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");

            string snapshotPath = RevitIssue.ExportSnapshot(tempPath);
            if (string.IsNullOrWhiteSpace(snapshotPath))
            {
                MessageBox.Show("Failed to export snapshot.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using var multipart = new MultipartFormDataContent();
                multipart.Add(new StreamContent(File.OpenRead(snapshotPath)), "file", Path.GetFileName(snapshotPath));

                using var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:44374/");
                var response = await client.PostAsync("/api/Snapshot/upload", multipart);
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Upload failed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var imageUrl = await response.Content.ReadAsStringAsync();
                MessageBox.Show("Snapshot uploaded and set!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error uploading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            //List<string> ids = RevitIssue.GetSelectedElementIds();

            //if (ids.Count == 0)
            //{
            //    MessageBox.Show("No elements selected.", "Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}
            //else
            //{
            //    string joinedIds = string.Join(", ", ids);
            //    MessageBox.Show($"Selected Element IDs: {joinedIds}", "Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
        }

        private readonly AsyncEventHandler _asyncEventHandler = new();

        [RelayCommand]
        private async Task IsolateSelectionInSectionBoxAsync()
        {
            await _asyncEventHandler.RaiseAsync(app =>
            {
                var uidoc = Context.ActiveUiDocument;
                var doc = uidoc.Document;
                var view = uidoc.ActiveView;

                if (view is not View3D view3D || view.ViewType != ViewType.ThreeD || view3D.IsTemplate)
                {
                    MessageBox.Show("Revit", "Please activate a 3D graphical view first.");
                    return;
                }

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
                    var box = element?.get_BoundingBox(view);
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


                var uiView = new FilteredElementCollector(doc)
                    .OfClass(typeof(View)).Cast<View>()
                    .Where(v => v.Id == uidoc.ActiveView.Id)
                    .Select(v => uidoc.GetOpenUIViews().FirstOrDefault(uiv => uiv.ViewId == v.Id))
                    .FirstOrDefault();

                if (uiView is not null)
                {
                    // Calculate size of bounding box
                    XYZ size = combinedBox.Max - combinedBox.Min;

                    // Define a small percentage offset (e.g., 10% of the box size)
                    double offsetFactor = 0.1;
                    XYZ offset = new XYZ(
                        size.X * offsetFactor,
                        size.Y * offsetFactor,
                        size.Z * offsetFactor);

                    // Apply offset to Min and Max to give some padding around the box
                    XYZ zoomMin = combinedBox.Min - offset;
                    XYZ zoomMax = combinedBox.Max + offset;

                    uiView.ZoomAndCenterRectangle(zoomMin, zoomMax);
                }


                using (var tx = new Transaction(doc, "Isolate in Section Box"))
                {
                    tx.Start();

                    view3D.SetSectionBox(combinedBox);
                    view3D.CropBoxActive = true;
                    view3D.CropBoxVisible = true;
                    view3D.IsolateElementsTemporary(selectedIds.ToList());

                    tx.Commit();
                }

                MessageBox.Show("Revit", "Isolated elements in section box in current view.");
            });
        }
    }
}
