using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.IO;

namespace Barcode
{
    /// <summary>
    /// BitmapをBitmapFrameに変換する
    /// </summary>
    internal class BitmapToBitmapFrame
    {
        /// <summary>
        /// BitmapFrameに変換
        /// </summary>
        /// <param name="src">変換対象ソース</param>
        /// <returns>BitmapFrame</returns>
        internal static BitmapFrame Convert(Bitmap src)
        {
            using (var stream = new MemoryStream())
            {
                src.Save(stream, ImageFormat.Bmp);
                return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }
    }
}
