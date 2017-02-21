using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace gifresizer.utility
{
    public class GifResize
    {
        public static void Crop(string gifFilePath, Rectangle rect, string outFilePath)
        {
            if (!File.Exists(gifFilePath))
            {
                throw new IOException(string.Format("{0}", gifFilePath));
            }
            using (Bitmap ora_Img = new Bitmap(gifFilePath))
            {
                if (ora_Img.RawFormat.Guid != ImageFormat.Gif.Guid)
                {
                    throw new IOException(string.Format("{0}", gifFilePath));
                }
            }
            GifImage gifImage = GifDecoder.Decode(gifFilePath);
            ThinkDisposalMethod(gifImage);
            int index = 0;
            foreach (GifFrame f in gifImage.Frames)
            {
                f.Image = f.Image.Clone(rect, f.Image.PixelFormat);
                f.ImageDescriptor.Width = (short)rect.Width;
                f.ImageDescriptor.Height = (short)rect.Height;
                if (index++ == 0)
                {
                    gifImage.LogicalScreenDescriptor.Width = (short)rect.Width;
                    gifImage.LogicalScreenDescriptor.Height = (short)rect.Height;
                }
            }
            GifEncoder.Encode(gifImage, outFilePath);
        }
        public static void GetThumbnail(string gifFilePath, double rate, string outputPath)
        {
            if (!File.Exists(gifFilePath))
            {
                throw new IOException(string.Format("{0}", gifFilePath));
            }
            using (Bitmap ora_Img = new Bitmap(gifFilePath))
            {
                if (ora_Img.RawFormat.Guid != ImageFormat.Gif.Guid)
                {
                    throw new IOException(string.Format("{0}", gifFilePath));
                }
            }
            GifImage gifImage = GifDecoder.Decode(gifFilePath);
            if (rate != 1.0)
            {
                gifImage.LogicalScreenDescriptor.Width = (short)(gifImage.LogicalScreenDescriptor.Width * rate);
                gifImage.LogicalScreenDescriptor.Height = (short)(gifImage.LogicalScreenDescriptor.Height * rate);
                int index = 0;
                foreach (GifFrame f in gifImage.Frames)
                {
                    f.ImageDescriptor.XOffSet = (short)(f.ImageDescriptor.XOffSet * rate);
                    f.ImageDescriptor.YOffSet = (short)(f.ImageDescriptor.YOffSet * rate);
                    f.ImageDescriptor.Width = (short)(f.ImageDescriptor.Width * rate);
                    f.ImageDescriptor.Height = (short)(f.ImageDescriptor.Height * rate);
                    if (f.ImageDescriptor.Width == 0)
                    {
                        f.ImageDescriptor.Width = 1;
                    }
                    if (f.ImageDescriptor.Height == 0)
                    {
                        f.ImageDescriptor.Height = 1;
                    }
                    Bitmap bmp = new Bitmap(f.ImageDescriptor.Width, f.ImageDescriptor.Height);
                    Graphics g = Graphics.FromImage(bmp);
                    g.DrawImage(f.Image, new Rectangle(0, 0, f.ImageDescriptor.Width, f.ImageDescriptor.Height));
                    g.Dispose();
                    Quantizer(bmp, f.Palette);
                    f.Image.Dispose();
                    f.Image = bmp;
                    index++;
                }
                GifEncoder.Encode(gifImage, outputPath);
            }
        }
        public static void Resize(string gifFilePath, int maxWidth, int maxHeight, string outputPath)
        {
            var rotateImage = Image.FromFile(gifFilePath);
            Bitmap image = new Bitmap(rotateImage);

            // Get the image's original width and height
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // To preserve the aspect ratio
            float ratioX = (float)maxWidth / (float)originalWidth;
            float ratioY = (float)maxHeight / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);
            rotateImage.Dispose();
            image.Dispose();

            GetThumbnail(gifFilePath, ratio, outputPath);
        }
        static void ThinkDisposalMethod(GifImage gifImage)
        {
            int lastDisposal = 0;
            Bitmap lastImage = null;
            int index = 0;
            short width = gifImage.Width;
            short height = gifImage.Height;
            foreach (GifFrame f in gifImage.Frames)
            {
                int xOffSet = f.ImageDescriptor.XOffSet;
                int yOffSet = f.ImageDescriptor.YOffSet;
                int iw = f.ImageDescriptor.Width;
                int ih = f.ImageDescriptor.Height;
                if ((f.Image.Width != width || f.Image.Height != height))
                {
                    f.ImageDescriptor.XOffSet = 0;
                    f.ImageDescriptor.YOffSet = 0;
                    f.ImageDescriptor.Width = (short)width;
                    f.ImageDescriptor.Height = (short)height;
                }
                int transIndex = -1;
                if (f.GraphicExtension.TransparencyFlag)
                {
                    transIndex = f.GraphicExtension.TranIndex;
                }
                if (iw == width && ih == height && index == 0)
                {

                }
                else
                {
                    int bgColor = Convert.ToInt32(gifImage.GlobalColorIndexedTable[f.GraphicExtension.TranIndex]);
                    Color c = Color.FromArgb(bgColor);
                    Bitmap newImg = null;
                    Graphics g;
                    newImg = new Bitmap(width, height);
                    g = Graphics.FromImage(newImg);
                    if (lastImage != null)
                    {
                        g.DrawImageUnscaled(lastImage, new Point(0, 0));
                    }
                    if (f.GraphicExtension.DisposalMethod == 1)
                    {
                        g.DrawRectangle(new Pen(new SolidBrush(c)), new Rectangle(xOffSet, yOffSet, iw, ih));
                    }
                    if (f.GraphicExtension.DisposalMethod == 2 && lastDisposal != 1)
                    {
                        g.Clear(c);
                    }
                    g.DrawImageUnscaled(f.Image, new Point(xOffSet, yOffSet));
                    g.Dispose();
                    f.Image.Dispose();
                    f.Image = newImg;
                }
                lastImage = f.Image;
                Quantizer(f.Image, f.Palette);
                lastDisposal = f.GraphicExtension.DisposalMethod;
                index++;
            }
        }
        static void Quantizer(Bitmap bmp, Color32[] colorTab)
        {
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            Hashtable table = new Hashtable();
            unsafe
            {
                int* bmpScan = (int*)bmpData.Scan0.ToPointer();
                for (int i = 0; i < bmp.Height * bmp.Width; i++)
                {
                    Color c = Color.FromArgb(bmpScan[i]);
                    int rc = FindCloser(c, colorTab, table);
                    Color newc = Color.FromArgb(rc);
                    bmpScan[i] = rc;
                }
            }
            bmp.UnlockBits(bmpData);
        }
        static void Quantizer(Bitmap bmp, int[] colorTab)
        {
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            Hashtable table = new Hashtable();
            unsafe
            {
                int* bmpScan = (int*)bmpData.Scan0.ToPointer();
                for (int i = 0; i < bmp.Height * bmp.Width; i++)
                {
                    Color c = Color.FromArgb(bmpScan[i]);
                    int rc = FindCloser(c, colorTab, table);
                    Color newc = Color.FromArgb(rc);
                    bmpScan[i] = rc;
                }
            }
            bmp.UnlockBits(bmpData);
        }
        static int FindCloser(Color c, Color32[] act, Hashtable table)
        {
            if (table.Contains(c))
            {
                return ((Color32)table[c]).ARGB;
            }
            int index = 0;
            int min = 0;
            int minIndex = 0;
            while (index < act.Length)
            {
                Color ac = act[index].Color;
                int tempIndex = index;
                int cr = Math.Abs(c.R - ac.R);
                int cg = Math.Abs(c.G - ac.G);
                int cb = Math.Abs(c.B - ac.B);
                int ca = Math.Abs(c.A - ac.A);
                int result = cr + cg + cb + ca;
                if (result == 0)
                {
                    minIndex = tempIndex;
                    break;
                }
                if (tempIndex == 0)
                {
                    min = result;
                }
                else
                {
                    if (result < min)
                    {
                        min = result;
                        minIndex = tempIndex;
                    }
                }
                index++;
            }
            if (!table.Contains(c))
            {
                table.Add(c, act[minIndex]);
            }
            return act[minIndex].ARGB;
        }
        static int FindCloser(Color c, int[] act, Hashtable table)
        {
            if (table.Contains(c))
            {
                return Convert.ToInt32(table[c]);
            }
            int index = 0;
            int min = 0;
            int minIndex = 0;
            while (index < act.Length)
            {
                Color ac = Color.FromArgb(act[index]);
                int tempIndex = index;
                int cr = Math.Abs(c.R - ac.R);
                int cg = Math.Abs(c.G - ac.G);
                int cb = Math.Abs(c.B - ac.B);
                int ca = Math.Abs(c.A - ac.A);
                int result = cr + cg + cb + ca;
                if (result == 0)
                {
                    minIndex = tempIndex;
                    break;
                }
                if (tempIndex == 0)
                {
                    min = result;
                }
                else
                {
                    if (result < min)
                    {
                        min = result;
                        minIndex = tempIndex;
                    }
                }
                index++;
            }
            if (!table.Contains(c))
            {
                table.Add(c, act[minIndex]);
            }
            return act[minIndex];
        }
    }
}
