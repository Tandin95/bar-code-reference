﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Barcode
{
    /// <summary>
    /// BitmapSourceの拡張メソッドクラス
    /// </summary>
    static class BitmapSourceExtensions
    {
        /// <summary>
        /// bitmap変換処理
        /// </summary>
        /// <param name="bitmapSource">BitmapSource</param>
        /// <param name="pixelFormat">カラーデータ形式</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(this BitmapSource bitmapSource, PixelFormat pixelFormat)
        {
            if(bitmapSource == null)
            {
                return null;
            }

            int width = bitmapSource.PixelWidth;

            int height = bitmapSource.PixelHeight;

            int stride = width * ((bitmapSource.Format.BitsPerPixel + 7) / 8);  // 行の長さは色深度によらず8の倍数のため

            IntPtr intPtr = IntPtr.Zero;

            try
            {

                intPtr = Marshal.AllocCoTaskMem(height * stride);

                bitmapSource.CopyPixels(new Int32Rect(0, 0, width, height), intPtr, height * stride, stride);

                using (var bitmap = new Bitmap(width, height, stride, pixelFormat, intPtr))
                {

                    // IntPtrからBitmapを生成した場合、Bitmapが存在する間、AllocCoTaskMemで確保したメモリがロックされたままとなる

                    // （FreeCoTaskMemするとエラーとなる）

                    // そしてBitmapを単純に開放しても解放されない

                    // このため、明示的にFreeCoTaskMemを呼んでおくために一度作成したBitmapから新しくBitmapを

                    // 再作成し直しておくとメモリリークを抑えやすい

                    return bitmap;
                }
            }
            catch (Exception ave)
            {
                Console.WriteLine(ave.Message);
                return null;
            }
            finally
            {

                if (intPtr != IntPtr.Zero)

                    Marshal.FreeCoTaskMem(intPtr);
            }
        }
    }
}
