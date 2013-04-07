using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModuleVideoPlayer.Views
{
    /// <summary>
    /// Interaction logic for ModuleVideoPlayerView.xaml
    /// </summary>
    public partial class ModuleVideoPlayerView : UserControl
    {
        public ModuleVideoPlayerView()
        {
            InitializeComponent();
            DataContext = new ViewModels.ModuleVideoPlayerViewModel()
            {
                Video = this.Video,
                Slider = this.Slider
            };
        }
    }
}