using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlanVisualizer.Models
{
    public class Semester
    {
        public string Name { get; set; }
        public List<Module> Modules { get; set; }
    }
}
