using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DTOs.Comments;
using DTOs.IssueLabel;
using DTOs.Issues;
using DTOs.Labels;
using DTOs.RevitElements;
using IssueManager.Srevices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace IssueManager.ViewModels
{
    [INotifyPropertyChanged]
    public sealed partial class AddIssueViewModel
    {
        private readonly ApiService _apiService = new();

        // === Form Fields ===
        private string title;
        private string? description;
        private string? assignedToUserId;
        private int areaId;
        private Priority priority;
        private DateTime? deadline = DateTime.Today;
        private string? snapshotImagePath;

        // === Project Context ===
        public int ProjectId { get; set; } = 1;
        public string CreatedByUserId { get; set; } = "current-user-id";

        // === Labels ===
        public ObservableCollection<LabelDto> AvailableLabels { get; set; } = new();
        public ObservableCollection<AssignLabelToIssueDto> SelectedLabels { get; set; } = new();

        // === Comments ===
        private string? commentContent;

        // === Commands ===
        public IRelayCommand SaveCommand => new AsyncRelayCommand(SaveAsync);
        //public IRelayCommand CancelCommand => new RelayCommand(() => CloseWindow());

        private async Task SaveAsync()
        {
            var dto = new CreateIssueDto
            {
                Title = title,
                Description = description,
                AssignedToUserId = assignedToUserId,
                AreaId = areaId,
                Priority = priority,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = CreatedByUserId,
                ProjectId = ProjectId,
                LabelIds = SelectedLabels.Select(l => l.LabelId).ToList(),
                RevitElements = string.IsNullOrWhiteSpace(snapshotImagePath)
                    ? new List<RevitElementDto>()
                    : new List<RevitElementDto>
                    {
                        new()
                        {
                            ElementId = "123",
                            ElementUniqueId = "ABC-123",
                            SnapshotImagePath = snapshotImagePath,
                            ViewpointCameraPosition = "0,0,0"
                        }
                    }
            };

            bool result = await _apiService.CreateIssueAsync(dto);
            if (result)
            {
                MessageBox.Show("Issue created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                //CloseWindow();
            }
            else
            {
                MessageBox.Show("Failed to create issue.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //private void CloseWindow()
        //{
        //    Application.Current.Windows
        //        .OfType<Window>()
        //        .FirstOrDefault(w => w.DataContext == this)?.Close();
        //}
    }
}
