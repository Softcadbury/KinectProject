using Common;
using Common.ManageMove;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModuleStart.ViewModels
{
    public class ModuleStartViewModel : BaseViewModel
    {
        #region Constructor & OnCopyDataReceived

        public ModuleStartViewModel()
        {
            InitView();

            LeaveCommand = new RelayCommand(param => LeaveMethod());
            GoCommand = new RelayCommand(param => GoMethod());

            EventAggregator eventAgg = (EventAggregator)ServiceLocator.Current.GetInstance<IEventAggregator>();
            eventAgg.GetEvent<CompositePresentationEvent<SharingData>>().Subscribe(OnCopyDataReceived);
        }

        public void OnCopyDataReceived(SharingData sharingData)
        {
            if (sharingData.ModuleName != Constant.ModuleStart)
                return;

            InitView();
        }

        #endregion Constructor & OnCopyDataReceived

        #region Fields

        public RelayCommand LeaveCommand { get; set; }

        public RelayCommand GoCommand { get; set; }

        #endregion Fields

        #region Methods

        private void LeaveMethod()
        {
            Constant.StopTimer();
            Application.Current.Shutdown();
        }

        private void GoMethod()
        {
            LoadModule(new SharingData()
            {
                RegionName = Constant.RegionMain,
                ModuleName = Constant.ModuleExplorerType
            });
        }

        #endregion Methods

        #region Kinect Position

        public void InitView()
        {
            ElementRegister.getInstance().reset();

            // Go Button
            BasicElement GoButton = new BasicElement()
            {
                PosX = Constant.WindowWidth - 10 - 250,
                PosY = 10,
                Height = 250,
                Width = 250
            };
            GoButton.OnLeftHandPush += new Element.EventKinectMovement(GoMethod);
            GoButton.OnRightHandPush += new Element.EventKinectMovement(GoMethod);

            ElementRegister.getInstance().registerElement(GoButton);
        }

        #endregion Kinect Position
    }
}