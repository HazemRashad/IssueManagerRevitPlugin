namespace IssueManager.Resources
{
    public static class CardButtonHelper
    {
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.RegisterAttached("ImageSource", typeof(ImageSource), typeof(CardButtonHelper), new PropertyMetadata(null));

        public static ImageSource GetImageSource(DependencyObject obj) => (ImageSource)obj.GetValue(ImageSourceProperty);
        public static void SetImageSource(DependencyObject obj, ImageSource value) => obj.SetValue(ImageSourceProperty, value);

        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.RegisterAttached("Subtitle", typeof(string), typeof(CardButtonHelper), new PropertyMetadata(string.Empty));

        public static string GetSubtitle(DependencyObject obj) => (string)obj.GetValue(SubtitleProperty);
        public static void SetSubtitle(DependencyObject obj, string value) => obj.SetValue(SubtitleProperty, value);
    }
}


