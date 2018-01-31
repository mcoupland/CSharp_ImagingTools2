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
    public static class InternalTools
    {
        /// <summary>
        /// Creates a MediaPlayer for the video and jumps to the specified time
        /// </summary>
        /// <param name="video_path">The path to the video that you want to image capture</param>
        /// <param name="time_in_seconds">The video will be paused at this time (in seconds)</param>
        /// <param name="sleep_time">The MediaPlayer needs time to do it's thing, this lets you tweak the wait time in milliseconds</param>
        /// <returns>Returns the MediaPlayer paused at the specified time</returns>
        internal static MediaPlayer GetPausedVideo(string video_path, double time_in_seconds, int sleep_time)
        {
            MediaPlayer video_player = new MediaPlayer { Volume = 0, ScrubbingEnabled = true };
            video_player.Open(new Uri(video_path));
            video_player.Play();
            Thread.Sleep(sleep_time);  // The player is a little finicky, you have to open, pause and sleep for it to work. I've not had good luck using the MediaPlayer events.
            video_player.Pause();
            double video_duration = video_player.NaturalDuration.TimeSpan.TotalSeconds;  // need this value because it is not available in the exception
            if (time_in_seconds < 0 || time_in_seconds > video_duration)
            {
                video_player.Close();
                throw new ImagingTools.InvalidTimeException(time_in_seconds, video_path, video_duration);
            }
            video_player.Position = TimeSpan.FromSeconds(time_in_seconds);
            Thread.Sleep(sleep_time);
            return video_player;
        }

        /// <summary>
        /// Gets the image data from the paused video
        /// </summary>
        /// <param name="video_player"></param>
        /// <returns>Returns a BitmapFrame ready to be encoded</returns>
        internal static BitmapFrame GetVideoImageData(MediaPlayer video_player)
        {
            int pixel_width = video_player.NaturalVideoWidth;  // For some reason you have to set these values in a variable.
            int pixel_height = video_player.NaturalVideoHeight;  // Using *.NaturalVideoWidth or *.NaturalVideoHeight does not work
            RenderTargetBitmap rtb = new RenderTargetBitmap
            (
                pixel_width,
                pixel_height,
                96,
                96,
                PixelFormats.Pbgra32
            );
            Rect crop = new Rect(0, 0, pixel_width, pixel_height);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                dc.DrawVideo(video_player, crop);
            }
            rtb.Render(dv);
            return BitmapFrame.Create(rtb).GetCurrentValueAsFrozen() as BitmapFrame;
        }

        /// <summary>
        /// Gets the encoder that will properly encode the image data into an image
        /// </summary>
        /// <param name="image_data"></param>
        /// <returns>Returns an encoder for the image data</returns>
        internal static BitmapEncoder GetImageDataEncoder(BitmapFrame image_data)
        {
            BitmapEncoder encoder_for_image_data = new PngBitmapEncoder();
            encoder_for_image_data.Frames.Add(image_data as BitmapFrame);
            return encoder_for_image_data;
        }

        /// <summary>
        /// Encodes the image data, saves the image and closes the video player.
        /// </summary>
        /// <param name="image_path"></param>
        /// <param name="image_data_encoder"></param>
        /// <param name="video_player"></param>
        internal static FileInfo SaveImage(string image_path, BitmapEncoder image_data_encoder, MediaPlayer video_player)
        {
            using (FileStream image_filestream = new FileStream(image_path, FileMode.Create))
            {
                image_data_encoder.Save(image_filestream);
                video_player.Close();
                image_filestream.Close();
                image_filestream.Dispose();
            }
            return new FileInfo(image_path);
        }

        /// <summary>
        /// Creates a matrix to convert an image to grayscale
        /// </summary>
        /// <returns></returns>
        internal static ColorMatrix GetGrayscaleMatrix()
        {
             return new ColorMatrix(
                new float[][]
                {
                    new float[] {.3f, .3f, .3f, 0, 0},
                    new float[] {.59f, .59f, .59f, 0, 0},
                    new float[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                }
            );
        }

        internal static ResizeType GetResizeType(int original_value, int new_value)
        {
            return original_value >= new_value ? ResizeType.Reduction : ResizeType.Enlargement;
        }
        
        internal static Orientation GetBitmapOrientation(Bitmap source_bitmap)
        {
            return source_bitmap.Height >= source_bitmap.Width ? Orientation.Portrait : Orientation.Landscape;
        }

        internal static System.Drawing.Size GetScaledSize(Bitmap source_bitmap, float max_dimension_value)
        {
            Orientation bitmap_orientation = GetBitmapOrientation(source_bitmap);
            System.Drawing.SizeF bitmap_size = new SizeF(source_bitmap.Size);
            System.Drawing.SizeF scaled_size = new SizeF(source_bitmap.Size);
            if (bitmap_orientation == Orientation.Landscape)
            {
                scaled_size.Width = max_dimension_value;
                scaled_size.Height = GetDerivedDimension(
                    bitmap_size.Width,
                    max_dimension_value,
                    bitmap_size.Height
                );
            }
            else
            {
                scaled_size.Height = max_dimension_value;
                scaled_size.Width = GetDerivedDimension(
                    bitmap_size.Height,
                    max_dimension_value,
                    bitmap_size.Width
                );                
            }
            return new System.Drawing.Size(Convert.ToInt32(Math.Floor(scaled_size.Width)), Convert.ToInt32(Math.Floor(scaled_size.Height)));
        }

        public static float GetDerivedDimension(
            float constant_value, 
            float max_dimension_value, 
            float derived_value
        )
        {
            ResizeType resize_type = InternalTools.GetResizeType(
                    (int)constant_value,
                    (int)max_dimension_value
                );
            float multiplier = 0f;
            if (resize_type == ResizeType.Reduction)
            {
                multiplier = max_dimension_value / constant_value;
            }
            else
            {
                multiplier = constant_value / max_dimension_value;
            }
            return derived_value * multiplier;
        }
    }
}
