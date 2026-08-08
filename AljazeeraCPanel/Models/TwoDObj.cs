using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AljazeeraCPanel.Models
{
    public class TwoDObj
    {
        public TwoDObj()
        {
            // TODO: Complete member initialization
        }

        public TwoDObj(string p1, string p2)
        {
            // TODO: Complete member initialization
            this.AxisX = p1;
            this.AxisY = p2;
        }
        public string AxisX { get; set; }
        public string AxisY { get; set; }
    }
}