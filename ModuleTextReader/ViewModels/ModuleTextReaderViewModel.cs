using Common;
using Common.ManageMove;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TextReader;

namespace ModuleTextReader.ViewModels
{
    public class ModuleTextReaderViewModel : BaseViewModel
    {
        #region Constructor & OnCopyDataReceived

        public ModuleTextReaderViewModel()
        {
            BackCommand = new RelayCommand(param => BackMethod());
            OutCommand = new RelayCommand(param => OutMethod());
            InCommand = new RelayCommand(param => InMethod());

            EventAggregator eventAgg = (EventAggregator)ServiceLocator.Current.GetInstance<IEventAggregator>();
            eventAgg.GetEvent<CompositePresentationEvent<SharingData>>().Subscribe(OnCopyDataReceived);
        }

        public void OnCopyDataReceived(SharingData sharingData)
        {
            if (sharingData.ModuleName != Constant.ModuleTextReader)
                return;

            if (!File.Exists(sharingData.FilePath))
                return;

            InitView();

            _filePage = sharingData.FilePage;
            TextName = sharingData.FileName;

            FileInfo fi = new FileInfo(sharingData.FilePath);

            switch (fi.Extension)
            {
                case ".txt":
                    StreamReader sr = new StreamReader(sharingData.FilePath, System.Text.Encoding.Default);
                    FlowDocument fdTxt = new FlowDocument();
                    Paragraph paraHeader = new Paragraph();
                    paraHeader.FontSize = 28;
                    paraHeader.FontWeight = FontWeights.Regular;
                    paraHeader.Inlines.Add(new Run(sr.ReadToEnd()));
                    fdTxt.Blocks.Add(paraHeader);
                    sr.Close();
                    sr.Dispose();
                    Text = fdTxt;
                    break;

                case ".docx":
                    FlowDocument fd = new FlowDocument();
                    WordProcessor.LoadFromWordML(fd, sharingData.FilePath);
                    Text = fd;
                    break;

                default:
                    break;
            }
        }

        #endregion Constructor & OnCopyDataReceived

        #region Fields

        public RelayCommand BackCommand { get; set; }

        public RelayCommand OutCommand { get; set; }

        public RelayCommand InCommand { get; set; }

        private int _filePage;

        public FlowDocumentScrollViewer _flowDoc;

        private int _flowIndex = 0;

        private string _textName;

        public string TextName
        {
            get { return _textName; }
            set { _textName = value; NotifyPropertyChanged("TextName"); }
        }

        private FlowDocument _text;

        public FlowDocument Text
        {
            get { return _text; }
            set { _text = value; NotifyPropertyChanged("Text"); }
        }

        #endregion Fields

        #region Command Methods

        public void BackMethod()
        {
            LoadModule(new SharingData()
            {
                RegionName = Constant.RegionMain,
                ModuleName = Constant.ModuleExplorerFile,
                FileType = Constant.ModuleTextReader,
                FilePage = _filePage
            });
        }

        public void OutMethod()
        {
            _flowIndex += 100;
            ScrollViewer sv = GetChild<ScrollViewer>(_flowDoc);
            if (sv != null)
                sv.ScrollToVerticalOffset(_flowIndex);
        }

        public void InMethod()
        {
            if (_flowIndex - 100 >= 0) _flowIndex -= 100;
            ScrollViewer sv = GetChild<ScrollViewer>(_flowDoc);
            if (sv != null)
                sv.ScrollToVerticalOffset(_flowIndex);
        }

        public T GetChild<T>(DependencyObject parent) where T : DependencyObject
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T) return (T)child;
                T childLabel = GetChild<T>(child);
                if (childLabel != null) return childLabel;
            }
            return default(T);
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

            // Option 2 Button
            BasicElement Option2Button = new BasicElement()
            {
                PosX = Constant.WindowWidth - 20 - 220 - 20 - 220,
                PosY = 20,
                Height = 220,
                Width = 220
            };
            Option2Button.OnLeftHandPush += new Element.EventKinectMovement(OutMethod);
            Option2Button.OnRightHandPush += new Element.EventKinectMovement(OutMethod);

            ElementRegister.getInstance().registerElement(Option2Button);

            // Option 1 Button
            BasicElement Option3Button = new BasicElement()
            {
                PosX = Constant.WindowWidth - 20 - 220,
                PosY = 20,
                Height = 220,
                Width = 220
            };
            Option3Button.OnLeftHandPush += new Element.EventKinectMovement(InMethod);
            Option3Button.OnRightHandPush += new Element.EventKinectMovement(InMethod);

            ElementRegister.getInstance().registerElement(Option3Button);
        }

        #endregion Kinect Position
    }
}