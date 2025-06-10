using DTOs.RevitElements;

namespace IssueManager.Revit
{
    public static class RevitIssue
    {
        public static string ExportSnapshot(string fullOutputPath)
        {
            var doc = Context.Document;
            var view = Context.ActiveView;

            var options = new ImageExportOptions
            {
                ZoomType = ZoomFitType.Zoom,
                PixelSize = 1920,
                FilePath = Path.ChangeExtension(fullOutputPath, null),
                FitDirection = FitDirectionType.Horizontal,
                ExportRange = ExportRange.VisibleRegionOfCurrentView,
                HLRandWFViewsFileType = ImageFileType.PNG,
                ShadowViewsFileType = ImageFileType.PNG,
                ImageResolution = ImageResolution.DPI_150
            };

            try
            {
                doc.ExportImage(options);
                return fullOutputPath;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static List<RevitElementDto> GetSelectedRevitElements()
        {
            var uidoc = Context.ActiveUiDocument;
            var doc = uidoc.Document;
            var view = Context.ActiveView as View3D;

            var orientation = view?.GetOrientation();

            string eye = $"{orientation?.EyePosition.X:F3},{orientation?.EyePosition.Y:F3},{orientation?.EyePosition.Z:F3}";
            string forward = $"{orientation?.ForwardDirection.X:F3},{orientation?.ForwardDirection.Y:F3},{orientation?.ForwardDirection.Z:F3}";
            string up = $"{orientation?.UpDirection.X:F3},{orientation?.UpDirection.Y:F3},{orientation?.UpDirection.Z:F3}";

            return uidoc.Selection
                .GetElementIds()
                .Select(id =>
                {
                    var element = doc.GetElement(id);
                    return new RevitElementDto
                    {
                        ElementId = id.IntegerValue.ToString(),
                        ElementUniqueId = element?.UniqueId ?? "",
                        ViewpointCameraPosition = eye,
                        ViewpointForwardDirection = forward,
                        ViewpointUpDirection = up
                    };
                })
                .ToList();
        }

        public static void ZoomToElementIdsInView(View view, List<ElementId> elementIds)
        {
            var doc = Context.Document;
            var uidoc = Context.ActiveUiDocument;

            var uiView = uidoc.GetOpenUIViews().FirstOrDefault(v => v.ViewId == view.Id);
            if (uiView == null) return;

            BoundingBoxXYZ? combinedBox = null;

            foreach (var id in elementIds)
            {
                var element = doc.GetElement(id);
                var box = element?.get_BoundingBox(view);
                if (box == null) continue;

                if (combinedBox == null)
                {
                    combinedBox = new BoundingBoxXYZ { Min = box.Min, Max = box.Max };
                }
                else
                {
                    combinedBox.Min = new XYZ(
                        Math.Min(combinedBox.Min.X, box.Min.X),
                        Math.Min(combinedBox.Min.Y, box.Min.Y),
                        Math.Min(combinedBox.Min.Z, box.Min.Z));

                    combinedBox.Max = new XYZ(
                        Math.Max(combinedBox.Max.X, box.Max.X),
                        Math.Max(combinedBox.Max.Y, box.Max.Y),
                        Math.Max(combinedBox.Max.Z, box.Max.Z));
                }
            }

            if (combinedBox != null)
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

            ZoomToElementIdsInView(view3D, selectedIds.ToList()); 

            using (var tx = new Transaction(doc, "Isolate in Section Box"))
            {
                tx.Start();

                var box = view3D.GetSectionBox();
                view3D.SetSectionBox(box);
                view3D.CropBoxActive = true;
                view3D.CropBoxVisible = true;
                view3D.IsolateElementsTemporary(selectedIds.ToList());

                tx.Commit();
            }

            MessageBox.Show("Isolated elements in section box in current view.", "Revit");
        }


        public static void NavigateToViewPointAndSelectElements(IssueDto issue)
        {
            var uidoc = Context.ActiveUiDocument;
            var doc = uidoc.Document;

            var elementIds = new List<ElementId>();

            foreach (var revitElement in issue.RevitElements ?? new())
            {
                ElementId? id = null;

                if (!string.IsNullOrEmpty(revitElement.ElementUniqueId))
                {
                    var element = doc.GetElement(revitElement.ElementUniqueId);
                    if (element != null)
                        id = element.Id;
                }

                if (id == null && int.TryParse(revitElement.ElementId, out int intId))
                {
                    id = new ElementId(intId);
                }

                if (id != null)
                    elementIds.Add(id);
            }

            if (!elementIds.Any())
            {
                MessageBox.Show("No matching elements found in Revit.", "Revit");
                return;
            }

            uidoc.Selection.SetElementIds(elementIds);

            var view = uidoc.ActiveView;

            if (view.IsTemplate || !(view.ViewType == ViewType.ThreeD))
            {
                MessageBox.Show("Please open a 3D view in Revit first.", "Revit");
                return;
            }

            if (view is View3D view3D && !view3D.IsTemplate)
            {
                var revitElement = issue.RevitElements?.FirstOrDefault();

                if (revitElement is not null &&
                    !string.IsNullOrWhiteSpace(revitElement.ViewpointCameraPosition) &&
                    !string.IsNullOrWhiteSpace(revitElement.ViewpointForwardDirection) &&
                    !string.IsNullOrWhiteSpace(revitElement.ViewpointUpDirection))
                {
                    try
                    {
                        var eyeCoords = revitElement.ViewpointCameraPosition.Split(',').Select(double.Parse).ToArray();
                        var forwardCoords = revitElement.ViewpointForwardDirection.Split(',').Select(double.Parse).ToArray();
                        var upCoords = revitElement.ViewpointUpDirection.Split(',').Select(double.Parse).ToArray();

                        if (eyeCoords.Length == 3 && forwardCoords.Length == 3 && upCoords.Length == 3)
                        {
                            XYZ eye = new XYZ(eyeCoords[0], eyeCoords[1], eyeCoords[2]);
                            XYZ forward = new XYZ(forwardCoords[0], forwardCoords[1], forwardCoords[2]);
                            XYZ up = new XYZ(upCoords[0], upCoords[1], upCoords[2]);

                            using var tx = new Transaction(doc, "Set Camera Orientation");
                            tx.Start();
                            view3D.SetOrientation(new ViewOrientation3D(eye, up, forward));
                            tx.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error restoring camera orientation: {ex.Message}", "Revit");
                    }
                }

                ZoomToElementIdsInView(view3D, elementIds);
            }
        }


    }
}

