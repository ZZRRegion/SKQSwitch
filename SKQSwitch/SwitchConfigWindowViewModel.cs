using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SKQSwitch.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SKQSwitch
{
    internal partial class SwitchConfigWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<SwitchConfig> switchs = new();
        [ObservableProperty]
        private string? exeName;
        [ObservableProperty]
        private int time = 3;
        [RelayCommand]
        private void Add()
        {
            if(string.IsNullOrWhiteSpace(ExeName))
            {
                MessageBox.Show("请输入要添加的软件名称！");
                return;
            }
            if(this.Time <= 0)
            {
                MessageBox.Show("请输入显示时间！");
                return;
            }
            foreach(SwitchConfig config in SwitchConfig.SwitchConfigs)
            {
                if(config.ExeName == ExeName)
                {
                    MessageBox.Show($"当前已配置该软件:{ExeName}");
                    return;
                }
            }
            if (MessageBox.Show("确认添加？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SwitchConfig.Add(ExeName, Time);
                this.Update();
            }
        }
        [RelayCommand]
        private void Remove()
        {
            if(string.IsNullOrWhiteSpace(ExeName))
            {
                MessageBox.Show("请输入要删除的软件名称！");
                return;
            }
            if (MessageBox.Show("确认删除？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                SwitchConfig.Remove(ExeName);
                this.Update();
            }
        }
        public void UpdateConfig(SwitchConfig switchConfig)
        {
            this.ExeName = switchConfig.ExeName;
            this.Time = switchConfig.Time;
        }
        public SwitchConfigWindowViewModel()
        {
            this.Update();
        }
        private void Update()
        {
            this.Switchs.Clear();
            foreach(SwitchConfig config in SwitchConfig.SwitchConfigs)
            {
                this.Switchs.Add(config);
            }
        }
    }
}
