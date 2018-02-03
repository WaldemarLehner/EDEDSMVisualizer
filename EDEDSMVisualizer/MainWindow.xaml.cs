using System;
using System.Collections.Generic;
using System.Linq;
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
using MahApps.Metro.Controls;


namespace EDEDSMVisualizer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ((App)Application.Current).PassMainWindow(this); // Pass Reference to this Instance of MainWindow so other classes can find it and set its content
            ChangeUI(new pages.MainMenu());
        }
        public void ChangeUI<T>(T page)
        {
            this.contentControl.Content = page;
        }
    }
}
