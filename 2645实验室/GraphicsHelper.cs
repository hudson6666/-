﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsHelper
{
    class ImageAnalysis
    {
        static public Bitmap binaryzation(Bitmap baseRes, int limit = 200)
        {
            for (int i = 0; i < baseRes.Width; i++)
            {
                for (int j = 0; j < baseRes.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = baseRes.GetPixel(i, j);
                    Color newColor = (color.R < limit || color.B < limit) ?
                        Color.FromArgb(0, 0, 0) : Color.FromArgb(255, 255, 255);
                    baseRes.SetPixel(i, j, newColor);
                }
            }
            return baseRes;
        }
        static public int[] getVerticalHistogram(Bitmap baseRes)
        {
            int[] baseVertical = new int[baseRes.Width];
            for (int i = 0; i < baseRes.Width; i++)
            {
                baseVertical[i] = 0;
                for (int j = 0; j < baseRes.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = baseRes.GetPixel(i, j);
                    if (color.R == 0)
                        baseVertical[i] += 1;
                }
            }
            return baseVertical;
        }

        static public Bitmap getVerticalHistogram(int[] baseVertical, int height)
        {
            Bitmap verticalHistogram = new Bitmap(baseVertical.Count(), height);
            for (int i = 0; i < verticalHistogram.Width; i++)
            {
                for (int j = 0; j < verticalHistogram.Height; j++)
                {
                    Color newColor = j > verticalHistogram.Height - baseVertical[i] ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255,
255, 255);
                    verticalHistogram.SetPixel(i, j, newColor);
                }
            }
            return verticalHistogram;
        }

        static public int[] getHorizontalHistogram(Bitmap baseRes)
        {
            int[] baseHorizontal = new int[baseRes.Height];
            for (int j = 0; j < baseRes.Height; j++)
            {
                baseHorizontal[j] = 0;
                for (int i = 0; i < baseRes.Width; i++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = baseRes.GetPixel(i, j);
                    if (color.R == 0)
                        baseHorizontal[j] += 1;
                }
            }
            return baseHorizontal;
        }

        static public Bitmap getHorizontalHistogram(int[] baseHorizontal, int Width)
        {
            Bitmap horizontalHistogram = new Bitmap(Width, baseHorizontal.Count());
            for (int j = 0; j < horizontalHistogram.Height; j++)
            {
                for (int i = 0; i < horizontalHistogram.Width; i++)
                {
                    Color newColor = i < baseHorizontal[j] ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255,
255, 255);
                    horizontalHistogram.SetPixel(i, j, newColor);
                }
            }
            return horizontalHistogram;
        }

        static public Bitmap removeBlankVerticalLines(Bitmap baseRes)
        {
            return removeBlankVerticalLines(baseRes, getVerticalHistogram(baseRes));
        }

        static public Bitmap removeBlankVerticalLines(Bitmap baseRes, int[] verticalHistogram)
        {
            List<int> verHisList = verticalHistogram.ToList();
            verHisList.Remove(0);
            Bitmap ret = new Bitmap(verHisList.Count, baseRes.Height);
            for(int iraw = 0, i = 0; iraw < ret.Width; ++iraw)
            {
                if(verticalHistogram[iraw] != 0)
                {
                    for(int j = 0; j < baseRes.Height; ++j)
                    {
                        ret.SetPixel(i, j, baseRes.GetPixel(iraw, j));
                    }
                    ++i;
                }
            }
            return ret;
        }
        public static Bitmap ResizeImage(Bitmap bmp, int newW, int newH)
        {
            try
            {
                Bitmap b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量   
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }
        public static double getVerticalSimilarity(int[] a, int[] b)
        {
            if (a.Length != b.Length)
                throw new Exception("The length of two vectors are not the same!");
            long sum = 0;
            for(int i = 0; i < a.Length - 1; ++i)
                sum += (a[i] - b[i]) * (a[i] - b[i]);
            return Math.Sqrt(sum);
        } 

        static public bool getVerticalSimilarity(Bitmap bmp1, Bitmap bmp2, double limit)
        {
            return getVerticalSimilarity(bmp1, bmp2) < limit;
        }
        static public double getVerticalSimilarity(Bitmap bmp1, Bitmap bmp2)
        {
            bmp1 = binaryzation(bmp1);
            bmp2 = binaryzation(bmp2);
            bmp1 = removeBlankVerticalLines(bmp1);
            bmp2 = removeBlankVerticalLines(bmp2);
            if (bmp1.Width < bmp2.Width)
                bmp2 = ResizeImage(bmp2, bmp1.Width, bmp1.Height);
            else
                bmp1 = ResizeImage(bmp1, bmp2.Width, bmp2.Height);
            bmp1 = binaryzation(bmp1);
            bmp2 = binaryzation(bmp2);
            return getVerticalSimilarity(getVerticalHistogram(bmp1), getVerticalHistogram(bmp2));
        }
    }
}
