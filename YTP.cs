using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        private static Xabe.FFmpeg.FFmpeg _ffmpeg;

        [STAThread]
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        static void Main()
        {

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static void Reverse(String InputFilePath)
        {
            Debug.WriteLine(DateTime.Today.ToString("yyyy") "Reverse called");

            // 
            


            return;
        }
    }


}
