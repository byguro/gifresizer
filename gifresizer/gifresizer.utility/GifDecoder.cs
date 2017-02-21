using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace gifresizer.utility
{
    internal class GifDecoder
    {
        internal static GifImage Decode(string gifPath)
        {
            FileStream fs = null;
            StreamHelper streamHelper = null;
            GifImage gifImage = new GifImage();
            List<GraphicEx> graphics = new List<GraphicEx>();
            int frameCount = 0;
            try
            {
                fs = new FileStream(gifPath, FileMode.Open);
                streamHelper = new StreamHelper(fs);
                gifImage.Header = streamHelper.ReadString(6);
                gifImage.LogicalScreenDescriptor = streamHelper.GetLCD(fs);
                if (gifImage.LogicalScreenDescriptor.GlobalColorTableFlag)
                {
                    gifImage.GlobalColorTable = streamHelper.ReadByte(gifImage.LogicalScreenDescriptor.GlobalColorTableSize * 3);
                }
                int nextFlag = streamHelper.Read();
                while (nextFlag != 0)
                {
                    if (nextFlag == GifExtensions.ImageLabel)
                    {
                        ReadImage(streamHelper, fs, gifImage, graphics, frameCount);
                        frameCount++;
                    }
                    else if (nextFlag == GifExtensions.ExtensionIntroducer)
                    {
                        int gcl = streamHelper.Read();
                        switch (gcl)
                        {
                            case GifExtensions.GraphicControlLabel:
                                {
                                    GraphicEx graphicEx = streamHelper.GetGraphicControlExtension(fs);
                                    graphics.Add(graphicEx);
                                    break;
                                }
                            case GifExtensions.CommentLabel:
                                {
                                    CommentEx comment = streamHelper.GetCommentEx(fs);
                                    gifImage.CommentExtensions.Add(comment);
                                    break;
                                }
                            case GifExtensions.ApplicationExtensionLabel:
                                {
                                    ApplicationEx applicationEx = streamHelper.GetApplicationEx(fs);
                                    gifImage.ApplictionExtensions.Add(applicationEx);
                                    break;
                                }
                            case GifExtensions.PlainTextLabel:
                                {
                                    PlainTextEx textEx = streamHelper.GetPlainTextEx(fs);
                                    gifImage.PlainTextEntensions.Add(textEx);
                                    break;
                                }
                        }
                    }
                    else if (nextFlag == GifExtensions.EndIntroducer)
                    {
                        break;
                    }
                    nextFlag = streamHelper.Read();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                fs.Close();
            }
            return gifImage;
        }
        static void ReadImage(StreamHelper streamHelper, Stream fs, GifImage gifImage, List<GraphicEx> graphics, int frameCount)
        {
            ImageDescriptor imgDes = streamHelper.GetImageDescriptor(fs);
            GifFrame frame = new GifFrame();
            frame.ImageDescriptor = imgDes;
            frame.LocalColorTable = gifImage.GlobalColorTable;
            if (imgDes.LctFlag)
            {
                frame.LocalColorTable = streamHelper.ReadByte(imgDes.LctSize * 3);
            }
            LZWDecoder lzwDecoder = new LZWDecoder(fs);
            int dataSize = streamHelper.Read();
            frame.ColorDepth = dataSize;
            byte[] piexel = lzwDecoder.DecodeImageData(imgDes.Width, imgDes.Height, dataSize);
            frame.IndexedPixel = piexel;
            int blockSize = streamHelper.Read();
            DataStruct data = new DataStruct(blockSize, fs);
            GraphicEx graphicEx = graphics[frameCount];
            frame.GraphicExtension = graphicEx;
            Bitmap img = GetImageFromPixel(piexel, frame.Palette, imgDes.InterlaceFlag, imgDes.Width, imgDes.Height);
            frame.Image = img;
            gifImage.Frames.Add(frame);
        }
        static Bitmap GetImageFromPixel(byte[] pixel, Color32[] colorTable, bool interlactFlag, int iw, int ih)
        {
            Bitmap img = new Bitmap(iw, ih);
            BitmapData bmpData = img.LockBits(new Rectangle(0, 0, iw, ih), ImageLockMode.ReadWrite, img.PixelFormat);
            unsafe
            {
                Color32* p = (Color32*)bmpData.Scan0.ToPointer();
                Color32* tempPointer = p;
                int offSet = 0;
                if (interlactFlag)
                {
                    int i = 0;
                    int pass = 0;
                    while (pass < 4)
                    {
                        if (pass == 1)
                        {
                            p = tempPointer;
                            p += (4 * iw);
                            offSet += 4 * iw;
                        }
                        else if (pass == 2)
                        {
                            p = tempPointer;
                            p += (2 * iw);
                            offSet += 2 * iw;
                        }
                        else if (pass == 3)
                        {
                            p = tempPointer;
                            p += (1 * iw);
                            offSet += 1 * iw;
                        }
                        int rate = 2;
                        if (pass == 0 | pass == 1)
                        {
                            rate = 8;
                        }
                        else if (pass == 2)
                        {
                            rate = 4;
                        }
                        while (i < pixel.Length)
                        {
                            *p++ = colorTable[pixel[i++]];
                            offSet++;
                            if (i % (iw) == 0)
                            {
                                p += (iw * (rate - 1));
                                offSet += (iw * (rate - 1));
                                if (offSet >= pixel.Length)
                                {
                                    pass++;
                                    offSet = 0;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    int i = 0;
                    for (i = 0; i < pixel.Length;)
                    {
                        *p++ = colorTable[pixel[i++]];
                    }
                }
            }
            img.UnlockBits(bmpData);
            return img;
        }

    }
}