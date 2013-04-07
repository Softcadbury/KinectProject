using Common;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleStart
{
    public class ModuleStart : IModule
    {
        public ModuleStart(IRegionManager regionManager, IUnityContainer container)
        {
            _regionManager = regionManager;
            _container = container;
        }

        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _container;

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(Constant.RegionMain, typeof(Views.ModuleStartView));
            _container.RegisterType<Object, Views.ModuleStartView>(Constant.ModuleStart);
        }
    }
}