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

namespace ModuleImageViewer.Views
{
    /// <summary>
    /// Interaction logic for ModuleImageViewerView.xaml
    /// </summary>
    public partial class ModuleImageViewerView : UserControl
    {
        public ModuleImageViewerView()
        {
            InitializeComponent();
            DataContext = new ViewModels.ModuleImageViewerViewModel()
            {
                Image = this.Image
            };
        }
    }
}