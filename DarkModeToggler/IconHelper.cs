using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace DarkModeToggler
{
    public static class IconHelper
    {
        private static Icon lightModeIcon;
        private static Icon darkModeIcon;

        public static void SetTaskbarIcons(bool isLightMode)
        {
            Icon icon = isLightMode ? GetLightModeIcon() : GetDarkModeIcon();
            SetApplicationIcon(icon);
        }

        private static Icon GetLightModeIcon()
        {
            if (lightModeIcon == null)
            {
                lightModeIcon = CreateLightModeIcon();
            }
            return lightModeIcon;
        }

        private static Icon GetDarkModeIcon()
        {
            if (darkModeIcon == null)
            {
                darkModeIcon = CreateDarkModeIcon();
            }
            return darkModeIcon;
        }

        private static void SetApplicationIcon(Icon icon)
        {
            // Set the application icon, which should reflect on all taskbars
            foreach (Form form in Application.OpenForms)
            {
                form.Icon = icon;
            }
        }

        public static Icon CreateLightModeIcon()
        {
            using (Bitmap bmp = new Bitmap(32, 32))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    DrawSun(g);
                }
                return ConvertBitmapToIcon(bmp);
            }
        }

        public static Icon CreateDarkModeIcon()
        {
            using (Bitmap bmp = new Bitmap(32, 32))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.MidnightBlue);
                    DrawMoon(g);
                }
                return ConvertBitmapToIcon(bmp);
            }
        }

        public static Icon CreateToggleIcon(bool isLightMode)
        {
            using (Bitmap bmp = new Bitmap(32, 32))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Transparent);
                    DrawToggle(g, isLightMode);
                }
                return ConvertBitmapToIcon(bmp);
            }
        }

        private static void DrawSun(Graphics g)
        {
            using (Brush sunBrush = Brushes.Gold)
            using (Pen rayPen = new Pen(Color.Gold, 2))
            {
                g.FillEllipse(sunBrush, 6, 6, 20, 20);
                g.DrawLine(rayPen, 16, 0, 16, 4);
                g.DrawLine(rayPen, 28, 16, 32, 16);
                g.DrawLine(rayPen, 16, 28, 16, 32);
                g.DrawLine(rayPen, 0, 16, 4, 16);
                g.DrawLine(rayPen, 5, 5, 8, 8);
                g.DrawLine(rayPen, 27, 5, 24, 8);
                g.DrawLine(rayPen, 5, 27, 8, 24);
                g.DrawLine(rayPen, 27, 27, 24, 24);
            }
        }

        private static void DrawMoon(Graphics g)
        {
            using (Brush moonBrush = Brushes.Silver)
            using (Brush backgroundBrush = Brushes.MidnightBlue)
            using (Brush starBrush = Brushes.White)
            {
                g.FillEllipse(moonBrush, 8, 6, 20, 20);
                g.FillEllipse(backgroundBrush, 12, 4, 20, 20);
                g.FillEllipse(starBrush, 3, 3, 2, 2);
                g.FillEllipse(starBrush, 26, 8, 2, 2);
                g.FillEllipse(starBrush, 20, 2, 1, 1);
                g.FillEllipse(starBrush, 5, 20, 1, 1);
                g.FillEllipse(starBrush, 28, 24, 2, 2);
            }
        }

        private static void DrawToggle(Graphics g, bool isLightMode)
        {
            using (SolidBrush backgroundBrush = new SolidBrush(isLightMode ? Color.LightBlue : Color.DarkBlue))
            using (SolidBrush toggleBrush = new SolidBrush(isLightMode ? Color.White : Color.SlateGray))
            {
                g.FillRoundedRectangle(backgroundBrush, 4, 12, 24, 10, 5);
                int togglePosition = isLightMode ? 18 : 6;
                g.FillEllipse(toggleBrush, togglePosition, 9, 16, 16);

                if (isLightMode)
                {
                    g.DrawEllipse(Pens.Orange, togglePosition + 5, 14, 6, 6);
                    g.DrawLine(Pens.Orange, togglePosition + 8, 11, togglePosition + 8, 13);
                    g.DrawLine(Pens.Orange, togglePosition + 8, 21, togglePosition + 8, 23);
                    g.DrawLine(Pens.Orange, togglePosition + 3, 17, togglePosition + 5, 17);
                    g.DrawLine(Pens.Orange, togglePosition + 11, 17, togglePosition + 13, 17);
                }
                else
                {
                    g.DrawArc(Pens.Yellow, togglePosition + 4, 12, 8, 10, 30, 180);
                }
            }
        }

        private static Icon ConvertBitmapToIcon(Bitmap bmp)
        {
            IntPtr hIcon = bmp.GetHicon();
            return Icon.FromHandle(hIcon);
        }
    }

    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, float x, float y, float width, float height, float radius)
        {
            RectangleF rectangle = new RectangleF(x, y, width, height);
            using (GraphicsPath path = GetRoundedRect(rectangle, radius))
            {
                graphics.FillPath(brush, path);
            }
        }

        private static GraphicsPath GetRoundedRect(RectangleF baseRect, float radius)
        {
            if (radius <= 0.0F)
            {
                GraphicsPath mPath = new GraphicsPath();
                mPath.AddRectangle(baseRect);
                mPath.CloseFigure();
                return mPath;
            }

            if (radius >= (Math.Min(baseRect.Width, baseRect.Height)) / 2.0f)
                radius = (Math.Min(baseRect.Width, baseRect.Height)) / 2.0f;

            GraphicsPath path = new GraphicsPath();
            path.AddArc(baseRect.X, baseRect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(baseRect.Right - radius * 2, baseRect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(baseRect.Right - radius * 2, baseRect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(baseRect.X, baseRect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
