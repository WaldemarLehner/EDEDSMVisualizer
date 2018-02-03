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
        static classes.Settings settings;
        public void PassMainWindow(MainWindow window)
        {
            mainWindow = window;
        }
        public MainWindow GetMainWindow()
        {
            return mainWindow;
        }
        public void PassSettings(classes.Settings _settings)
        {
            settings = _settings;
        }
        public classes.Settings GetSettings()
        {
            return settings;
        }

    }
}
