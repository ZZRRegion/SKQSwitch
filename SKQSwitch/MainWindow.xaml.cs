﻿using SKQSwitch.Utils;
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
        private GlobalKeyboardHook _keyboardHook;
        private Queue<Key> keys = new();
        public MainWindow()
        {
            InitializeComponent();
            _keyboardHook = new GlobalKeyboardHook();
            _keyboardHook.OnKeyPressed += _keyboardHook_OnKeyPressed;
        }

        private void _keyboardHook_OnKeyPressed(object? sender, GlobalKeyboardHook.KeyPressedEventArgs e)
        {
            this.keys.Enqueue(e.Key);
            if(this.keys.Count > 5)
            {
                this.keys.Dequeue();
            }
            StringBuilder sb = new();
            foreach(var key in this.keys)
            {
                sb.Append(key.ToString());
            }
            this.viewModel.KeyName = sb.ToString();
            Debug.WriteLine($"key:{sb}");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utils.LogUtil.AddInfoInsertDateTime("软件启动");
            this.ShowPosition();
            _keyboardHook.Start();
        }
        private void ShowPosition()
        {
            Rect workArea = SystemParameters.WorkArea;
            this.Top = workArea.Height - this.ActualHeight;
            this.Left = workArea.Width - this.ActualWidth;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _keyboardHook.Stop();
            Utils.LogUtil.AddInfoInsertDateTime("软件退出");
        }
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if(e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
            base.OnMouseMove(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("确定退出？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
    }
}