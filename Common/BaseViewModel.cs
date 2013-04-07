using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Class to help viewmodels implementation
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Method to notified the changement of a propertie in a view
        /// </summary>
        protected virtual void NotifyPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Load the defined module in the defined region
        /// </summary>
        protected virtual void LoadModule(SharingData sharingData)
        {
            EventAggregator eventAgg = (EventAggregator)ServiceLocator.Current.GetInstance<IEventAggregator>();
            CompositePresentationEvent<SharingData> fileSharingEvent = eventAgg.GetEvent<CompositePresentationEvent<SharingData>>();
            fileSharingEvent.Publish(sharingData);

            var regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
            regionManager.RequestNavigate(sharingData.RegionName, new Uri(sharingData.ModuleName, UriKind.Relative));
        }
    }
}