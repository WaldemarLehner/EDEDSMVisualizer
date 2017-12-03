using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FastBitmapLib;


namespace EDEDSMVisualizer
{
    class Program
    {

// ##### SETTINGS ######
const String pathtojson = @"E:\2017-Q4\Visual Studio\EDEDSMVisualizer\systemsWithCoordinates.json"; // path to the json containing coordinates
const String output = @"E:\2017-Q4\Visual Studio\EDEDSMVisualizer\output.jpeg";
static Value2d sol_offset = new Value2d(4000,3000); // where is Sol Located on the Image
static Value2d img_res = new Value2d(10000,10000); //Image Resolution  1px = 10x10lyr = 100lyr²

// ##### SETTINGS ######
static FastBitmap bitmap;
static long counter = 0;
        
        static void Main(string[] args)
        {
            Write("Press any key to start");
            Pause();
            Clear();
            GenerateBitMap();
            ParseJSON();
            bitmap.Save(output);
            Clear();
            Write("Image complete");
            Pause();

        }

        static void Write(String s) { Console.WriteLine(s); } 
        static void Pause() { Console.ReadKey();}
        static void Clear() { Console.Clear();  }
        static void GenerateBitMap()
        {
            Write("Generating Bitmap");
            bitmap = new FastBitmap(8000,8000);
        }
        static void ParseJSON()
        {
            try
            {
                String line;
                using (StreamReader reader = new StreamReader(pathtojson))
                {
                    while((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("{\"x\":")) //"x":
                        {
                            line = line.Substring(line.LastIndexOf("{\"x\":"));
                            if (line.Contains(",\"id\""))
                            {
                                line = line.Substring(0, line.LastIndexOf(",\"id\""));
                                line = line.Replace(",", ".");
                                
                            }
                            List<long> xyz = new List<long>();
                            MatchCollection collection = Regex.Matches(line, @"-?(?:\d*\.+)?\d+");
                            foreach(Match m in collection)
                            {
                              int d = (int) Double.Parse(m.Value.Replace(".",","));
                                xyz.Add(d/10);

                            }
                            AddToBitmap(xyz.ElementAt(0), xyz.ElementAt(2));
                            counter++;
                           
                        }
                    }



                }

                
            }
            catch(Exception e)
            {
                Write("Cant open file\n\n\n" + e);
                Pause();
            }
        }
        static public void AddToBitmap(long x_,long y_)
        {
            int bitmap_x = (int)(x_ + sol_offset._x);
            int bitmap_y = (int)(y_ + sol_offset._y);


            int systems_amount = bitmap.GetPixel(bitmap_x, bitmap_y).R;
          if(systems_amount < 253)
            {
                systems_amount = systems_amount + 3;
                FastColor c = new FastColor();
                c.R = (byte)systems_amount;
                c.G = (byte)systems_amount;
                c.B = (byte)systems_amount;
                Write("["+counter+"]\n"+bitmap_x + " " + bitmap_y + "@ " + c.R);
                bitmap.SetPixel(bitmap_x, bitmap_y, c);
            }
          else if(systems_amount > 252 && systems_amount < 255)
            {
                systems_amount = 255;
                FastColor c = new FastColor();
                c.R = (byte)systems_amount;
                c.G = (byte)systems_amount;
                c.B = (byte)systems_amount;
                Write("[" + counter + "]\n" + bitmap_x + " " + bitmap_y + "@ " + c.R);
                bitmap.SetPixel(bitmap_x, bitmap_y, c);
            }

        }
    }
}
