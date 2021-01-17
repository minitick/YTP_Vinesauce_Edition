using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using Xabe.FFmpeg;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;

namespace YTP_Vinesauce_Edition
{
    public enum Effects 
    {
        RandomSound,
        Reverse,
        SpeedUp,
        SlowDown,
        PitchUp,
        PitchDown
    }
    
    static class YTP
    {
        //TODO: This needs to NOT be hardcoded at some point
        private static readonly int NumEffects;
        private static Random EffectPicker = new Random();

        //private static Xabe.FFmpeg.FFmpeg _ffmpeg;

        [STAThread]
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        static void Main()
        {
            string LogFilePath = ".\\debug.log";

            if (!File.Exists(LogFilePath))
            {

                using (StreamWriter sw = File.CreateText(LogFilePath))
                {
                    sw.WriteLine(GetTimestamp() + "\n");
                }
            }

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        /// <summary>
        /// Returns datetime in the universal ISO-8601 format.  Primarily used
        /// for writing out to the debug log.
        /// </summary>
        /// <returns></returns>
        static String GetTimestamp()
        {
            string Timestamp = DateTime.Today.ToString("u");

            return Timestamp;
        }

        private static async Task Reverse(string inputFile)
        {
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(inputFile);
            Debug.Write("MediaInfo set\n");

            IStream videoStream = mediaInfo.VideoStreams.FirstOrDefault()
                ?.SetCodec(VideoCodec.h264)
                ?.Reverse()
                ?.SetSize(VideoSize.Hd480);

            Debug.Write("Finished videoStream\n");

            var reverse = FFmpeg.Conversions.New()
                .AddStream(videoStream)
                .SetOutput(CreateOutputFilePath())
                .SetOverwriteOutput(true);

            await reverse.Start();

            Debug.Write("Exiting Reverse()\n");
        }

        private static string CreateOutputFilePath()
        {
            Guid guid = Guid.NewGuid();

            return Path.Combine(outputDirectory, guid.ToString() + ".mp4");
        }
    }


}
