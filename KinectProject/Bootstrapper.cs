using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using System.Windows;

namespace KinectProject
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return new Shell();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            App.Current.MainWindow = (Window)this.Shell;
            App.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;

            moduleCatalog.AddModule(typeof(ModuleStart.ModuleStart));
            moduleCatalog.AddModule(typeof(ModuleExplorerFile.ModuleExplorerFile));
            moduleCatalog.AddModule(typeof(ModuleExplorerType.ModuleExplorerType));
            moduleCatalog.AddModule(typeof(ModuleVideoPlayer.ModuleVideoPlayer));
            moduleCatalog.AddModule(typeof(ModuleMusicPlayer.ModuleMusicPlayer));
            moduleCatalog.AddModule(typeof(ModuleImageViewer.ModuleImageViewer));
            moduleCatalog.AddModule(typeof(ModuleTextReader.ModuleTextReader));
        }
    }
}