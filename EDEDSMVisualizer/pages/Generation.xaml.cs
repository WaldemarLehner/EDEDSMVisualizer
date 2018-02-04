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

using System.Windows.Navigation;
using System.ComponentModel;

using ColorMine.ColorSpaces;
using System.Drawing;
using FastBitmapLib;

using System.IO;
using Newtonsoft.Json;
using System.Drawing.Drawing2D;

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
        FastBitmap bitmap;

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
                        int _x = ((int)x / settings.Ly_to_px + settings.Img_X_offset);
                        int _y = ((int)y / settings.Ly_to_px + settings.Img_Y_offset);
                        if (_x > 0 && _y > 0 && _x < settings.Img_Xres && _y < settings.Img_Yres) {
                            
                                sysmap[_x - 1][(_y - 1)+2*(settings.Img_Yres/2-_y)]++;
                            
                            
                            
                        }

                        counter++;
                        if (counter % 10045 == 0)
                        {
                            Dispatcher.BeginInvoke((Action)(() => { Write($"{counter} Systems parsed."); })); 
                        }


                    }


                }
                
            }
            Dispatcher.BeginInvoke((Action)(() => { Write($"{counter} Systems parsed. \n Applying to Bitmap."); }));

            // Plot 2dArray onto bitmap, using selected method
             bitmap = new FastBitmap((int)settings.Img_Xres,(int)settings.Img_Yres);
            
            /*
            * 1:Greyscale
             * 2:Hue
             * 3:Hue+Value
             * 4:r
             * 5:g
             * 6:b
             * 7:a
             */
            #region using Greyscale
            if(settings.Rendering == 1 || settings.Rendering == 0)
            {
                int index = 0;
                bitmap.Clear(FastColor.black);
                for(int iteration_x = 0; iteration_x < sysmap.Length; iteration_x++)
                {
                    for(int iteration_y = 0; iteration_y < sysmap[iteration_x].Length; iteration_y++)
                    {
                        uint value = Convert.ToUInt32(sysmap[iteration_x][iteration_y]*settings.Systems_per_ColorVal);
                        byte greyscale;
                        if (value > 255)
                        {
                            greyscale = 255;
                        }
                        else
                        {
                            greyscale = (byte)value;
                        }
                        bitmap.SetPixel(iteration_x + 1, iteration_y + 1, new FastColor(greyscale, greyscale, greyscale));
                    }
                }
                index++;
                if (index % 10045 == 0)
                {
                    Dispatcher.BeginInvoke((Action)(() => { Write($"{index} pixels set."); }));
                }

            }
            #endregion
            #region using Hue(but no Value)
            else if (settings.Rendering == 2)
            {
                int index = 0;
                bitmap.Clear(FastColor.black);
                for (int iteration_x = 0; iteration_x < sysmap.Length; iteration_x++)
                {
                    for (int iteration_y = 0; iteration_y < sysmap[iteration_x].Length; iteration_y++)
                    {
                        uint value = Convert.ToUInt32(sysmap[iteration_x][iteration_y] * settings.Systems_per_ColorVal);
                        
                        
                        if(value > 180) { value = value - 180; } // MOve Hue by 180 Degreen so blue is 0 and Red is maximum
                        else { value = value + 180; }
                        var color = new Hsv((value+180)/2, 1, 1).ToRgb();
                        
                        bitmap.SetPixel(iteration_x + 1, iteration_y + 1, new FastColor((byte)color.R, (byte)color.G, (byte)color.B));
                    }
                }
                index++;
                if (index % 10045 == 0)
                {
                    Dispatcher.BeginInvoke((Action)(() => { Write($"{index} pixels set."); }));
                }

            }


            #endregion
            #region using Hue and Value
            else if (settings.Rendering == 3)
            {
                int index = 0;
                bitmap.Clear(FastColor.black);
                for (int iteration_x = 0; iteration_x < sysmap.Length; iteration_x++)
                {
                    for (int iteration_y = 0; iteration_y < sysmap[iteration_x].Length; iteration_y++)
                    {
                        uint value = Convert.ToUInt32(sysmap[iteration_x][iteration_y] * settings.Systems_per_ColorVal);
                        uint value_greyscale;
                        if (value > 360)
                        {
                            value_greyscale = 255;
                        }
                        else if(value > 255)
                        {
                            value_greyscale = 225;
                            
                        }
                        else
                        {
                            value_greyscale = value;
                        }
                        
                        var color = new Hsv((value + 180)/2, 1, value_greyscale).ToRgb();

                        bitmap.SetPixel(iteration_x + 1, iteration_y + 1, new FastColor((byte)color.R, (byte)color.G, (byte)color.B));
                    }
                }
                index++;
                if (index % 10045 == 0)
                {
                    Dispatcher.BeginInvoke((Action)(() => { Write($"{index} pixels set."); }));
                }

            }


            #endregion
            #region using Red
            else if (settings.Rendering == 4)
            {
               
                    int index = 0;
                    bitmap.Clear(FastColor.black);
                    for (int iteration_x = 0; iteration_x < sysmap.Length; iteration_x++)
                    {
                        for (int iteration_y = 0; iteration_y < sysmap[iteration_x].Length; iteration_y++)
                        {
                            uint value = Convert.ToUInt32(sysmap[iteration_x][iteration_y] * settings.Systems_per_ColorVal);
                            byte greyscale;
                            if (value > 255)
                            {
                                greyscale = 255;
                            }
                            else
                            {
                                greyscale = (byte)value;
                            }
                            bitmap.SetPixel(iteration_x + 1, iteration_y + 1, new FastColor(greyscale, 0, 0));
                        }
                    }
                    index++;
                    if (index % 10045 == 0)
                    {
                        Dispatcher.BeginInvoke((Action)(() => { Write($"{index} pixels set."); }));
                    }

                
            }
            #endregion
            #region using Green
            else if (settings.Rendering == 5)
            {

                int index = 0;
                bitmap.Clear(FastColor.black);
                for (int iteration_x = 0; iteration_x < sysmap.Length; iteration_x++)
                {
                    for (int iteration_y = 0; iteration_y < sysmap[iteration_x].Length; iteration_y++)
                    {
                        uint value = Convert.ToUInt32(sysmap[iteration_x][iteration_y] * settings.Systems_per_ColorVal);
                        byte greyscale;
                        if (value > 255)
                        {
                            greyscale = 255;
                        }
                        else
                        {
                            greyscale = (byte)value;
                        }
                        bitmap.SetPixel(iteration_x + 1, iteration_y + 1, new FastColor(0, greyscale, 0));
                    }
                }
                index++;
                if (index % 10045 == 0)
                {
                    Dispatcher.BeginInvoke((Action)(() => { Write($"{index} pixels set."); }));
                }


            }
            #endregion
            #region using Blue
            else if (settings.Rendering == 6)
            {

                int index = 0;
                bitmap.Clear(FastColor.black);
                for (int iteration_x = 0; iteration_x < sysmap.Length; iteration_x++)
                {
                    for (int iteration_y = 0; iteration_y < sysmap[iteration_x].Length; iteration_y++)
                    {
                        uint value = Convert.ToUInt32(sysmap[iteration_x][iteration_y] * settings.Systems_per_ColorVal);
                        byte greyscale;
                        if (value > 255)
                        {
                            greyscale = 255;
                        }
                        else
                        {
                            greyscale = (byte)value;
                        }
                        bitmap.SetPixel(iteration_x + 1, iteration_y + 1, new FastColor(0, 0, greyscale));
                    }
                }
                index++;
                if (index % 10045 == 0)
                {
                    Dispatcher.BeginInvoke((Action)(() => { Write($"{index} pixels set."); }));
                }


            }
            #endregion

            //Additional Manipulation
            if (settings.Scaling || settings.Radial5kly || settings.Radial10kly || settings.Axial5kly || settings.Axial10kly || settings.ColorTable || settings.EDSMIcon)
            {

                // add a scale
                int thousand_ly_in_px = 1000 / settings.Ly_to_px;
                Bitmap _bitmap = bitmap;
                using (var graphics = Graphics.FromImage(_bitmap))
                {
                    if (settings.Scaling)
                    {
                        int img_y = (int)settings.Img_Yres;
                        Rectangle[] filledrectangle = 
                        {   new Rectangle(50,                       img_y-200,thousand_ly_in_px,100),
                            new Rectangle(50+2*thousand_ly_in_px,   img_y-200,thousand_ly_in_px,100),
                            new Rectangle(50+4*thousand_ly_in_px,   img_y-200,thousand_ly_in_px,100),
                            new Rectangle(50+6*thousand_ly_in_px,   img_y-200,thousand_ly_in_px,100),
                            new Rectangle(50+8*thousand_ly_in_px,   img_y-200,thousand_ly_in_px,100),

                            new Rectangle(50,                       img_y-120,thousand_ly_in_px*10,20)
                        };
                        Rectangle[] hollowrectangle =
                        {
                            new Rectangle(50+1*thousand_ly_in_px,   img_y-200,thousand_ly_in_px,100),
                            new Rectangle(50+3*thousand_ly_in_px,   img_y-200,thousand_ly_in_px,100),
                            new Rectangle(50+5*thousand_ly_in_px,   img_y-200,thousand_ly_in_px,100),
                            new Rectangle(50+7*thousand_ly_in_px,   img_y-200,thousand_ly_in_px,100),
                            new Rectangle(50+9*thousand_ly_in_px,   img_y-200,thousand_ly_in_px,100),
                        };
                       
                        graphics.FillRectangles(new SolidBrush(Color.White), filledrectangle);
                        graphics.DrawRectangles(new Pen(Color.White), hollowrectangle);
                        


                        graphics.DrawString("10.000ly", new Font("Arial", 100), new SolidBrush(Color.White), new PointF(50, settings.Img_Yres - 350));
                    }
                    if (settings.ColorTable)
                    {
                        PointF offset = new PointF(50, settings.Img_Yres - 500);
                        int maxvalue;
                        switch (settings.Rendering)
                        {
                            case 0:
                            case 1:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                                maxvalue = settings.Systems_per_ColorVal * 255; // max value is a byte (rgb)
                                break;
                            default:
                                maxvalue = settings.Systems_per_ColorVal * 360; // max value is 360° (hsv)
                                break;

                            
                        }
                        if(settings.Rendering == 0 || settings.Rendering == 1)
                        {
                            LinearGradientBrush linearGradient = new LinearGradientBrush() // WIP
                        }


                    }
                }
                bitmap = _bitmap;
                
            }

            string file = settings.Path_output + "/" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".png";
            Dispatcher.BeginInvoke((Action)(() => { Write("Saving Image..."); }));
                bitmap.Save(file);
            bitmap = null;
            
            sysmap = null;
            Dispatcher.BeginInvoke((Action)(() => {
                Write($"Image was sucessfully generated.");
                loadingicon.Visibility = Visibility.Hidden;
            }));
            System.Diagnostics.Process.Start(file); // Display image with default Image Displaying Software


        }
    }
    
}
