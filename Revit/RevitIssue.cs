namespace IssueManager.Revit
{
    public static class RevitIssue
    {
        public static string ExportSnapshot(string path)
        {
            var doc = Context.Document;
            var view = Context.ActiveView;

            string fileName = $"Snapshot_{view.Name}_{DateTime.Now:yyyyMMdd_HHmmss}";
            string fullPath = Path.Combine(path, fileName);

            var options = new ImageExportOptions
            {
                ZoomType = ZoomFitType.Zoom,
                PixelSize = 1920,
                FilePath = fullPath,
                FitDirection = FitDirectionType.Horizontal,
                ExportRange = ExportRange.VisibleRegionOfCurrentView,
                HLRandWFViewsFileType = ImageFileType.PNG,
                ShadowViewsFileType = ImageFileType.PNG,
                ImageResolution = ImageResolution.DPI_150
            };

            try
            {
                doc.ExportImage(options);
                return fullPath + ".png";
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string GetCameraPosition()
        {
            var view = Context.ActiveView;

            if (view is View3D view3D)
            {
                XYZ eyePosition = view3D.GetOrientation().EyePosition;
                return $"{eyePosition.X:F3},{eyePosition.Y:F3},{eyePosition.Z:F3}";
            }

            return "0,0,0";
        }

        public static List<string> GetSelectedElementIds()
        {
            return Context.ActiveUiDocument.Selection
                .GetElementIds()
                .Select(id => id.IntegerValue.ToString())
                .ToList();
        }

        public static void IsolateSelectionInSectionBox()
        {
            var uidoc = Context.ActiveUiDocument;
            var doc = uidoc.Document;
            var view = uidoc.ActiveView;

            if (view is not View3D view3D || view.ViewType != ViewType.ThreeD || view3D.IsTemplate)
            {
                MessageBox.Show("Please activate a 3D graphical view first.", "Revit");
                return;
            }

            var selectedIds = uidoc.Selection.GetElementIds();
            if (!selectedIds.Any())
            {
                MessageBox.Show("Please select elements first.", "Revit");
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
                MessageBox.Show("Could not compute bounding box.", "Revit");
                return;
            }


            var uiView = new FilteredElementCollector(doc)
                .OfClass(typeof(View)).Cast<View>()
                .Where(v => v.Id == uidoc.ActiveView.Id)
                .Select(v => uidoc.GetOpenUIViews().FirstOrDefault(uiv => uiv.ViewId == v.Id))
                .FirstOrDefault();

            if (uiView is not null)
            {
                XYZ size = combinedBox.Max - combinedBox.Min;

                double offsetFactor = 0.1;
                XYZ offset = new XYZ(
                    size.X * offsetFactor,
                    size.Y * offsetFactor,
                    size.Z * offsetFactor);

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

            MessageBox.Show("Isolated elements in section box in current view.", "Revit");
        }


    }
}

