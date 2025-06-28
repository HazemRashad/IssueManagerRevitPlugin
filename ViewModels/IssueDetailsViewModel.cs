using System.Windows.Threading;

namespace IssueManager.ViewModels
{
    public partial class IssueDetailsViewModel : ObservableObject
    {
        private readonly IssueApiService _issueApiService;

        public IssueDetailsViewModel(
            IssueApiService issueApiService)
        {
            _issueApiService = issueApiService;
        }

        [ObservableProperty]
        private IssueDto? issue;

        [RelayCommand]
        private async Task MarkAsResolved()
        {
            if (Issue == null || Issue.IsResolved)
                return;

            Issue.IsResolved = true;
            await _issueApiService.UpdateResolvedStatusAsync(Issue.IssueId, true);
            OnPropertyChanged(nameof(Issue));
        }

        [RelayCommand]
        private async Task DeleteIssue()
        {
            if (Issue == null)
                return;

            await _issueApiService.DeleteAsync(Issue.IssueId);
            //RequestClose.Invoke();
            MessageBox.Show("Issue deleted.");
        }

        public async Task LoadIssueAsync(int issueId)
        {
            Issue = await _issueApiService.GetByIdAsync(issueId);
        }

        [RelayCommand]
        private async Task AddCommentAsync()
        {
            if (Issue is null) return;

            var command = new CommentCommand(Issue, async () =>
            {
                var updated = await _issueApiService.GetByIdAsync(Issue.IssueId);
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    Issue.Comments = updated.Comments;
                    OnPropertyChanged(nameof(Issue));
                });

            });

            command.Execute();
        }


    }

}
