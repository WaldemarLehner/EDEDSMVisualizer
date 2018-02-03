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
        Boolean allowstart = false;
        Byte outputrenderingtype = 0;
        public MainMenu()
        {
            
            InitializeComponent();
        }

        private void SetOutputLocation(object sender, MouseButtonEventArgs e)
        {
            // Open Dialog to choose file output location
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog
            {
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() == true)
            {
                pathtooutput = dialog.SelectedPath;
                if (Directory.Exists(pathtooutput))
                {
                    icon_setoutput.Icon = FontAwesome.WPF.FontAwesomeIcon.Check;
                    if (!String.IsNullOrEmpty(pathtoinput))
                    {
                        allowstart = true;
                        icon_startcalc.Icon = FontAwesome.WPF.FontAwesomeIcon.CheckCircleOutline;
                    }
                }
                else
                {
                    icon_setoutput.Icon = FontAwesome.WPF.FontAwesomeIcon.Close;
                    pathtooutput = null;
                    allowstart = false;
                    icon_startcalc.Icon = FontAwesome.WPF.FontAwesomeIcon.Close;
                }
            }
        }
        private void SetInputLocation(object sender, MouseButtonEventArgs e)
        {
            // Open Dialog to choose file input location
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "EDSM JSON|systemsWithCoordinates.json"
            };
            if (dialog.ShowDialog() == true)
            {
                pathtoinput = dialog.FileName;
                if (File.Exists(pathtoinput))
                {
                    FileInfo info = new FileInfo(pathtoinput);
                    if(info.Length > 1000000) // File needs to be larger than 1gb
                    {
                        icon_setinput.Icon = FontAwesome.WPF.FontAwesomeIcon.Check;
                        if (!String.IsNullOrEmpty(pathtooutput))
                        {
                            icon_startcalc.Icon = FontAwesome.WPF.FontAwesomeIcon.CheckCircleOutline;
                            allowstart = true;
                        }

                    }
                    else
                    {
                        icon_setinput.Icon = FontAwesome.WPF.FontAwesomeIcon.Close;
                        pathtoinput = null;
                        icon_startcalc.Icon = FontAwesome.WPF.FontAwesomeIcon.Close;
                        allowstart = false;
                        MessageBox.Show("Error: File is < 1gb and is not a systemsWithCoordinates.json");
                    }
                }
                else
                {
                    icon_setinput.Icon = FontAwesome.WPF.FontAwesomeIcon.Close;
                    pathtoinput = null;
                    icon_startcalc.Icon = FontAwesome.WPF.FontAwesomeIcon.Close;
                    allowstart = false;
                    MessageBox.Show("Error: File doesn't seem to exist");
                }
            }
        }
#region output rendering type
        private void outputtypechanged(object sender, MouseButtonEventArgs e)
        {

        }

        private void output_select_greyscale(object sender, MouseButtonEventArgs e)
        {
            outputrenderingtype = 1;
        }

        private void output_select_hue(object sender, MouseButtonEventArgs e)
        {
            outputrenderingtype = 2;
        }

        private void output_select_hueandvalue(object sender, MouseButtonEventArgs e)
        {
            outputrenderingtype = 3;
        }

        private void output_select_red(object sender, MouseButtonEventArgs e)
        {
            outputrenderingtype = 4;
        }

        private void output_select_green(object sender, MouseButtonEventArgs e)
        {
            outputrenderingtype = 5;
        }

        private void output_select_blue(object sender, MouseButtonEventArgs e)
        {
            outputrenderingtype = 6;
        }

        private void output_select_alpha(object sender, MouseButtonEventArgs e)
        {
            outputrenderingtype = 7;
        }
#endregion

        private void StartImageCalculation(object sender, MouseButtonEventArgs e)
        {
            if (!allowstart)
            {
                MessageBox.Show("Error: Either input or output are not defined yet");
            }
            else
            {
#region Prepare Image Generation. Create a Settings Object and populate it
                // Get all settings, store them in App Superclass
                classes.Settings settings = new classes.Settings();
                if(outputrenderingtype == 0 || outputrenderingtype > 7)
                {
                    settings.Rendering = 1;
                }
                else
                {
                    settings.Rendering = outputrenderingtype;
                }
                settings.Path_json = pathtoinput;
                settings.Path_output = pathtooutput;
                settings.Img_Xres = (xres.Value != null) ? (uint)xres.Value : 10000;
                settings.Img_Yres = (xres.Value != null) ? (uint)yres.Value : 10000;
                settings.Img_X_offset = (xoffset.Value != null)?(int)xoffset.Value:5000;
                settings.Img_Y_offset = (yoffset.Value != null)?(int)yoffset.Value:2000;
                settings.Ly_to_px = (lyppx.Value != null)?(int)lyppx.Value:10;
                settings.Systems_per_ColorVal = (syspval.Value != null)?(int)syspval.Value:1;

                settings.Axial5kly = (bool)ax5kly.IsChecked;
                settings.Axial10kly = (bool)ax10kly.IsChecked;
                settings.Radial5kly = (bool)rad5kly.IsChecked;
                settings.Radial10kly = (bool)rad10kly.IsChecked;
                settings.Scaling = (bool)scaling.IsChecked;
                settings.ColorTable = (bool)colortable.IsChecked;
                settings.EDSMIcon = (bool)edsmicon.IsChecked;
                ((App)Application.Current).PassSettings(settings);
                (((App)Application.Current).GetMainWindow()).ChangeUI(new pages.Generation());
                #endregion
            }
        }
    }
}
