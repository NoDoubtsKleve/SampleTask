using System.Windows;


namespace SampleTask
{
    /// <summary>
    /// Interaction logic for FrameProfileWindow.xaml
    /// </summary>
    public partial class FrameProfileWindow : Window
    {
        public FrameProfileWindow()
        {
            InitializeComponent();

            
        }
        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
