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

namespace ModuleTextReader.Views
{
    /// <summary>
    /// Interaction logic for ModuleTextReaderView.xaml
    /// </summary>
    public partial class ModuleTextReaderView : UserControl
    {
        public ModuleTextReaderView()
        {
            InitializeComponent();
            DataContext = new ViewModels.ModuleTextReaderViewModel()
            {
                _flowDoc = FlowDoc
            };
        }
    }
}