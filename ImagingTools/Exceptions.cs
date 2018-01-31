using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace ImagingTools
{
    /// <summary>
    /// This is a custom exception raised when the time is less than 0 or greater than the video duration
    /// </summary>
    public class MediaToolsGenericException : Exception
    {
        /// <summary>
        /// Exception thrown when an unexcpected error happens
        /// </summary>
        /// <param name="exception_name">The name of the exception</param>
        /// <param name="exception_message">The exception message</param>
        public MediaToolsGenericException(string mediatools_method, Exception unexpected_exception)
        {
            Console.WriteLine(
                string.Format(
                    "MediaTools encountered an unexpected error in {0}. Details: Exception name: {1}, Exception Message: {2}",
                    mediatools_method,
                    unexpected_exception.GetType().Name,
                    unexpected_exception.Message
                )
            );
        }
    }
    /// <summary>
    /// This is a custom exception raised when the time is less than 0 or greater than the video duration
    /// </summary>
    public class InvalidTimeException : Exception
    {
        /// <summary>
        /// The exception constructor to be used when the exception is caught and thrown
        /// </summary>
        /// <param name="requested_time">The time (in seconds) that the user specified</param>
        /// <param name="video_name">The name of the video</param>
        /// <param name="video_duration">The duration (in seconds) of the video</param>
        public InvalidTimeException(double requested_time, string video_name, double video_duration)
        {
            Console.WriteLine(
                string.Format(
                    "The specified time ({0}) is not valid.  The time for {1} must be between 0 and {2} seconds.",
                    requested_time, video_name, video_duration
                )
            );
        }
    }
    /// <summary>
    /// This is a custom exception raised when a saved file does not exist
    /// </summary>
    public class FileNotSavedException : Exception
    {
        /// <summary>
        /// The exception constructor to be used when the exception is caught and thrown
        /// </summary>
        /// <param name="file_info">FileInfo object representing the file</param>
        /// <param name="file_path_exists">bool indicating whether or not the path exists</param>
        /// <param name="file_name_exists">bool indicating whether or not the file exists</param>
        public FileNotSavedException(FileInfo file_info, bool file_path_exists, bool file_name_exists)
        {
            string exception_reason = string.Format("The file {0} does not exist.", file_info.Name);
            if(!file_path_exists){
                exception_reason = string.Format("The directory {0} does not exist.", file_info.DirectoryName);
            }
            Console.WriteLine(
                string.Format(
                    "The file ({0}) was not saved. {1}.",
                    file_info.Name,
                    exception_reason
                )
            );
        }
    }
}
