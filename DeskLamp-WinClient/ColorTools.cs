using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Linq;
using System.Drawing.Drawing2D;

namespace DeskLamp_WinClient
{
    /// <summary>
    /// A collection of tools when working with Colors.
    /// </summary>
    public static class ColorTools
    {
        /// <summary>
        /// Compares two Colors if they are equal.
        /// In fact compares the A R G B Values.
        /// </summary>
        /// <param name="c1">Color 1</param>
        /// <param name="c2">Color 2</param>
        /// <returns>true when they are equal</returns>
        public static bool ColorsEqual(this Color c1, Color c2)
        {
            return c1.ToArgb() == c2.ToArgb();
        }

        public static Image Color2Image(Color c, Size z)
        {
            return Color2Image(c, z.Width, z.Height);
        }

        public static Image Color2Image(Color c, int width, int height)
        {
            Bitmap pict = new Bitmap(width, height);
            using(Graphics g = Graphics.FromImage(pict))
                g.Clear(c);
            return pict;
        }

        /// <summary>
        /// Checks whether value is between min an max. min and max are inclusive.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <param name="min">The inclusive min boarder</param>
        /// <param name="max">The inclusive max boarder</param>
        /// <returns>True when value between min and max</returns>
        public static bool Between(this double value, double min, double max)
        {
            return value >= min && value <= max;
        }

        public static double RuleOfThree(double input, double inMax, double inMin, double outMax, double outMin)
        {
            if (inMin == inMax)
            {
                //AL 2016-07-14: Who did this? That doesnt make any sence
                //if inMin == inMax, this is 0 - input + inMin
                return (inMax - inMin) - (input - inMin);
            }
            if (input <= inMin)
            {
                if (input < inMin)
                    throw new ArgumentOutOfRangeException("input", input, "Input must be >= inMin [" + inMin + "]");
                return outMin;
            }
            if (input >= inMax)
            {
                if (input > inMax)
                    throw new ArgumentOutOfRangeException("input", input, "Input must be <= inMax [" + inMax + "]");
                return outMax;
            }
            double percent = (input - inMin) / (inMax - inMin);
            double ret = ((outMax - outMin) * percent) + outMin;
            return ret;
        }

        public static Color? FromHSV(double h, double s, double v, int _base, bool returnNullOnError = true)
        {
            switch (_base)
            {
                case 1:
                    if (!h.Between(0, 1) || !s.Between(0, 1) || !v.Between(0, 1))
                    {
                        if (returnNullOnError) return null;
                        throw new ArgumentOutOfRangeException();
                    }
                    h = RuleOfThree(h, 1, 0, 360, 0);
                    break;
                case 255:
                    if (!h.Between(0, 255) || !s.Between(0, 255) || !v.Between(0, 255))
                    {
                        if (returnNullOnError) return null;
                        throw new ArgumentOutOfRangeException();
                    }
                    h = RuleOfThree(h, 255, 0, 360, 0);
                    s = RuleOfThree(s, 255, 0, 1, 0);
                    v = RuleOfThree(v, 255, 0, 1, 0);
                    break;
                case 360:
                    if (!h.Between(0, 360) || !s.Between(0, 1) || !v.Between(0, 1))
                    {
                        if (returnNullOnError) return null;
                        throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    if (returnNullOnError) return null;
                    throw new ArgumentException("base must be 1, 255 or 360");
            }

            return HSVConverter.HSVtoRGB(h, s, v).ToColor();
        }

        public static double TrimHue(double p)
        {
            if (p > 360)
                p %= 360;
            else if (p < 0)
                p = 360 + (p % 360);
            return p;
        }
    }

    public static class HSVConverter
    {
        public static HSV ColortoHSV(Color c)
        {
            return RGBtoHSV((double) c.R/255, (double) c.G/255, (double) c.B/255);
        }

        public static HSV RGBtoHSV(RGB rgb)
        {
            return RGBtoHSV(rgb.R, rgb.G, rgb.B);
        }

        public static HSV RGBtoHSV(double r, double g, double b)
        {
            HSV ret = new HSV();
            double min, max, delta;
            min = Math.Min(r, Math.Min(g, b));
            max = Math.Max(r, Math.Max(g, b));
            ret.V = max;				// v
            delta = max - min;
            if (max != 0)
                ret.S = delta / max;		// s
            else
            {
                // r = g = b = 0		// s = 0, h is undefined
                ret.S = 0;
                ret.H = -1;
                return ret;
            }
            if (delta != 0)
            {
                if (r == max)
                    ret.H = (g - b) / delta;		// between yellow & magenta
                else if (g == max)
                    ret.H = 2 + (b - r) / delta;	// between cyan & yellow
                else
                    ret.H = 4 + (r - g) / delta;	// between magenta & cyan
                ret.H *= 60;				// degrees
                while (ret.H < 0)
                    ret.H += 360;
            }
            else //r = g = b => grey, h is undefined
            { 
                ret.H = -1; 
            }
            return ret;
        }

        public static RGB HSVtoRGB(HSV hsv)
        {
            return HSVtoRGB(hsv.H, hsv.S, hsv.V);
        }

        public static RGB HSVtoRGB(double h, double s, double v)
        {
            RGB ret = new RGB();
            int i;
            double f, p, q, t;
            if (s == 0)
            {
                // achromatic (grey)
                ret.R = ret.G = ret.B = v;
                return ret;
            }
            h /= 60;			// sector 0 to 5
            i = (int)Math.Floor(h);
            f = h - i;			// factorial part of h
            p = v * (1 - s);
            q = v * (1 - s * f);
            t = v * (1 - s * (1 - f));
            switch (i%6) // %6 because when h is 360° then is i 6, we need 0° !
            {
                default:
                case 0:
                    ret.R = v;
                    ret.G = t;
                    ret.B = p;
                    break;
                case 1:
                    ret.R = q;
                    ret.G = v;
                    ret.B = p;
                    break;
                case 2:
                    ret.R = p;
                    ret.G = v;
                    ret.B = t;
                    break;
                case 3:
                    ret.R = p;
                    ret.G = q;
                    ret.B = v;
                    break;
                case 4:
                    ret.R = t;
                    ret.G = p;
                    ret.B = v;
                    break;
                case 5:
                    ret.R = v;
                    ret.G = p;
                    ret.B = q;
                    break;
            }
            return ret;
        }
    }

    public class RGB
    {
        public double R { get; set; }
        public double G { get; set; }
        public double B { get; set; }

        public Color ToColor()
        {
            int r = (int)Math.Round(R * 255);
            int g = (int)Math.Round(G * 255);
            int b = (int)Math.Round(B * 255);
            return Color.FromArgb(r, g, b);
        }
    }

    public class HSV
    {
        public double H { get; set; }
        public double S { get; set; }
        public double V { get; set; }
    }
}