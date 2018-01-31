using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImagingTools
{
    public static class BitmapTools
    {
        /// <summary>
        /// Captures an image from a video
        /// </summary>
        /// <param name="source_video">FileInfo contia</param>
        /// <param name="capture_time">Time in seconds at which you want to capture the image</param>
        /// <param name="sleep_time">Time in milliseconds to sleep so that MediaPlayer doesn't crap itself</param>
        /// <returns>A System.Drawing.Bitmap captured from the video</returns>
        public static Bitmap Capture(FileInfo source_video, double capture_time, int sleep_time)
        {
            try
            {
                MediaPlayer paused_video = InternalTools.GetPausedVideo(source_video.FullName, capture_time, sleep_time);
                BitmapFrame image_data_at_position = InternalTools.GetVideoImageData(paused_video);
                BitmapEncoder image_data_encoder = InternalTools.GetImageDataEncoder(image_data_at_position);
                Bitmap captured_bitmap;

                using (MemoryStream stream = new MemoryStream())
                {
                    image_data_encoder.Frames.Add(image_data_at_position);
                    image_data_encoder.Save(stream);
                    byte[] bit = stream.ToArray();
                    captured_bitmap = Bitmap.FromStream(stream) as Bitmap;
                    stream.Close();
                }
                return captured_bitmap;
            }
            catch (Exception e)
            {
                throw new ImagingTools.MediaToolsGenericException(
                    "Capture_Bitmap.Capture(FileInfo source_video, double capture_time, int sleep_time)",
                    e
                );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original_bitmap"></param>
        /// <param name="new_size"></param>
        /// <returns></returns>
        public static Bitmap Resize(Bitmap original_bitmap, int resize_percentage)
        {
            try 
            {
                float percentage = (float)resize_percentage / 100f;
                float f_width = (float)original_bitmap.Width;
                float f_height = (float)original_bitmap.Height;

                int new_width = Convert.ToInt32(Math.Floor(f_width * percentage));
                int new_height = Convert.ToInt32(Math.Floor(f_height * percentage));

                System.Drawing.Size new_size = new System.Drawing.Size(
                    new_width,
                    new_height
                );
                return new Bitmap(
                    original_bitmap, 
                    new_size
                );
            }
            catch (Exception e)
            {
                throw new ImagingTools.MediaToolsGenericException(
                    "BitmapTools.Resize(Bitmap original_bitmap, Size new_size)",
                    e
                );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original_bitmap"></param>
        /// <param name="new_width"></param>
        /// <param name="new_height"></param>
        /// <returns></returns>
        public static Bitmap Resize(Bitmap original_bitmap, float new_width, float new_height)
        {
            int width = Convert.ToInt32(Math.Floor(new_width));
            int height = Convert.ToInt32(Math.Floor(new_height));
            return Resize(original_bitmap, width, height);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="original_bitmap"></param>
        /// <param name="new_width"></param>
        /// <param name="new_height"></param>
        /// <returns></returns>
        public static Bitmap Resize(Bitmap original_bitmap, int new_width, int new_height)
        {
            try
            {
                System.Drawing.Size new_size = new System.Drawing.Size(
                    new_width,
                    new_height
                );
                return new Bitmap(
                    original_bitmap,
                    new_size
                );
            }
            catch (Exception e)
            {
                throw new ImagingTools.MediaToolsGenericException(
                    "BitmapTools.Resize(Bitmap original_bitmap, Size new_size)",
                    e
                );
            }
        }

        /// <summary>
        /// Save a bitmap to the specified location.
        /// </summary>
        /// <param name="source_bitmap">The bitmap to save</param>
        /// <param name="saved_bitmap_file_info">The full path & file name where the bitmap will be saved</param>
        /// <returns>The full path and file name if it was saved, else throws an exception</returns>
        public static FileInfo Save(Bitmap source_bitmap, FileInfo saved_bitmap_file_info)
        {
            try
            {
                source_bitmap.Save(saved_bitmap_file_info.FullName);                
                return saved_bitmap_file_info;
            }
            catch (Exception e)
            {
                throw new ImagingTools.MediaToolsGenericException(
                    "Save_Bitmap.Save(Bitmap source_bitmap, FileInfo saved_bitmap_file_info)",
                    e
                );
            }
        }

        /// <summary>
        /// Make a grayscale copy of the bitmap
        /// </summary>
        /// <param name="source_bitmap">The System.Drawing.Bitmap you want to convert</param>
        /// <returns>Grayscale copy of the original bitmap</returns>
        public static Bitmap Desaturate(Bitmap source_bitmap)
        {
            try
            {
                Rectangle source_rectangle = new Rectangle(0, 0, source_bitmap.Width, source_bitmap.Height);
                //create a blank bitmap the same size as original
                Bitmap desaturated_bitmap = new Bitmap(source_bitmap.Width, source_bitmap.Height);
                Graphics g = Graphics.FromImage(desaturated_bitmap);

                ImageAttributes desaturated_bitmap_attributes = new ImageAttributes();
                desaturated_bitmap_attributes.SetColorMatrix(InternalTools.GetGrayscaleMatrix());

                //draw the original image on the new image
                //using the grayscale color matrix
                g.DrawImage(
                    source_bitmap,
                    source_rectangle,
                    0,
                    0,
                    source_bitmap.Width,
                    source_bitmap.Height,
                    GraphicsUnit.Pixel,
                    desaturated_bitmap_attributes
                );

                //dispose the Graphics object
                g.Dispose();
                return desaturated_bitmap;   
            }
            catch (Exception e)
            {
                throw new ImagingTools.MediaToolsGenericException(
                    "Save_Bitmap.Save(Bitmap source_bitmap, FileInfo saved_bitmap_file_info)",
                    e
                );
            }         
        }
        
        /// <summary>
        /// The GDI+ method Image.FromFile(...) has a garbage collection bug
        /// This safely opens and returns a System.Drawing.Image
        /// </summary>
        /// <param name="image_full_path"></param>
        /// <returns>System.Drawing.Image</returns>
        public static Image ImageFromFile(string image_full_path)
        {
            return ImageFromFileInfo(new FileInfo(image_full_path));
        }

        /// <summary>
        /// The GDI+ method Image.FromFile(...) has a garbage collection bug
        /// This safely opens and returns a System.Drawing.Image
        /// </summary>
        /// <param name="image_file_info"></param>
        /// <returns>System.Drawing.Image</returns>
        public static Image ImageFromFileInfo(FileInfo image_file_info)
        {
            return BitmapFromFileInfo(image_file_info) as Image;
        }
        
        /// <summary>
        /// The GDI+ method Bitmap.FromFile(...) has a garbage collection bug
        /// This safely opens and returns a System.Drawing.Bitmap
        /// </summary>
        /// <param name="bitmap_full_path"></param>
        /// <returns></returns>
        public static Bitmap BitmapFromFile(string bitmap_full_path)
        {
           return BitmapFromFileInfo(new FileInfo(bitmap_full_path));
        }

        /// <summary>
        /// The GDI+ method Bitmap.FromFile(...) has a garbage collection bug
        /// This safely opens and returns a System.Drawing.Bitmap
        /// </summary>
        /// <param name="bitmap_file_info"></param>
        /// <returns></returns>
        public static Bitmap BitmapFromFileInfo(FileInfo bitmap_file_info)
        {
            using (FileStream bitmap_stream = new FileStream(bitmap_file_info.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (Bitmap bitmap_from_file = new Bitmap(bitmap_stream))
                {
                    return new Bitmap(bitmap_from_file);
                }
            }
        }

        /// <summary>
        /// Scale a bitmap to a max dimension, maintaining the original aspect ratio
        /// </summary>
        /// <param name="source_bitmap"></param>
        /// <param name="max_dimension_value"></param>
        /// <returns>Scaled System.Drawing.Bitmap</returns>
        public static Bitmap Scale(Bitmap source_bitmap, int max_dimension_value)
        {
            try
            {
                System.Drawing.Size new_size = InternalTools.GetScaledSize(source_bitmap, max_dimension_value);
                return BitmapTools.Resize(source_bitmap, new_size.Width, new_size.Height);
            }
            catch (Exception e)
            {
                throw new ImagingTools.MediaToolsGenericException(
                    "Save_Bitmap.Save(Bitmap source_bitmap, FileInfo saved_bitmap_file_info)",
                    e
                );
            }
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source_bitmap"></param>
        /// <param name="start_x"></param>
        /// <param name="start_y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap Crop(Bitmap source_bitmap, int start_x, int start_y, int width, int height)
        {
            Bitmap cloned_bitmap = new Bitmap(source_bitmap);
            Rectangle crop_box = new Rectangle(start_x, start_y, width, height);
            return cloned_bitmap.Clone(crop_box, cloned_bitmap.PixelFormat);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desired_file_name"></param>
        /// <returns></returns>
        public static FileInfo GetUniqueFileInfoName(FileInfo desired_file_name)
        {
            if(!Directory.Exists(desired_file_name.DirectoryName))
            {
                DirectoryInfo desired_directory = new DirectoryInfo(desired_file_name.DirectoryName);
                Directory.CreateDirectory(desired_file_name.DirectoryName);
            }
            if (!File.Exists(desired_file_name.FullName))
            {
                return desired_file_name;
            }
            string name_without_extension = desired_file_name.FullName.Replace(
                desired_file_name.Extension,
                string.Empty
            );
            string file_extension = desired_file_name.Extension;
            string unique_file_name = desired_file_name.FullName;
            int unique_counter = 0;
            while (File.Exists(unique_file_name))
            {
                unique_file_name = string.Format(
                    "{0}_{1}.{2}",
                    name_without_extension,
                    unique_counter.ToString(),
                    file_extension
                );
                unique_counter++;
            }
            return new FileInfo(unique_file_name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source_bitmap"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap GetBitmapThumbnail(Bitmap source_bitmap, int thumbnail_dimension, DimensionType apply_to)
        {
            float bitmap_width = (float)source_bitmap.Width;
            float bitmap_height = (float)source_bitmap.Height;

            float thumbnail_width = 0f;
            float thumbnail_height = 0f;

            if(apply_to == DimensionType.Height)
            {
                thumbnail_height = thumbnail_dimension;
                thumbnail_width = bitmap_width * thumbnail_height / bitmap_height;
            }
            else
            {
                thumbnail_width = thumbnail_dimension;
                thumbnail_height = bitmap_height * thumbnail_width / bitmap_width;
            }
            return BitmapTools.Resize(source_bitmap, thumbnail_width, thumbnail_height);
        }
    
    }
}
