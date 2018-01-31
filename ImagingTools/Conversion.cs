using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;

namespace ImagingTools
{
    public static class Conversion
    {
        /// <summary>
        /// Converts a Bitmap to a format suitable for use as Image.Source
        /// </summary>
        /// <param name="bitmap_to_convert">The System.Windows.Drawing.Bitmap you want to convert</param>
        /// <returns>System.Windows.Media.Imaging.BitmapImage</returns>
        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap_to_convert)
        {
            try
            {
                using (MemoryStream bitmap_memory = new MemoryStream())
                {
                    bitmap_to_convert.Save(bitmap_memory, ImageFormat.Bmp);
                    bitmap_memory.Position = 0;
                    BitmapImage converted_bitmapimage = new BitmapImage();
                    converted_bitmapimage.BeginInit();
                    converted_bitmapimage.StreamSource = bitmap_memory;
                    converted_bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    converted_bitmapimage.EndInit();

                    return converted_bitmapimage;
                }
            }
            catch (Exception e)
            {
                throw new ImagingTools.MediaToolsGenericException(
                    "Conversion.BitmapToImageSource(Bitmap bitmap_to_convert)",
                    e
                );
            }
        }
  
        /// <summary>
        /// Converts a System.Drawing.Image to a format suitable for use as Image.Source
        /// </summary>
        /// <param name="image_to_convert">The System.Windows.Drawing.Image you want to convert</param>
        /// <returns>System.Windows.Media.Imaging.BitmapImage</returns>
        public static BitmapImage ImageToBitmapImage(Image image_to_convert)
        {
            try
            {
                return Conversion.BitmapToBitmapImage((Bitmap)image_to_convert);
            }
            catch(Exception e)
            {
                throw new ImagingTools.MediaToolsGenericException(
                    "Conversion.BitmapToImageSource(Bitmap bitmap_to_convert)",
                    e
                );
            }
        }
    }
}
