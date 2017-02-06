using System.Windows;
using ES.Manager.ViewModels;

namespace ES.Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();
            DataContext = new ShellViewModel();
        }
    }
}
