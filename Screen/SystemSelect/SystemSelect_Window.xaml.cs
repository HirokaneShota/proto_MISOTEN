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
using System.Runtime.InteropServices;
using System.Windows.Interop;
using MISOTEN_APPLICATION.Screen.DevelopSystem;
using MISOTEN_APPLICATION.Screen.SystemSetting;

namespace MISOTEN_APPLICATION.Screen.SystemSelect
{
    /// <summary>
    /// SystemSelect_Window.xaml の相互作用ロジック
    /// </summary>
    public partial class SystemSelect_Window : Window
    {

        public SystemSelect_Window()
        {
            InitializeComponent();
            //通信接続待機(Signal Connect Standby)ページ表示
            Uri SystemSelectUri = new Uri("SignalConnectStandby_Page.xaml", UriKind.Relative);
            SystemSelectFrame.Source = SystemSelectUri;
        }

        private void DevelopSystemButton_Click(object sender, RoutedEventArgs e)
        {
            // 子画面を生成します。
            DevelopSystem_Window window = new DevelopSystem_Window();

            // 子画面を表示します。
            window.ShowDialog();

            // 子画面のオーナープロパティにこの画面を設定します。
            //window.Owner = GetWindow(this);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 子画面を生成します。
            SystemSetting_Window window = new SystemSetting_Window();

            // 子画面を表示します。
            window.ShowDialog();

            //var SettingWindow = new SystemSetting_Window(this);

            // return ((bool)SettingWindow.ShowDialog()) ? SettingWindow.Text : str;
        }
    }
}
