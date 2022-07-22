using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlanVisualizer.Models
{
    public class Module
    {
        public string Name { get; set; } = "Unknown";
        public double Credits { get; set; }
        public double? Grade { get; set; }
        public string? HexColor { get; set; }
        public Color Color
        {
            get
            {
                if (string.IsNullOrEmpty(HexColor))
                {
                    return Color.White;
                }
                var conv = new ColorConverter();
                var color = ((Color?)conv.ConvertFromString(HexColor)) ?? Color.White;
                return color;
            }
        }
    }
}
