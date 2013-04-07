using Common;
using ModuleExplorerFile.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleExplorerFile.Models
{
    internal class ItemModel
    {
        #region Constructor

        public void SeItemModelBack(ModuleExplorerFileViewModel obj)
        {
            Obj = obj;
            IconName = "/KinectProject;component/Ressources/Logout.png";
            Name = "Retour";
            ItemsCommand = new RelayCommand(param => Obj.BackMethod());
        }

        public void SetItemModelMore(ModuleExplorerFileViewModel obj)
        {
            Obj = obj;
            IconName = "/KinectProject;component/Ressources/Text Edit.png";
            Name = "Plus";
            ItemsCommand = new RelayCommand(param => Obj.MoreMethod());
        }

        public void SetItemModelLess(ModuleExplorerFileViewModel obj)
        {
            Obj = obj;
            IconName = "/KinectProject;component/Ressources/ArrowHead-Left.png";
            Name = "Moins";
            ItemsCommand = new RelayCommand(param => Obj.LessMethod());
        }

        public void SetItemModelModule(ModuleExplorerFileViewModel obj, FileSystemInfo file, string fileType)
        {
            Obj = obj;
            ItemsCommand = new RelayCommand(param => Obj.ModuleMethod(FullName, Name));

            if (file != null)
            {
                Name = file.Name.Split('.')[0];
                FullName = file.FullName;
            }

            switch (fileType)
            {
                case Constant.ModuleVideoPlayer:
                    IconName = "/KinectProject;component/Ressources/Video Camera.png";
                    break;

                case Constant.ModuleMusicPlayer:
                    IconName = "/KinectProject;component/Ressources/Head Phone.png";
                    break;

                case Constant.ModuleImageViewer:
                    IconName = "/KinectProject;component/Ressources/Camera.png";
                    break;

                case Constant.ModuleTextReader:
                    IconName = "/KinectProject;component/Ressources/Document.png";
                    break;
            }
        }

        #endregion Constructor

        #region Fields

        private ModuleExplorerFileViewModel Obj;

        public RelayCommand ItemsCommand { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public string IconName { get; set; }

        #endregion Fields

        #region Methods

        public void UpdateInfo()
        {
            if (!string.IsNullOrEmpty(FullName))
            {
                String[] spliter = FullName.Split('\\');
                string info = "Nom du fichier: " + spliter[spliter.Length - 1];
                Obj.Info = info;
            }
            else
                Obj.Info = string.Empty;
        }

        public void ItemMethod()
        {
            ItemsCommand.Execute(new Object());
        }

        #endregion Methods
    }
}