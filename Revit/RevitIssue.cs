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
                ExportRange = ExportRange.CurrentView,
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
        }
    }
}

