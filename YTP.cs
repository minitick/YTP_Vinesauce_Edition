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
        private static string outputDirectory = "C:\\temp";

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

        /// <summary>
        /// Given a video file, reverses the video and audio streams, and 
        /// creates a "clip" in outputDirectory.
        /// </summary>
        /// <param name="inputFile">The video file which will be 
        /// reversed</param>
        /// <returns></returns>
        private static async Task Reverse(string inputFile)
        {
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(inputFile);
            Debug.Write("MediaInfo set\n");

            IStream videoStream = mediaInfo.VideoStreams.FirstOrDefault()
                ?.SetCodec(VideoCodec.h264)
                ?.Reverse()
                ?.SetSize(VideoSize.Hd480);

            Debug.Write("Finished videoStream\n");

            IStream audioStream = mediaInfo.AudioStreams.FirstOrDefault()
                ?.Reverse();

            Debug.Write("Finished audioStream\n");

            var reverse = FFmpeg.Conversions.New()
                .AddStream(videoStream)
                .AddStream(audioStream)
                .SetOutput(CreateOutputFilePath())
                .SetOverwriteOutput(true);

            await reverse.Start();

            Debug.Write("Exiting Reverse()\n");
        }


        /// <summary>
        /// Creates a clip based on changing the playback speed.  Designed to
        /// be used to speed up (2x) or slow down (.5x).
        /// </summary>
        /// <returns></returns>
        private static async Task ChangeSpeed(IMediaInfo input, double speed)
        {
            Debug.Write("Entering ChangeSpeed. Speed is " + speed.ToString() +"\n");

            IStream videoStream = input.VideoStreams.FirstOrDefault()
                ?.SetCodec(VideoCodec.h264)
                ?.SetSize(VideoSize.Hd480)
                ?.ChangeSpeed(speed);

            Debug.Write("ChangeSpeed(): Finished videoStream\n");

            var changespeed = FFmpeg.Conversions.New()
                .AddStream(videoStream)
                .SetOutput(CreateOutputFilePath())
                .SetOverwriteOutput(true);

            await changespeed.Start();

            Debug.Write("Exiting ChangeSpeed() (speed was "+speed+"\n");
        }

        /// <summary>
        /// Creates a clip with its audio pitched up. Implements this logic:
        /// http://johnriselvato.com/ffmpeg-how-to-change-the-pitch-sample-rate-of-an-audio-track-mp3/
        /// </summary>
        /// <returns></returns>
        private static async Task PitchUp(IMediaInfo input)
        {
            Debug.Write("Entering PitchUp()\n");

            IStream videoStream = input.VideoStreams.FirstOrDefault()
                ?.SetCodec(VideoCodec.h264)
                ?.SetSize(VideoSize.Hd480);

            Debug.Write("PitchUp(): Finished videoStream\n");

            IStream audioStream = input.AudioStreams.FirstOrDefault()
                //TODO: Make the sample rate dynamic based on input audio.
                // Also need to test the length of audio, make sure it's 1:1 with
                // the original audio.
                // Investigate using this, in case it's better: 
                // https://manuelhans.com/blog/2020/01/09/changing-audio-pitch-with-ffmpeg/
                ?.AddParameter("-af \"asetrate=44100*2,atempo=.5,aresample=44100\"");

            var pitchup = FFmpeg.Conversions.New()
                .AddStream(audioStream, videoStream)
                .SetOutput(CreateOutputFilePath())
                .SetOverwriteOutput(true);

            await pitchup.Start();

            Debug.Write("Exiting PitchUp()\n");
        }

        /// <summary>
        /// Creates a clip with its audio pitched down.
        /// </summary>
        /// <returns></returns>
        private static async Task PitchDown(IMediaInfo input)
        {

        }

        private static string CreateOutputFilePath()
        {
            Guid guid = Guid.NewGuid();

            return Path.Combine(outputDirectory, guid.ToString() + ".mp4");
        }
    }


}
