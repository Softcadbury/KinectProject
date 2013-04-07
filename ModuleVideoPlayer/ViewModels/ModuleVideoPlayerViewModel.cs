using Common;
using Common.ManageMove;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ModuleVideoPlayer.ViewModels
{
    internal class ModuleVideoPlayerViewModel : BaseViewModel
    {
        #region Constructor & OnCopyDataReceived

        public ModuleVideoPlayerViewModel()
        {
            BackCommand = new RelayCommand(param => BackMethod());
            PlayCommand = new RelayCommand(param => PlayMethod());
            MoveBackwardCommand = new RelayCommand(param => MoveBackwardMethod());
            MoveForwardCommand = new RelayCommand(param => MoveForwardMethod());

            EventAggregator eventAgg = (EventAggregator)ServiceLocator.Current.GetInstance<IEventAggregator>();
            eventAgg.GetEvent<CompositePresentationEvent<SharingData>>().Subscribe(OnCopyDataReceived);
        }

        public void OnCopyDataReceived(SharingData sharingData)
        {
            if (sharingData.ModuleName != Constant.ModuleVideoPlayer)
                return;

            if (!File.Exists(sharingData.FilePath))
                return;

            InitView();

            _isPlaying = true;
            _filePage = sharingData.FilePage;
            VideoName = sharingData.FileName;
            PlayImage = "Media-Pause-Video";

            try
            {
                Video.Stop();
                Video.Close();
                Video.Source = new Uri(sharingData.FilePath);
                Video.Play();
                Video.MediaOpened += new System.Windows.RoutedEventHandler(MovieMediaOpened);
            }
            catch { }
        }

        private void MovieMediaOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Video.NaturalDuration.HasTimeSpan)
            {
                var t = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
                Slider.Minimum = 0;
                Slider.TickFrequency = 1;
                Slider.Maximum = Video.NaturalDuration.TimeSpan.TotalSeconds;
                t.Tick += (obj, eve) => Slider.Value = Video.Position.TotalSeconds;
                t.Start();
            }
        }

        #endregion Constructor & OnCopyDataReceived

        #region Fields

        public RelayCommand BackCommand { get; set; }

        public RelayCommand PlayCommand { get; set; }

        public RelayCommand MoveBackwardCommand { get; set; }

        public RelayCommand MoveForwardCommand { get; set; }

        private int _filePage;

        public MediaElement Video { get; set; }

        public Slider Slider { get; set; }

        private string _videoName;

        public string VideoName
        {
            get { return _videoName; }
            set { _videoName = value; NotifyPropertyChanged("VideoName"); }
        }

        private bool _isPlaying;

        private string _playImage;

        public string PlayImage
        {
            get { return _playImage; }
            set
            {
                _playImage = @"/KinectProject;component/Ressources/" + value + ".png";
                NotifyPropertyChanged("PlayImage");
            }
        }

        #endregion Fields

        #region Command Methods

        public void BackMethod()
        {
            LoadModule(new SharingData()
            {
                RegionName = Constant.RegionMain,
                ModuleName = Constant.ModuleExplorerFile,
                FileType = Constant.ModuleVideoPlayer,
                FilePage = _filePage
            });
        }

        public void PlayMethod()
        {
            if (_isPlaying)
            {
                Video.Pause();
                _isPlaying = false;
                PlayImage = "Media-Play-Video";
            }
            else
            {
                Video.Play();
                _isPlaying = true;
                PlayImage = "Media-Pause-Video";
            }
        }

        public void MoveBackwardMethod()
        {
            Video.Position = Video.Position - TimeSpan.FromSeconds(30);
        }

        public void MoveForwardMethod()
        {
            Video.Position = Video.Position + TimeSpan.FromSeconds(30);
        }

        #endregion Command Methods

        #region Kinect Position

        public void InitView()
        {
            ElementRegister.getInstance().reset();

            // Back Button
            BasicElement BackButton = new BasicElement()
            {
                PosX = 20,
                PosY = 20,
                Height = 220,
                Width = 220
            };
            BackButton.OnLeftHandPush += new Element.EventKinectMovement(BackMethod);
            BackButton.OnRightHandPush += new Element.EventKinectMovement(BackMethod);

            ElementRegister.getInstance().registerElement(BackButton);

            // Option 1 Button
            BasicElement Option1Button = new BasicElement()
            {
                PosX = Constant.WindowWidth - 20 - 220 - 20 - 220 - 20 - 220,
                PosY = 20,
                Height = 220,
                Width = 220
            };
            Option1Button.OnLeftHandPush += new Element.EventKinectMovement(PlayMethod);
            Option1Button.OnRightHandPush += new Element.EventKinectMovement(PlayMethod);

            ElementRegister.getInstance().registerElement(Option1Button);

            // Option 2 Button
            BasicElement Option2Button = new BasicElement()
            {
                PosX = Constant.WindowWidth - 20 - 220 - 20 - 220,
                PosY = 20,
                Height = 220,
                Width = 220
            };
            Option2Button.OnLeftHandPush += new Element.EventKinectMovement(MoveBackwardMethod);
            Option2Button.OnRightHandPush += new Element.EventKinectMovement(MoveBackwardMethod);

            ElementRegister.getInstance().registerElement(Option2Button);

            // Option 1 Button
            BasicElement Option3Button = new BasicElement()
            {
                PosX = Constant.WindowWidth - 20 - 220,
                PosY = 20,
                Height = 220,
                Width = 220
            };
            Option3Button.OnLeftHandPush += new Element.EventKinectMovement(MoveForwardMethod);
            Option3Button.OnRightHandPush += new Element.EventKinectMovement(MoveForwardMethod);

            ElementRegister.getInstance().registerElement(Option3Button);
        }

        #endregion Kinect Position
    }
}