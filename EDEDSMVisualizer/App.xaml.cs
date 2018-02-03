using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EDEDSMVisualizer
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        static MainWindow mainWindow;
        public void PassMainWindow(MainWindow window)
        {
            MainWindow = window;
        }
        public MainWindow GetMainWindow()
        {
            return mainWindow;
        }
    }
}
