using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
// Bitmap
using System.Windows.Media;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows;
// Leap
using Leap;

namespace RecognitionGestureFeed_Universal.Recognition.Leap.Stream
{
    public static class StreamUpdate
    {
        /* Attributi */

        /* Metodi */
        public static void convertBitmap(this Bitmap bitmap, ImageList imageList)
        {
            //bitmap.Lock();
            //bitmap.WritePixels(new Int32Rect(0, 0, image.Width, image.Height), image.Data, image.BytesPerPixel, 0);// Disegno i nuovi pixel
            //bitmap.Unlock();
            bitmap = new Bitmap(imageList[0].Width, imageList[0].Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            // Set palette
            ColorPalette grayscale = bitmap.Palette;
            for (int i = 0; i < 256; i++)
            {
                grayscale.Entries[i] = System.Drawing.Color.FromArgb((int)255, i, i, i);
            }
            bitmap.Palette = grayscale;
            System.Drawing.Rectangle lockArea = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bitmapData = bitmap.LockBits(lockArea, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            byte[] rawImageData = imageList[0].Data;
            System.Runtime.InteropServices.Marshal.Copy(rawImageData, 0, bitmapData.Scan0, imageList[0].Width * imageList[0].Height);
            bitmap.UnlockBits(bitmapData);
        }
    }
}
