using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace Common
{
    /// <summary>
    /// Class to contain constant
    /// </summary>
    public class Constant
    {
        /// <summary>
        /// Regions
        /// </summary>

        public const string RegionMain = "RegionMain";

        /// <summary>
        /// Modules
        /// </summary>

        public const string ModuleStart = "ModuleStartView";
        public const string ModuleExplorerType = "ModuleExplorerTypeView";
        public const string ModuleExplorerFile = "ModuleExplorerFileView";
        public const string ModuleVideoPlayer = "ModuleVideoPlayerView";
        public const string ModuleMusicPlayer = "ModuleMusicPlayerView";
        public const string ModuleImageViewer = "ModuleImageViewerView";
        public const string ModuleTextReader = "ModuleTextReaderView";

        /// <summary>
        /// Kinect
        /// </summary>

        public static int WindowWidth { get { return Convert.ToInt32(System.Windows.SystemParameters.PrimaryScreenWidth); } }

        public static int WindowHeight { get { return Convert.ToInt32(System.Windows.SystemParameters.PrimaryScreenHeight); } }

        public const double DistMoveToPush = 0.20;
        public const int SlideRatio = 200;

        /// <summary>
        /// Timer to manage the return in the start module
        /// </summary>

        public static DispatcherTimer Timer { get; set; }

        public const int TimeBeforeReturn = 500;

        public static void StopTimer()
        {
            if (Timer != null)
                Timer.Stop();
        }

        /// <summary>
        /// Max file number to display and element by line in ModuleExplorerFile
        /// </summary>

        public static int GetMaxFileNumber()
        {
            if (WindowWidth == 1366)
                return 10;
            else
                return 6;
        }

        public static int GetElementMaxByLine()
        {
            if (WindowWidth == 1366)
                return 6;
            else
                return 4;
        }
    }
}