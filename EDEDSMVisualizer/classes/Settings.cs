using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDEDSMVisualizer.classes
{
    public class Settings
    {
        //Store settings in this class

        //In and Output Path
        public string Path_json { get; set; }
        public string Path_output { get; set; }

        //Image Dimensions
        public uint Img_Xres { get; set; }
        public uint Img_Yres {get;set; }
        public int Img_X_offset { get; set; }
        public int Img_Y_offset { get; set; }

        public int ColorValPerSystem { get; set; } // How many systems are represented by 1 color val
        public int Ly_to_px { get; set; } // How many Ly are in 1 px

        //Gridlines
        public Boolean Axial5kly { get; set; }
        public Boolean Axial10kly { get; set; }

        public Boolean Radial5kly { get; set; }
        public Boolean Radial10kly { get; set; }

        //Guiding
        public Boolean EDSMIcon { get; set; }
        public Boolean ColorTable { get; set; }
        public Boolean Scaling { get; set; }

        //Rendering Type
        public Byte Rendering { get; set; }
        /*
         * 1:Greyscale
         * 2:Hue
         * 3:Hue+Value
         * 4:r
         * 5:g
         * 6:b
         * 7:a
         */ 






    }
}
