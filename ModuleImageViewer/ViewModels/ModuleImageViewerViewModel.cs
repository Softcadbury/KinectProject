using Common;
using Common.ManageMove;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModuleImageViewer.ViewModels
{
    public class ModuleImageViewerViewModel : BaseViewModel
    {
        #region Constructor & OnCopyDataReceived

        public ModuleImageViewerViewModel()
        {
            BackCommand = new RelayCommand(param => BackMethod());
            ZoomOutCommand = new RelayCommand(param => ZoomOutMethod());
            ZoomInCommand = new RelayCommand(param => ZoomInMethod());

            EventAggregator eventAgg = (EventAggregator)ServiceLocator.Current.GetInstance<IEventAggregator>();
            eventAgg.GetEvent<CompositePresentationEvent<SharingData>>().Subscribe(OnCopyDataReceived, ThreadOption.UIThread);
        }

        public void OnCopyDataReceived(SharingData sharingData)
        {
            if (sharingData.ModuleName != Constant.ModuleImageViewer)
                return;

            if (!File.Exists(sharingData.FilePath))
                return;

            InitView();

            _filePage = sharingData.FilePage;
            ImageName = sharingData.FileName;

            BitmapImage _tmp = new BitmapImage();
            _tmp.BeginInit();
            _tmp.CacheOption = BitmapCacheOption.None;
            _tmp.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            _tmp.CacheOption = BitmapCacheOption.OnLoad;
            _tmp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            _tmp.UriSource = new Uri(sharingData.FilePath, UriKind.RelativeOrAbsolute);
            _tmp.EndInit();
            Image.Source = _tmp;

            ScaleTransform scale = new ScaleTransform();
            scale.ScaleX = 1;
            scale.ScaleY = 1;
            Image.RenderTransform = scale;
        }

        #endregion Constructor & OnCopyDataReceived

        #region Fields

        public RelayCommand BackCommand { get; set; }

        public RelayCommand ZoomOutCommand { get; set; }

        public RelayCommand ZoomInCommand { get; set; }

        private int _filePage;

        public Image Image;

        private string _imageName;

        public string ImageName
        {
            get { return _imageName; }
            set { _imageName = value; NotifyPropertyChanged("ImageName"); }
        }

        #endregion Fields

        #region Methods

        public void BackMethod()
        {
            LoadModule(new SharingData()
            {
                RegionName = Constant.RegionMain,
                ModuleName = Constant.ModuleExplorerFile,
                FileType = Constant.ModuleImageViewer,
                FilePage = _filePage
            });
        }

        public void ZoomOutMethod()
        {
            var st = (ScaleTransform)Image.RenderTransform;
            double zoom = 0.2;
            if (st.ScaleX - zoom > 0)
            {
                st.ScaleX -= zoom;
                st.ScaleY -= zoom;
            }
        }

        public void ZoomInMethod()
        {
            var st = (ScaleTransform)Image.RenderTransform;
            double zoom = 0.2;
            if (st.ScaleX + zoom < 2)
            {
                st.ScaleX += zoom;
                st.ScaleY += zoom;
            }
        }

        #endregion Methods

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

            // Option 2 Button
            BasicElement Option2Button = new BasicElement()
            {
                PosX = Constant.WindowWidth - 20 - 220 - 20 - 220,
                PosY = 20,
                Height = 220,
                Width = 220
            };
            Option2Button.OnLeftHandPush += new Element.EventKinectMovement(ZoomInMethod);
            Option2Button.OnRightHandPush += new Element.EventKinectMovement(ZoomInMethod);

            ElementRegister.getInstance().registerElement(Option2Button);

            // Option 1 Button
            BasicElement Option3Button = new BasicElement()
            {
                PosX = Constant.WindowWidth - 20 - 220,
                PosY = 20,
                Height = 220,
                Width = 220
            };
            Option3Button.OnLeftHandPush += new Element.EventKinectMovement(ZoomOutMethod);
            Option3Button.OnRightHandPush += new Element.EventKinectMovement(ZoomOutMethod);

            ElementRegister.getInstance().registerElement(Option3Button);
        }

        #endregion Kinect Position
    }
}