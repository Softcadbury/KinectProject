using Common;
using Common.ManageMove;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using ModuleExplorerFile.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleExplorerFile.ViewModels
{
    internal class ModuleExplorerFileViewModel : BaseViewModel
    {
        #region Constructor & OnCopyDataReceived

        public ModuleExplorerFileViewModel()
        {
            MaxWidth = Constant.WindowWidth;

            EventAggregator eventAgg = (EventAggregator)ServiceLocator.Current.GetInstance<IEventAggregator>();
            eventAgg.GetEvent<CompositePresentationEvent<SharingData>>().Subscribe(OnCopyDataReceived, ThreadOption.UIThread);
        }

        public void OnCopyDataReceived(SharingData sharingData)
        {
            if (sharingData.ModuleName != Constant.ModuleExplorerFile)
                return;

            Info = string.Empty;
            _actualFileType = sharingData.FileType;
            _filePage = sharingData.FilePage;
            Items = new ObservableCollection<ItemModel>();
            Load();
        }

        #endregion Constructor & OnCopyDataReceived

        #region Fields

        private string _actualFileType;

        private int _filePage;

        private ObservableCollection<ItemModel> _items;

        public ObservableCollection<ItemModel> Items
        {
            get { return _items; }
            set { _items = value; NotifyPropertyChanged("Items"); }
        }

        private String _title;

        public String Title
        {
            get { return _title; }
            set { _title = value; NotifyPropertyChanged("Title"); }
        }

        private String _info;

        public String Info
        {
            get { return _info; }
            set { _info = value; NotifyPropertyChanged("Info"); }
        }

        private int _maxWidth;

        public int MaxWidth
        {
            get { return _maxWidth; }
            set { _maxWidth = value; NotifyPropertyChanged("MaxWidth"); }
        }

        #endregion Fields

        #region Methods

        private void Load()
        {
            string path = "./Medias/";
            DirectoryInfo dir = new DirectoryInfo(path);

            switch (_actualFileType)
            {
                case Constant.ModuleVideoPlayer:
                    dir = new DirectoryInfo(path + "Video");
                    Title = "Mes Vidéos";
                    break;

                case Constant.ModuleMusicPlayer:
                    dir = new DirectoryInfo(path + "Music");
                    Title = "Mes Musiques";
                    break;

                case Constant.ModuleImageViewer:
                    dir = new DirectoryInfo(path + "Image");
                    Title = "Mes Images";
                    break;

                case Constant.ModuleTextReader:
                    dir = new DirectoryInfo(path + "Text");
                    Title = "Mes Documents";
                    break;
            }

            Items.Clear();
            ItemModel tmp;

            if (_filePage == 0)
            {
                tmp = new ItemModel();
                tmp.SeItemModelBack(this);
                Items.Add(tmp);
            }
            else
            {
                tmp = new ItemModel();
                tmp.SetItemModelLess(this);
                Items.Add(tmp);
            }

            int cpt = 0;
            foreach (FileSystemInfo file in dir.GetFileSystemInfos())
            {
                cpt++;

                if (cpt > _filePage * Constant.GetMaxFileNumber() && cpt <= (_filePage + 1) * Constant.GetMaxFileNumber())
                {
                    tmp = new ItemModel();
                    tmp.SetItemModelModule(this, file, _actualFileType);
                    Items.Add(tmp);
                }

                if (cpt > (_filePage + 1) * Constant.GetMaxFileNumber()) break;
            }

            if (cpt != dir.GetFileSystemInfos().Length && dir.GetFileSystemInfos().Length > Constant.GetMaxFileNumber())
            {
                tmp = new ItemModel();
                tmp.SetItemModelMore(this);
                Items.Add(tmp);
            }

            InitView();
        }

        internal void BackMethod()
        {
            LoadModule(new SharingData()
            {
                RegionName = Constant.RegionMain,
                ModuleName = Constant.ModuleExplorerType,
                FileType = Constant.ModuleImageViewer
            });
        }

        internal void MoreMethod()
        {
            _filePage++;
            Load();
        }

        internal void LessMethod()
        {
            if (_filePage != 0)
            {
                _filePage--;
                Load();
            }
        }

        internal void ModuleMethod(string FullName, string Name)
        {
            LoadModule(new SharingData()
            {
                RegionName = Constant.RegionMain,
                ModuleName = _actualFileType,
                FilePath = FullName,
                FileName = Name,
                FilePage = _filePage
            });
        }

        #endregion Methods

        #region Kinect Position

        public void InitView()
        {
            ElementRegister.getInstance().reset();

            int itemIndex = 0;
            int itemLine = 0;
            int itemByLine = Constant.GetElementMaxByLine();
            if (_items.Count < itemByLine)
                itemByLine = _items.Count;

            foreach (ItemModel item in _items)
            {
                BasicElement ItemButton = new BasicElement()
                {
                    Height = 240,
                    Width = 210
                };

                if (itemIndex % itemByLine == 0)
                {
                    ++itemLine;
                    itemIndex = 0;
                }

                ItemButton.PosY = Constant.WindowHeight - (70 + (itemLine) * 250);
                ItemButton.PosX = Constant.WindowWidth / 2 - (itemByLine / 2) * 210 + itemIndex * 210;
                ++itemIndex;

                ItemButton.OnLeftHandPush += new Element.EventKinectMovement(item.ItemMethod);
                ItemButton.OnRightHandPush += new Element.EventKinectMovement(item.ItemMethod);
                ItemButton.OnRightHandOver += new Element.EventKinectMovement(item.UpdateInfo);

                ElementRegister.getInstance().registerElement(ItemButton);
            }
        }

        #endregion Kinect Position
    }
}