using DesktopPart.View;
using DesktopPart.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopPart.ViewModel;
using System.Windows;
using DesktopPart.Service;

namespace DesktopPart.Bootstrapper
{
    public class Booststraper
    {
        public Window Run()
        {
            var mainWindow = new MainWindow();

            mainWindow.DataContext = new MainViewModel(new ExelService());
            mainWindow.Show();
            return mainWindow;
        }
    }
}
