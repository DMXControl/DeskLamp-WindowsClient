using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace DeskLamp_WinClient
{
    public static class ImageTools
    {
        private static readonly float[][] GREY_SCALE_MATRIX = new float[][]{  
                new float[] {.3f, .3f, .3f, 0, 0},  
                new float[] {.59f, .59f, .59f, 0, 0},  
                new float[] {.11f, .11f, .11f, 0, 0},  
                new float[] {0, 0, 0, 1f, 0},  
                new float[] {0, 0, 0, 0, 1f}  
        };

        public static Bitmap ConvertToGreyScale(Bitmap original)
        {
            return ApplyMatrix(original, GREY_SCALE_MATRIX);
        }

        public static Bitmap ApplyMatrix(Bitmap original, float[][] matrix)
        {
            if(original == null)
                throw new ArgumentNullException("original");
            if(matrix == null)
                throw new ArgumentNullException("matrix");

            Bitmap newBitmap = new Bitmap(original.Width, original.Height);
            using (Graphics newGraphics = Graphics.FromImage(newBitmap))
            {
                ColorMatrix newColorMatrix = new ColorMatrix(matrix);
                using (ImageAttributes attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(newColorMatrix);
                    newGraphics.DrawImage(original,
                            new System.Drawing.Rectangle(0, 0, original.Width, original.Height),
                            0, 0, original.Width, original.Height,
                            GraphicsUnit.Pixel, attributes);
                }
            }
            return newBitmap;
        }

        public static Icon ImageToIcon(this Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");
            
            Bitmap b = image is Bitmap ? (Bitmap)image : new Bitmap(image);
            return Icon.FromHandle(b.GetHicon());
        }

        public static Image IconToImage(this Icon icon)
        {
            if (icon == null)
                throw new ArgumentNullException("icon");

            //This works better then icon.ToBitmap()!
            return Image.FromHbitmap(icon.ToBitmap().GetHbitmap());
        }

        public static Image Resize(this Image input, Size newSize, InterpolationMode mode = InterpolationMode.HighQualityBicubic)
        {
            return Resize(input, newSize.Width, newSize.Height, mode);
        }

        public static Image Resize(this Image input, int newWidth, int newHeight, InterpolationMode mode = InterpolationMode.HighQualityBicubic)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.Width == newWidth && input.Height == newHeight)
                return input;

            //Create new Image with right Size
            Image newI = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(newI))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = mode;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(input, 0, 0, newWidth, newHeight);
            }
            return newI;
        }

        public static Bitmap DrawHSVCircle(Color backColor, int drawWidth = 512, int drawHeight = 512, int boarder = 0)
        {
            if (boarder < 0 || boarder >= drawHeight || boarder >= drawWidth)
                throw new ArgumentOutOfRangeException("boarder");
            if (drawHeight < 16)
                throw new ArgumentOutOfRangeException("drawHeight");
            if (drawWidth < 16)
                throw new ArgumentOutOfRangeException("drawWidth");


            Bitmap drawArea = new Bitmap(drawWidth, drawHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Point center = new Point(drawWidth / 2, drawHeight / 2);
            double radius = Math.Min((drawWidth - boarder) / 2, (drawHeight - boarder) / 2);
            for (int i = 0; i < drawWidth; i++)
            {
                for (int j = 0; j < drawHeight; j++)
                {
                    double angle;
                    double dist = Math.Sqrt(Math.Pow(i - center.X, 2) + Math.Pow(j - center.Y, 2));

                    if (i - center.X > 0)
                        angle = Math.Atan2(i - center.X, j - center.Y);
                    else
                        angle = Math.Atan2(-(i - center.X), -(j - center.Y)) + Math.PI;

                    if (dist <= radius)
                    {
                        Color c = ColorTools.FromHSV(angle / (2 * Math.PI), dist / radius, 1.0, _base: 1, returnNullOnError:false)
                            ?? backColor;
                        drawArea.SetPixel(i, j, c);
                    }
                    else
                        drawArea.SetPixel(i, j, backColor);
                }
            }
            return drawArea;
        }

        public static Bitmap DrawHSVRectangle(Color backColor, double hueMin = 0, double hueMax = 360, int drawWidth = 512, int drawHeight = 512, int boarder = 0)
        {
            if (boarder < 0 || boarder >= drawHeight || boarder >= drawWidth)
                throw new ArgumentOutOfRangeException("boarder");
            if (drawHeight < 16)
                throw new ArgumentOutOfRangeException("drawHeight");
            if (drawWidth < 16)
                throw new ArgumentOutOfRangeException("drawWidth");
            if (hueMin < 0 || hueMin >= hueMax)
                throw new ArgumentOutOfRangeException("hueMin");
            if (hueMax < 0 || hueMax > 360)
                throw new ArgumentOutOfRangeException("hueMax");

            Bitmap drawArea = new Bitmap(drawWidth, drawHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            double hDelta = (hueMin - hueMax)/(drawWidth - 2*boarder);
            for (int i = 0; i < drawWidth; i++)
            {
                Color c = backColor;
                if (i >= boarder || i < drawWidth - boarder)
                {
                    c = ColorTools.FromHSV((i - boarder)*hDelta + hueMin, 1.0, 1.0, _base:360, returnNullOnError:false)
                        ?? backColor;
                }

                for (int j = 0; j < drawHeight; j++)
                {
                    if (i < boarder || i >= drawWidth - boarder
                      || j < boarder || j >= drawHeight - boarder)
                    {
                        drawArea.SetPixel(i, j, backColor);
                    }
                    else
                    {
                        drawArea.SetPixel(i, j, c);
                    }
                }
            }
            return drawArea;
        }
    }
}
