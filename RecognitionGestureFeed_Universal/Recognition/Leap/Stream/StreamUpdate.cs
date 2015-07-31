using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
// Bitmap
using System.Windows.Media;
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
        public static void convertBitmap(this WriteableBitmap bitmap, Image image)
        {
            bitmap.Lock();
            bitmap.WritePixels(new Int32Rect(0, 0, image.Width, image.Height), image.Data, image.BytesPerPixel, 0);// Disegno i nuovi pixel
            bitmap.Unlock();
            /*//Bitmap bitmap2 = new Bitmap(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            // Set palette
            ColorPalette grayscale = bitmap.Palette;
            for (int i = 0; i < 256; i++)
            {
                grayscale.Entries[i] = Color.FromArgb((int)255, i, i, i);
            }
            bitmap.Palette = grayscale;
            Rectangle lockArea = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bitmapData = bitmap.LockBits(lockArea, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            byte[] rawImageData = image.Data;
            System.Runtime.InteropServices.Marshal.Copy(rawImageData, 0, bitmapData.Scan0, image.Width * image.Height);
            bitmap.UnlockBits(bitmapData);*/
        }
    }
}
