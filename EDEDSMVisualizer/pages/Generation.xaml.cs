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
using System.ComponentModel;
using System.Windows.Shapes;
using System.IO;
using Newtonsoft.Json;


namespace EDEDSMVisualizer.pages
{
    /// <summary>
    /// Interaktionslogik für Generation.xaml
    /// </summary>
    public partial class Generation : UserControl
    {
        classes.Settings settings;
        uint[][] sysmap;
        static long counter;
        System.Threading.Tasks.Task task;

        public Generation()
        {
            InitializeComponent();
            settings = ((App)Application.Current).GetSettings();
            //Initialize 2dArray to hold system position data
            Write("Generating 2D Array");
            
            
            sysmap = CreateJaggedArray<uint[][]>(Convert.ToInt32(settings.Img_Xres),Convert.ToInt32(settings.Img_Xres));

             task = Task.Factory.StartNew(() => ParseJSON());
            
           


            
           
            


        }
        void Write(String s)
        {
            info_output.Content = s;
        }
        static T CreateJaggedArray<T>(params int[] lengths)
        {
            return (T)InitializeJaggedArray(typeof(T).GetElementType(), 0, lengths);
        }
        static object InitializeJaggedArray(Type type, int index, int[] lengths)
        {
            Array array = Array.CreateInstance(type, lengths[index]);
            Type elementType = type.GetElementType();

            if (elementType != null)
            {
                for (int i = 0; i < lengths[index]; i++)
                {
                    array.SetValue(
                        InitializeJaggedArray(elementType, index + 1, lengths), i);
                }
            }

            return array;
        }
      
        void UpdateThread(object sender, DoWorkEventArgs e) {
                
        }
         void ParseJSON()
        {
            
            
            #region description
            /* How the JSON is set up:
                 * 
                 * [...]
                 * x : VALUE
                 * y : VALUE
                 * z : VLAUE
                 * [...]
                 * 
                 * We need x and z.
                 * The StreamReader reads it like this:
                 * x
                 * VALUE
                 * y
                 * VALUE
                 * z
                 * VALUE
                 * .. So we look for "x" only (skiptoX = true), if we find it we use the next reading iteration for its value (thats why there is the boolean "lastX"), skip 3 iterations (for y ; VALUE; z;) and take the next value.
                 * 
                 */
            #endregion
            bool skiptoX = true, lastX = false;
            byte count = 0;
            long x = 0, y = 0;
            JsonTextReader streamReader = new JsonTextReader(new StreamReader(settings.Path_json));
            while (streamReader.Read())
            {
                if (streamReader.Value != null)
                {
                    if (skiptoX)
                    {
                        if (String.Equals(streamReader.Value, "x"))
                        {
                            lastX = true;
                            skiptoX = false;
                        }
                    }
                    else if (lastX)
                    {
                        x = Convert.ToInt64(streamReader.Value);
                        lastX = false;
                        count = 1;
                    }


                    else if (count > 0 && count < 4) // Count 3 Iterations (we starts with count = 1)
                    {
                        count++;
                    }


                    else if (count == 4) // After 3 Iterations, we read the value and pass it and x to AddToBitmap method while displaying the process on the Console.
                    {
                        y = Convert.ToInt64(streamReader.Value);

                        skiptoX = true;
                        count = 0;
                        int _x = (int)x + settings.Img_X_offset;
                        int _y = (int)y + settings.Img_Y_offset;
                        if (_x > 0 && _y > 0 && _x < settings.Img_Xres && _y < settings.Img_Yres) { sysmap[_x - 1][_y - 1]++; }

                        counter++;
                        if (counter % 10045 == 0)
                        {
                            Dispatcher.BeginInvoke((Action)(() => { Write($"{counter} Systems parsed."); })); 
                        }


                    }


                }
                
            }
            Dispatcher.BeginInvoke((Action)(() => { Write($"{counter} Systems parsed. \n Applying to Bitmap."); }));


        }
    }
    
}
