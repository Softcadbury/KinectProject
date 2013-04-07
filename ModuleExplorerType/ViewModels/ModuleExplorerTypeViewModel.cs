using Common;
using Common.ManageMove;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ModuleExplorerType.ViewModels
{
    public class ModuleExplorerTypeViewModel : BaseViewModel
    {
        #region Constructor & OnCopyDataReceived

        public ModuleExplorerTypeViewModel()
        {
            VideoCommand = new RelayCommand(param => VideoMethod());
            MusicCommand = new RelayCommand(param => MusicMethod());
            ImageCommand = new RelayCommand(param => ImageMethod());
            TextCommand = new RelayCommand(param => TextMethod());
            LeaveCommand = new RelayCommand(param => LeaveMethod());

            EventAggregator eventAgg = (EventAggregator)ServiceLocator.Current.GetInstance<IEventAggregator>();
            eventAgg.GetEvent<CompositePresentationEvent<SharingData>>().Subscribe(OnCopyDataReceived);
        }

        public void OnCopyDataReceived(SharingData sharingData)
        {
            if (sharingData.ModuleName != Constant.ModuleExplorerType)
                return;

            InitView();
            StartTimer();
        }

        #endregion Constructor & OnCopyDataReceived

        #region Fields

        public RelayCommand VideoCommand { get; set; }

        public RelayCommand MusicCommand { get; set; }

        public RelayCommand ImageCommand { get; set; }

        public RelayCommand TextCommand { get; set; }

        public RelayCommand LeaveCommand { get; set; }

        private int _cpt = 0;

        #endregion Fields

        #region Methods

        private void VideoMethod()
        {
            Constant.StopTimer();
            LoadModule(new SharingData()
            {
                RegionName = Constant.RegionMain,
                ModuleName = Constant.ModuleExplorerFile,
                FileType = Constant.ModuleVideoPlayer,
                FilePage = 0
            });
        }

        private void MusicMethod()
        {
            Constant.StopTimer();
            LoadModule(new SharingData()
            {
                RegionName = Constant.RegionMain,
                ModuleName = Constant.ModuleExplorerFile,
                FileType = Constant.ModuleMusicPlayer,
                FilePage = 0
            });
        }

        private void ImageMethod()
        {
            Constant.StopTimer();
            LoadModule(new SharingData()
            {
                RegionName = Constant.RegionMain,
                ModuleName = Constant.ModuleExplorerFile,
                FileType = Constant.ModuleImageViewer,
                FilePage = 0
            });
        }

        private void TextMethod()
        {
            Constant.StopTimer();
            LoadModule(new SharingData()
            {
                RegionName = Constant.RegionMain,
                ModuleName = Constant.ModuleExplorerFile,
                FileType = Constant.ModuleTextReader,
                FilePage = 0
            });
        }

        private void LeaveMethod()
        {
            Constant.StopTimer();
            Application.Current.Shutdown();
        }

        #endregion Methods

        #region Timer

        private void StartTimer()
        {
            _cpt = 0;

            if (Constant.Timer == null)
            {
                Constant.Timer = new DispatcherTimer();
                Constant.Timer.Interval = new TimeSpan(0, 0, 2);
                Constant.Timer.Tick += new EventHandler(returnTimer_Tick);
            }
            Constant.Timer.Start();

            BasicElement eltTimer = new BasicElement() { PosX = 0, PosY = 0, Height = 0, Width = 0 };
            eltTimer.OnSkeltonPresent += new Common.ManageMove.Element.EventKinectMovement(eltTimer_OnSkeltonPresent);
            eltTimer.OnSkeltonAbsent += new Common.ManageMove.Element.EventKinectMovement(eltTimer_OnSkeltonAbsent);
            Common.ManageMove.ElementRegister.getInstance().registerElement(eltTimer);
        }

        private void returnTimer_Tick(object sender, EventArgs e)
        {
            if (_cpt > Constant.TimeBeforeReturn)
            {
                LoadModule(new SharingData()
                {
                    RegionName = Constant.RegionMain,
                    ModuleName = Constant.ModuleStart
                });
            }
        }

        private void eltTimer_OnSkeltonAbsent()
        {
            _cpt++;
        }

        private void eltTimer_OnSkeltonPresent()
        {
            _cpt = 0;
        }

        #endregion Timer

        #region Kinect Position

        public void InitView()
        {
            ElementRegister.getInstance().reset();

            // Video Button
            BasicElement VideoButton = new BasicElement()
            {
                PosX = Constant.WindowWidth / 2 - 50 - 250,
                PosY = Constant.WindowHeight - 70 - 250,
                Height = 250,
                Width = 250
            };
            VideoButton.OnLeftHandPush += new Element.EventKinectMovement(VideoMethod);
            VideoButton.OnRightHandPush += new Element.EventKinectMovement(VideoMethod);

            ElementRegister.getInstance().registerElement(VideoButton);

            // Music Button
            BasicElement MusicButton = new BasicElement()
            {
                PosX = Constant.WindowWidth / 2 + 50,
                PosY = Constant.WindowHeight - 70 - 250,
                Height = 250,
                Width = 250
            };
            MusicButton.OnLeftHandPush += new Element.EventKinectMovement(MusicMethod);
            MusicButton.OnRightHandPush += new Element.EventKinectMovement(MusicMethod);

            ElementRegister.getInstance().registerElement(MusicButton);

            // Image Button
            BasicElement ImageButton = new BasicElement()
            {
                PosX = Constant.WindowWidth / 2 - 50 - 250,
                PosY = Constant.WindowHeight - 70 - 250 - 80 - 250,
                Height = 250,
                Width = 250
            };
            ImageButton.OnLeftHandPush += new Element.EventKinectMovement(ImageMethod);
            ImageButton.OnRightHandPush += new Element.EventKinectMovement(ImageMethod);

            ElementRegister.getInstance().registerElement(ImageButton);

            // Text Button
            BasicElement TextButton = new BasicElement()
            {
                PosX = Constant.WindowWidth / 2 + 50,
                PosY = Constant.WindowHeight - 70 - 250 - 80 - 250,
                Height = 250,
                Width = 250
            };
            TextButton.OnLeftHandPush += new Element.EventKinectMovement(TextMethod);
            TextButton.OnRightHandPush += new Element.EventKinectMovement(TextMethod);

            ElementRegister.getInstance().registerElement(TextButton);
        }

        #endregion Kinect Position
    }
}