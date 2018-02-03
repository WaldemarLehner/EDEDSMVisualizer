using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

namespace EDEDSMVisualizer.pages
{
    /// <summary>
    /// Interaktionslogik für MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        String pathtoinput, pathtooutput;
        
        public MainMenu()
        {
            
            InitializeComponent();
        }

        private void SetOutputLocation(object sender, MouseButtonEventArgs e)
        {
            // Open Dialog to choose file output location
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.ShowNewFolderButton = true;
            
            if(dialog.ShowDialog() == true)
            {
                pathtooutput = dialog.SelectedPath;
                if (Directory.Exists(pathtooutput))
                {
                    icon_setoutput.Icon = FontAwesome.WPF.FontAwesomeIcon.Check;
                }
                else
                {
                    icon_setoutput.Icon = FontAwesome.WPF.FontAwesomeIcon.Close;
                }
            }
        }

        private void SetInputLocation(object sender, MouseButtonEventArgs e)
        {
            // Open Dialog to choose file input location
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "EDSM JSON|systemsWithCoordinates.json";
            if (dialog.ShowDialog() == true)
            {
                pathtoinput = dialog.FileName;
                if (File.Exists(pathtoinput))
                {
                    FileInfo info = new FileInfo(pathtoinput);
                    if(info.Length > 1000000) // File needs to be larger than 1gb
                    {
                        icon_setinput.Icon = FontAwesome.WPF.FontAwesomeIcon.Check;
                    }
                    else
                    {
                        icon_setinput.Icon = FontAwesome.WPF.FontAwesomeIcon.Close;
                    }
                }
                else
                {
                    icon_setinput.Icon = FontAwesome.WPF.FontAwesomeIcon.Close;
                }
            }
        }
    }
}
