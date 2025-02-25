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
        // Create and save the custom icons to temporary files
        public static Icon CreateLightModeIcon()
        {
            // Create a bitmap for the light mode icon
            using (Bitmap bmp = new Bitmap(32, 32))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    // Fill background
                    g.Clear(Color.White);

                    // Draw sun shape
                    g.FillEllipse(Brushes.Gold, 6, 6, 20, 20);

                    // Draw sun rays
                    Pen rayPen = new Pen(Color.Gold, 2);
                    // Top ray
                    g.DrawLine(rayPen, 16, 0, 16, 4);
                    // Right ray
                    g.DrawLine(rayPen, 28, 16, 32, 16);
                    // Bottom ray
                    g.DrawLine(rayPen, 16, 28, 16, 32);
                    // Left ray
                    g.DrawLine(rayPen, 0, 16, 4, 16);
                    // Diagonal rays
                    g.DrawLine(rayPen, 5, 5, 8, 8);
                    g.DrawLine(rayPen, 27, 5, 24, 8);
                    g.DrawLine(rayPen, 5, 27, 8, 24);
                    g.DrawLine(rayPen, 27, 27, 24, 24);
                }

                // Convert bitmap to icon
                IntPtr hIcon = bmp.GetHicon();
                return Icon.FromHandle(hIcon);
            }
        }

        public static Icon CreateDarkModeIcon()
        {
            // Create a bitmap for the dark mode icon
            using (Bitmap bmp = new Bitmap(32, 32))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    // Fill background
                    g.Clear(Color.MidnightBlue);

                    // Draw moon shape
                    g.FillEllipse(Brushes.Silver, 8, 6, 20, 20);
                    g.FillEllipse(Brushes.MidnightBlue, 12, 4, 20, 20);

                    // Draw stars
                    g.FillEllipse(Brushes.White, 3, 3, 2, 2);
                    g.FillEllipse(Brushes.White, 26, 8, 2, 2);
                    g.FillEllipse(Brushes.White, 20, 2, 1, 1);
                    g.FillEllipse(Brushes.White, 5, 20, 1, 1);
                    g.FillEllipse(Brushes.White, 28, 24, 2, 2);
                }

                // Convert bitmap to icon
                IntPtr hIcon = bmp.GetHicon();
                return Icon.FromHandle(hIcon);
            }
        }

        // Alternative: Create a toggle switch icon
        public static Icon CreateToggleIcon(bool isLightMode)
        {
            // Create a bitmap for the toggle icon
            using (Bitmap bmp = new Bitmap(32, 32))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    // Fill background with transparency
                    g.Clear(Color.Transparent);

                    // Draw toggle background
                    using (SolidBrush backgroundBrush = new SolidBrush(isLightMode ? Color.LightBlue : Color.DarkBlue))
                    {
                        g.FillRoundedRectangle(backgroundBrush, 4, 12, 24, 10, 5);
                    }

                    // Draw toggle switch
                    int togglePosition = isLightMode ? 18 : 6;
                    using (SolidBrush toggleBrush = new SolidBrush(isLightMode ? Color.White : Color.SlateGray))
                    {
                        g.FillEllipse(toggleBrush, togglePosition, 9, 16, 16);
                    }

                    // Add sun/moon symbol on the toggle
                    if (isLightMode)
                    {
                        // Sun symbol
                        g.DrawEllipse(Pens.Orange, togglePosition + 5, 14, 6, 6);
                        g.DrawLine(Pens.Orange, togglePosition + 8, 11, togglePosition + 8, 13);
                        g.DrawLine(Pens.Orange, togglePosition + 8, 21, togglePosition + 8, 23);
                        g.DrawLine(Pens.Orange, togglePosition + 3, 17, togglePosition + 5, 17);
                        g.DrawLine(Pens.Orange, togglePosition + 11, 17, togglePosition + 13, 17);
                    }
                    else
                    {
                        // Moon symbol
                        g.DrawArc(Pens.Yellow, togglePosition + 4, 12, 8, 10, 30, 180);
                    }
                }

                // Convert bitmap to icon
                IntPtr hIcon = bmp.GetHicon();
                return Icon.FromHandle(hIcon);
            }
        }
    }

    // Extension method to draw rounded rectangles
    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, float x, float y, float width, float height, float radius)
        {
            RectangleF rectangle = new RectangleF(x, y, width, height);
            GraphicsPath path = GetRoundedRect(rectangle, radius);
            graphics.FillPath(brush, path);
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

            // If the corner radius is greater than or equal to half the width, or height, then return a capsule instead
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