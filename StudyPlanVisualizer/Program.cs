using System.Drawing;
using System.Drawing.Imaging;

const int SEMESTER_WIDTH = 300;

using (Bitmap newImage = new(300, 300))
{

    // Crop and resize the image.
    Rectangle destination = new Rectangle(0, 0, 200, 120);
    using (Graphics graphic = Graphics.FromImage(newImage))
    {
        graphic.DrawRectangle(new Pen(Color.Red), destination);
    }
    newImage.Save("test.jpg", ImageFormat.Jpeg);
}