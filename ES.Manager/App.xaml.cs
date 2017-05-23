using System;
using System.Windows;

namespace ES.Manager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);
                this.DispatcherUnhandledException += Application_DispatcherUnhandledException;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Application error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (Application.Current != null) Current.Shutdown();
            }
            catch
            {
                MessageBox.Show("Unknown exception.", "Application error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (Application.Current != null) Current.Shutdown();
            }
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var dictionary = new ResourceDictionary { Source = new Uri("Es.Manager;component/Views/Generic.xaml", UriKind.Relative) };
            Application.Current.Resources.MergedDictionaries.Add(dictionary);
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
            e.Handled = true;
        }
    }

}
