using SKQSwitch.Utils;
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
using System.Windows.Shapes;

namespace SKQSwitch
{
    /// <summary>
    /// SwitchConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SwitchConfigWindow : Window
    {
        public SwitchConfigWindow()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox? listBox = sender as ListBox;
            if(listBox != null)
            {
                SwitchConfig? switchConfig = listBox.SelectedItem as SwitchConfig;
                if(switchConfig != null)
                {
                    this.viewModel.UpdateConfig(switchConfig);
                }
            }
        }
    }
}
