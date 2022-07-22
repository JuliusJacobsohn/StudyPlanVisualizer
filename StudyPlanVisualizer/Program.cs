using StudyPlanVisualizer.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;

const int SEMESTER_WIDTH = 300;
const int MODULE_HEIGHT = 50;

var data = File.ReadAllLines("data.csv");
List<Semester> semesters = ReadInput(data);

int targetWidth = semesters.Count * SEMESTER_WIDTH;
int targetHeight = semesters.Max(m => m.Modules.Count) * MODULE_HEIGHT;

using (Bitmap newImage = new(targetWidth, targetHeight))
{
    // Crop and resize the image.
    using (Graphics graphics = Graphics.FromImage(newImage))
    {
        graphics.FillRectangle(Brushes.White, 0, 0, targetWidth, targetHeight);
        int index = 0;
        foreach (var semester in semesters)
        {
            DrawSemester(semester, index, graphics);
            index++;
        }
    }
    newImage.Save("data.jpg", ImageFormat.Jpeg);
}

void DrawSemester(Semester semester, int index, Graphics graphics)
{
    Rectangle semesterFrame = new Rectangle(index*SEMESTER_WIDTH, 0, SEMESTER_WIDTH, targetWidth);
    graphics.DrawRectangle(new Pen(Color.Red), semesterFrame);
}

static List<Semester> ReadInput(string[] data)
{
    List<Semester> semesters = new List<Semester>();
    foreach (var line in data)
    {
        var parts = line.Split(";");
        var existingSemester = semesters.FirstOrDefault(m => m.Name == parts[0]);
        if (existingSemester == null)
        {
            existingSemester = new Semester { Name = parts[0], Modules = new List<Module>() };
            semesters.Add(existingSemester);
        }

        Module newModule = new Module
        {
            Name = parts[1],
            HexColor = parts[2],
            Grade = double.Parse(parts[3], CultureInfo.InvariantCulture),
        };
        existingSemester.Modules.Add(newModule);
    }

    return semesters;
}