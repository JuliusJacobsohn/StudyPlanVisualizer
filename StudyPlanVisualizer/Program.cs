using StudyPlanVisualizer.Models;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;

const int SEMESTER_WIDTH = 500;
const int MODULE_HEIGHT = 100;
const int MODULE_FRAME_SIZE = 4;

// Read in data from csv file and parse into class structure
var data = File.ReadAllLines("data.csv").Skip(1).ToArray();
List<Semester> semesters = ReadInput(data);

// Determine final output size
int targetWidth = semesters.Count * SEMESTER_WIDTH;
int targetHeight = (1+semesters.Max(m => m.Modules.Count)) * MODULE_HEIGHT + MODULE_HEIGHT;

using (Bitmap newImage = new(targetWidth, targetHeight))
{
    using (Graphics graphics = Graphics.FromImage(newImage))
    {
        // Make frame white
        graphics.FillRectangle(Brushes.White, 0, 0, targetWidth, targetHeight);

        // Go over all semesters to draw their frames and titles
        int index = 0;
        foreach (var semester in semesters)
        {
            DrawSemester(semester, index * SEMESTER_WIDTH, 0, graphics);
            index++;
        }

        // Go over all semesters again and draw their modules
        // This is needed because otherwise, 2-semester modules would be overlapped by the next semester frame
        index = 0;
        foreach(var semester in semesters)
        {
            // Determine if the last semester had a module that spans two semesters
            bool skipOneModule = false;
            if (index > 0)
            {
                skipOneModule = semesters.ElementAt(index - 1).Modules.Any(m => m.SpansTwoSemesters);
            }

            // If the last semester had a module that spans two semesters, skip the first line in the current semester
            int moduleIndex = skipOneModule ? 2 : 1;

            // Draw all modules for the current semester
            foreach (var module in semester.Modules)
            {
                DrawModule(module, index * SEMESTER_WIDTH, moduleIndex * MODULE_HEIGHT, graphics);
                moduleIndex++;
            }
            index++;
        }
    }

    // Save and preview image
    newImage.Save("data.png", ImageFormat.Png);
    var fileInfo = new FileInfo("data.png");
    Process.Start("explorer.exe", fileInfo.FullName);
}

// Draws the frame and title of a semester
void DrawSemester(Semester semester, int xOffset, int yOffset, Graphics graphics)
{
    Rectangle semesterFrame = new Rectangle(xOffset, yOffset, SEMESTER_WIDTH, targetWidth);
    graphics.FillRectangle(new SolidBrush(Color.Beige), semesterFrame);
    graphics.DrawString(semester.Name, new Font("Arial", 20, FontStyle.Bold), new SolidBrush(Color.Black), semesterFrame.X + 10, semesterFrame.Y + 10);
}

// Draws the frame and data of a module
void DrawModule(Module module, int xOffset, int yOffset, Graphics graphics)
{
    // Create the frame of the module
    int widthModifier = module.SpansTwoSemesters ? 2 : 1;
    Rectangle moduleFrame = new Rectangle(xOffset + MODULE_FRAME_SIZE, yOffset + MODULE_FRAME_SIZE, (widthModifier * SEMESTER_WIDTH) - MODULE_FRAME_SIZE * 2, MODULE_HEIGHT - MODULE_FRAME_SIZE * 2);
    graphics.FillRectangle(new SolidBrush(module.Color), moduleFrame);

    // Draw the module type
    if (!string.IsNullOrEmpty(module.TypeName))
    {
        graphics.DrawString(module.TypeName, new Font("Arial", 16, FontStyle.Underline), new SolidBrush(Color.Black), moduleFrame.X + 10, moduleFrame.Y + 10);
    }

    // Draw the module name
    graphics.DrawString(module.Name, new Font("Arial", 14, FontStyle.Bold), new SolidBrush(Color.Black), moduleFrame.X + 10, moduleFrame.Y + moduleFrame.Height - 40);

    // Draw the CP
    graphics.DrawString("CP: " + module.Credits.ToString("0.0", CultureInfo.InvariantCulture), new Font("Arial", 12), new SolidBrush(Color.Black), moduleFrame.X + moduleFrame.Width - 80, moduleFrame.Y + 10);

    if (module.IsGraded)
    {
        // Draw the grade
        if (module.Grade != null)
        {
            // Already received a grade
            graphics.DrawString("Grade: " + module.Grade.Value.ToString("0.0", CultureInfo.InvariantCulture), new Font("Arial", 12), new SolidBrush(Color.Black), moduleFrame.X + moduleFrame.Width - 80, moduleFrame.Y + moduleFrame.Height - 30);
        }
        else
        {
            // No grade yet
            graphics.DrawString("Grade: -", new Font("Arial", 12), new SolidBrush(Color.Black), moduleFrame.X + moduleFrame.Width - 80, moduleFrame.Y + moduleFrame.Height - 30);
        }
    }
    else
    {
        // Ungraded
        graphics.DrawString("Ungraded", new Font("Arial", 12), new SolidBrush(Color.Black), moduleFrame.X + moduleFrame.Width - 80, moduleFrame.Y + moduleFrame.Height - 30);
    }
}

// Reads in the data from the csv file and parses it into a list of Semester objects
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
            TypeName = parts[2],
            IsExtraModule = parts[3] != string.Empty ? bool.Parse(parts[3]) : false,
            SpansTwoSemesters = parts[4] != string.Empty ? bool.Parse(parts[4]) : false,
            IsGraded = parts[5] != string.Empty ? bool.Parse(parts[5]) : true,
            Grade = parts[6] != string.Empty ? double.Parse(parts[6], CultureInfo.InvariantCulture) : null,
            Credits = double.Parse(parts[7], CultureInfo.InvariantCulture),
            HexColor = parts[8],
        };
        existingSemester.Modules.Add(newModule);
    }

    return semesters;
}