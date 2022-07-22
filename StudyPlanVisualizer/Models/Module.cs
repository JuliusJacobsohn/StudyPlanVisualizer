using System;
using System.Collections.Generic;
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
    }
}
