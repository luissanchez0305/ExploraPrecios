using System;
using System.Collections;
using System.Data;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Explora_Precios.ApplicationServices
{

    /// <summary>
    /// Summary description for Captcha
    /// </summary>
    public class CaptchaImage
    {
        public CaptchaImage()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public string Text
        {
            get { return this.text; }
        }

        public Bitmap Image
        {
            get { return this.image; }
        }

        public int Width
        {
            get { return this.width; }
        }

        public int Height
        {
            get { return this.height; }
        }

        private string text;
        private int width;
        private int height;
        private string familyName;
        private Bitmap image;

        // // // // SETTINGS

        private readonly string[] fontList = new string[] { "Times New Roman", "Verdana", "Comic Sans MS", "Arial", "Courier New", "Georgia", "Impact", "Palatino Linotype", "Lucida Console", "Marlett" };
        private static int charLength = 5;

        // // // // END SETTINGS

        private Random random = new Random();


        public CaptchaImage(string s, int width, int height)
        {
            this.text = s;
            HttpContext.Current.Session["CaptchaCode"] = s;
            this.SetDimensions(width, height);
            this.SetFamilyName(fontList[random.Next(0, fontList.Length - 1)]);
            this.GenerateImage();
        }

        ~CaptchaImage()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.image.Dispose();
            }
        }

        private void SetDimensions(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width", width, "Argument out of range, must be greater than zero.");
            if (height <= 0)
                throw new ArgumentOutOfRangeException("height", height, "Argument out of range, must be greater than zero.");
            this.width = width;
            this.height = height;
        }

        private void SetFamilyName(string familyName)
        {
            try
            {
                Font font = new Font(this.familyName, 12F);
                this.familyName = familyName;
                font.Dispose();
            }
            catch
            {
                this.familyName = System.Drawing.FontFamily.GenericSerif.Name;
            }
        }
        private void GenerateImage()
        {
            Bitmap bitmap = new Bitmap(this.width, this.height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, this.width, this.height);

            Color backColor = Color.FromArgb(random.Next(128, 255), random.Next(128, 255), random.Next(128, 255));
            Color foreColor = ColorInvert(backColor);

            HatchBrush hatchBrush = new HatchBrush(RandomEnum<HatchStyle>(), backColor, Color.White);
            g.FillRectangle(hatchBrush, rect);

            SizeF size;
            float fontSize = rect.Height + 11;
            Font font;
            do
            {
                fontSize--;
                font = new Font(this.familyName, fontSize, FontStyle.Bold);
                size = g.MeasureString(this.text, font);
            } while (size.Width > rect.Width);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            GraphicsPath path = new GraphicsPath();
            path.AddString(this.text, font.FontFamily, (int)font.Style, font.Size, rect, format);
            float v = 4F;
            PointF[] points =
            {
                new PointF(this.random.Next(rect.Width)/v, this.random.Next(rect.Height)/v),
                new PointF(rect.Width - this.random.Next(rect.Width)/v, this.random.Next(rect.Height)/v),
                new PointF(this.random.Next(rect.Width)/v, rect.Height - this.random.Next(rect.Height)/v),
                new PointF(rect.Width - this.random.Next(rect.Width)/v, rect.Height - this.random.Next(rect.Height)/v)
            };
            Matrix matrix = new Matrix();
            matrix.Translate(0F, 0F);
            path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);

            hatchBrush = new HatchBrush(RandomEnum<HatchStyle>(), Color.DimGray, foreColor);
            g.FillPath(hatchBrush, path);

            int m = Math.Max(rect.Width, rect.Height);
            for (int i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
            {
                int x = this.random.Next(rect.Width);
                int y = this.random.Next(rect.Height);
                int w = this.random.Next(m / 50);
                int h = this.random.Next(m / 50);
                g.FillEllipse(hatchBrush, x, y, w, h);
            }

            font.Dispose();
            hatchBrush.Dispose();
            g.Dispose();
            this.image = bitmap;
        }

        public static string generateRandomCode()
        {
            ArrayList Kod = new ArrayList();
            Random rnd = new Random();

            for (char i = 'A'; i <= 'Z'; i++)
                Kod.Add(i);

            for (char i = 'a'; i <= 'z'; i++)
                Kod.Add(i);

            for (char i = '0'; i <= '9'; i++)
                Kod.Add(i);

            string val = "";

            for (int i = 0; i < charLength; i++)
                val += Kod[rnd.Next(Kod.Count)].ToString();

            return val;
        }

        private Random rand = new Random();
        private T RandomEnum<T>()
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            return values[rand.Next(0, values.Length)];
        }

        private Color ColorInvert(Color colorIn)
        {
            return Color.FromArgb(colorIn.A, Color.White.R - colorIn.R,
    Color.White.G - colorIn.G, Color.White.B - colorIn.B);
        }

    }
}