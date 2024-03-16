using DesktopPart.Bootstrapper;
using System.Configuration;
using System.Data;
using System.Windows;

namespace DesktopPart
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Booststraper? _bootstrapper;
        protected override void OnStartup(StartupEventArgs e)
        {
            _bootstrapper = new Booststraper();
            MainWindow = _bootstrapper.Run();

        }
    }

}
