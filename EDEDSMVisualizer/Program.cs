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
        static String pathtojson = "";
        static String output = "";
        static readonly Value2d sol_offset = new Value2d(5000,2000); // where is Sol Located on the Image
        static readonly Value2d img_res = new Value2d(10000,10000); //Image Resolution  1px = 10x10lyr = 100lyr²

        // ##### SETTINGS ######
        static FastBitmap bitmap;
        static long counter = 0;
        
        static void Main(string[] args)
        {
#if DEBUG
            args = new[] { @"E:\2017-Q4\Visual Studio\EDSMVis\EDEDSMVisualizer\systemsWithCoordinates.json", @"C:\Users\Waldemar L\Documents\" };
#endif
            ImportArgs(args);
            ValidatePathToJson();
            ValidateOutput();
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
        static string Read() { return Console.ReadLine(); }
        static void Pause() { Console.ReadKey();}
        static void Clear() { Console.Clear();  }
        static void GenerateBitMap()
        {
            Write("Generating Bitmap");
            bitmap = new FastBitmap((int)img_res._x,(int)img_res._y); // Generate new bitmap with dimensions from the settings above
            
        }
        static void ParseJSON() //take the json and just extract the x and z coords and add them to the bitmap.
        {
            

                bool skiptoX = true, lastX = false;
                byte count = 0;
                long x = 0, y = 0;
                JsonTextReader streamReader = new JsonTextReader(new StreamReader(pathtojson));
                while (streamReader.Read())
                {
                    if(streamReader.Value != null)
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


                        else if(count > 0 && count < 4)
                        {
                            count++;
                        }

                        
                        else if (count==4)
                        {
                            y = Convert.ToInt64(streamReader.Value);
                            
                            skiptoX = true;
                            count = 0;
                            AddToBitmap(x, y);
                            counter++;
                            

                        }
                       

                    }
                }


                /*
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
                    }*/



               // }

                
            
            
        }
        static public void ImportArgs(string[] args)
        {
            if (args == null) { }//No arguments were passed
            else
            {
                if (args.Length == 2)
                {
                    pathtojson = args[0];
                    output = args[1];
                }
                else if (args.Length == 1)
                {
                    if (args[0].Contains(".json"))
                    {
                        pathtojson = args[0];
                    }
                    else
                    {
                        output = args[0];
                    }
                }
            }
        }
        static public void AddToBitmap(long _x,long _y)
        
        {
            
            int bitmap_x = (int)(_x/10 + sol_offset._x);
            int bitmap_y = (int)(img_res._y)-(int)(_y/10 + sol_offset._y);


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
                Write("[" + counter + "]\n");
                bitmap.SetPixel(bitmap_x, bitmap_y, c);
            }

        }
        static void ValidatePathToJson()
        {
            if (String.IsNullOrEmpty(pathtojson))
            {
                Write("Enter the path to the JSON (needs to have EDSM's systemsWithCoordinates.json format)");
                String _pathtojson = Read();
                if (File.Exists(_pathtojson) && _pathtojson.EndsWith(".json")){
                    pathtojson = _pathtojson;
                }
                else
                {
                    Clear();
                    Write("Error: File/Path does not exist or is not a .json");
                    ValidatePathToJson();
                }
            }
        }
        static void ValidateOutput()
        {
            if (String.IsNullOrEmpty(output)){
                Write("Enter the path to the output directory (\\path\\to\\file\\)");
                String _output = Read();
                Directory.CreateDirectory(_output);
                if (!_output.EndsWith(@"\")){
                    _output = _output + @"\";
                }
                output = _output + DateTime.Now.ToString("yyyy-MM-dd_hh-mm") + ".png";
            }
            else
            {
                Directory.CreateDirectory(output);
                if (!output.EndsWith(@"\"))
                {
                    output = output + @"\";
                }
                output = output + DateTime.Now.ToString("yyyy-MM-dd_HH-mm") + ".png";
            }
        }
    }
}
