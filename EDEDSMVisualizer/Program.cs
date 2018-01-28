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
        static long counter = 0; // Counts the number of systems that are being parsed.
        
        static void Main(string[] args)
        {
#if DEBUG   // Only in Debug mode - Application arguments are being overwritten, so one doesnt have to always input json and output directory.
            args = new[] { @"E:\2017-Q4\Visual Studio\EDSMVis\EDEDSMVisualizer\systemsWithCoordinates.json", @"C:\Users\Waldemar L\Documents\" };
#endif
            //Here's what the Application does from start to end.
            ImportArgs(args);                   //Set Application Arguments as pathtojson / output
            ValidatePathToJson();               //Check if JSON exists, if not, ask user to enter new path
            ValidateOutput();                   //Check if path exists, if not, create it. Also name the output file (the current date+time).png
            Write("Press any key to start");
            Pause();
            Clear();
            GenerateBitMap();
            ParseJSON();                        //Read to Datastream from the JSON and parse the X and Z values onto the bitmap as x and y
            bitmap.Save(output);                //Export the final bitmap as png.
            Clear();    
            Write("Image complete");            
            Pause();

        }

        static void Write(String s) { Console.WriteLine(s); }   //Shortcut Method to display text in the Console Window
        static string Read() { return Console.ReadLine(); }     //Shortcut Method to Read the string that is entered by the user
        static void Pause() { Console.ReadKey();}               //Shortcut Method to Stop execution and wait for a keypress
        static void Clear() { Console.Clear();  }               //Shortcut Method to Clear Console Content
        static void GenerateBitMap()
        {
            Write("Generating Bitmap");
            bitmap = new FastBitmap((int)img_res._x,(int)img_res._y); // Generate new bitmap with dimensions from the settings above
            
        }
        static void ParseJSON() //take the json and just extract the x and z coords and add them to the bitmap.
        {
            
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


                        else if(count > 0 && count < 4) // Count 3 Iterations (we starts with count = 1)
                        {
                            count++;
                        }

                        
                        else if (count==4) // After 3 Iterations, we read the value and pass it and x to AddToBitmap method while displaying the process on the Console.
                        {
                            y = Convert.ToInt64(streamReader.Value);
                            
                            skiptoX = true;
                            count = 0;
                            AddToBitmap(x, y);
                            counter++;
                            

                        }
                       

                    }
                }

            
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
            //The Origin gets an offset
            int bitmap_x = (int)(_x/10 + sol_offset._x);
            int bitmap_y = (int)(img_res._y)-(int)(_y/10 + sol_offset._y);

            //we get the color-value. (since it's greyscale R = G = B)
            int systems_amount = bitmap.GetPixel(bitmap_x, bitmap_y).R;
          if(systems_amount < 253) //We count up if we dont cause a byte overflow
            {
                systems_amount = systems_amount + 3;
                FastColor c = new FastColor();
                c.R = (byte)systems_amount;
                c.G = (byte)systems_amount;
                c.B = (byte)systems_amount;
                Write("["+counter+"]\n"+bitmap_x + " " + bitmap_y + "@ " + c.R);
                bitmap.SetPixel(bitmap_x, bitmap_y, c);
            }
          else if(systems_amount > 252 && systems_amount < 255) //if we would've caused a byte overflow we just set the value to byte.maximum (255)
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
