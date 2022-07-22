using StudyPlanVisualizer.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;

const int SEMESTER_WIDTH = 300;
const int MODULE_HEIGHT = 50;

var data = File.ReadAllLines("data.csv").Skip(1);
List<Semester> semesters = ReadInput(data);

int targetWidth = semesters.Count * SEMESTER_WIDTH;
int targetHeight = semesters.Max(m => m.Modules.Count) * MODULE_HEIGHT+MODULE_HEIGHT;

using (Bitmap newImage = new(targetWidth, targetHeight))
{
    // Crop and resize the image.
    using (Graphics graphics = Graphics.FromImage(newImage))
    {
        graphics.FillRectangle(Brushes.White, 0, 0, targetWidth, targetHeight);
        int index = 0;
        foreach (var semester in semesters)
        {
            DrawSemester(semester, index*SEMESTER_WIDTH, 0, graphics);
            index++;
        }
    }
    newImage.Save("data.jpg", ImageFormat.Jpeg);
}

void DrawSemester(Semester semester, int xOffset, int yOffset, Graphics graphics)
{
    Rectangle semesterFrame = new Rectangle(xOffset, yOffset, SEMESTER_WIDTH, targetWidth);
    graphics.FillRectangle(new SolidBrush(Color.Beige), semesterFrame);
    graphics.DrawString(semester.Name, new Font("Arial", 16), new SolidBrush(Color.Black), semesterFrame.X, semesterFrame.Y);
    int moduleIndex = 1;
    foreach (var module in semester.Modules)
    {
        DrawModule(module, xOffset, moduleIndex*MODULE_HEIGHT, graphics);
        moduleIndex++;
    }
}

void DrawModule(Module module, int xOffset, int yOffset, Graphics graphics)
{
    Rectangle moduleFrame = new Rectangle(xOffset, yOffset, SEMESTER_WIDTH, MODULE_HEIGHT);
    graphics.FillRectangle(new SolidBrush(ColorFromHexString(module.HexColor ?? "#ffffff")), moduleFrame);
    graphics.DrawString(module.Name, new Font("Arial", 14), new SolidBrush(Color.Black), moduleFrame.X, moduleFrame.Y);
    if (module.Grade != null)
    {
        graphics.DrawString(module.Grade.Value.ToString(CultureInfo.InvariantCulture), new Font("Arial", 12), new SolidBrush(Color.Black), moduleFrame.X, moduleFrame.Y + moduleFrame.Height - 20);
    }
    else
    {
        graphics.DrawString("-", new Font("Arial", 12), new SolidBrush(Color.Black), moduleFrame.X, moduleFrame.Y + moduleFrame.Height - 20);
    }
}

static Color ColorFromHexString(string hex)
{
    var conv = new ColorConverter();
    var color = ((Color?)conv.ConvertFromString(hex)) ?? Color.White;
    return color;
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