using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;



namespace EDEDSMVisualizer
{
    public class Value2d
    {
        public Int64 _x;
        public Int64 _y;
        public Value2d(Int64 x_l, Int64 y_l) { this._x = x_l; this._y = y_l; }
        public Value2d(Value2d_float value2D_Float)
        {
            _x = (int)Math.Round(value2D_Float._x);
            _y = (int)Math.Round(value2D_Float._y);
        }
    }
    public class Value2d_float
    {
        [JsonProperty("x")]
        public float _x;
        [JsonProperty("y")]
        public float _y;
        //[JsonProperty("z")]
       // public float _z;
        public Value2d_float(float x_l, float y_l)
        {
            _x = x_l;
            _y = y_l;
          
        }
    }
}
