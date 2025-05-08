using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Vanara.PInvoke;

namespace SKQSwitch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utils.LogUtil.AddInfoInsertDateTime("软件启动");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Utils.LogUtil.AddInfoInsertDateTime("软件退出");
        }
    }
}