using Common;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleImageViewer
{
    public class ModuleImageViewer : IModule
    {
        public ModuleImageViewer(IRegionManager regionManager, IUnityContainer container)
        {
            _regionManager = regionManager;
            _container = container;
        }

        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _container;

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(Constant.RegionMain, typeof(Views.ModuleImageViewerView));
            _container.RegisterType<Object, Views.ModuleImageViewerView>(Constant.ModuleImageViewer);
        }
    }
}