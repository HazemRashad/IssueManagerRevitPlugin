using DTOs.Comments;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace IssueManager.ViewModels
{
    public partial class CommentViewModel : ObservableObject
    {
        private readonly CommentApiService _commentApiService;

        [ObservableProperty]
        private string commentText;

        [ObservableProperty]
        private ObservableCollection<CommentDto> issueComments = new();

        public int IssueId { get; }

        public Action? OnCommentAdded { get; set; }

        public CommentViewModel(CommentApiService commentApiService, int issueId)
        {
            _commentApiService = commentApiService;
            IssueId = issueId;

            _ = LoadIssueCommentsAsync();
        }

        [RelayCommand]
        private async Task SubmitAsync()
        {
            if (!string.IsNullOrWhiteSpace(CommentText))
            {
                var dto = new CreateCommentDto
                {
                    Message = CommentText,
                    IssueId = IssueId,
                    SnapshotId = null,
                    CreatedByUserId = AppSession.UserId
                };

                var result = await _commentApiService.CreateForIssueAsync(IssueId, dto);
                if (result != null)
                {
                    CommentText = string.Empty;

                    await LoadIssueCommentsAsync();

                    OnCommentAdded?.Invoke();
                }
                else
                {
                    MessageBox.Show("Failed to add comment");
                }
            }
        }

        public async Task LoadIssueCommentsAsync()
        {
            try
            {
                var comments = await _commentApiService.GetByIssueIdAsync(IssueId);

                IssueComments.Clear();
                foreach (var comment in comments)
                    IssueComments.Add(comment);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load comments: {ex.Message}");
            }
        }
    }
}
