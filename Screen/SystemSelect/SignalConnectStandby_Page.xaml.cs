using MISOTEN_APPLICATION.Screen.DevelopSystem;
using MISOTEN_APPLICATION.Screen.SignalConnect;
using MISOTEN_APPLICATION.Screen.SystemSetting;
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

namespace MISOTEN_APPLICATION.Screen.SystemSelect
{
    /// <summary>
    /// SignalConnectStandby_Page.xaml の相互作用ロジック
    /// </summary>
    public partial class SignalConnectStandby_Page : Page
    {
        public SignalConnectStandby_Page()
        {
            InitializeComponent();
        }

        /* 機体との接続開始 */
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            SignalConnect_Page signalconnect_page = new SignalConnect_Page();
            NavigationService.Navigate(signalconnect_page);
        }

        /* 開発用システム */
        private void DevelopSystemButton_Click(object sender, RoutedEventArgs e)
        {
            // 子画面を生成します。
            DevelopSystem_Window window = new DevelopSystem_Window();
            window.ShowDialog();
        }

        /* Com設定 */
        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            // 子画面を生成します。
            SystemSetting_Window window = new SystemSetting_Window();
            window.ShowDialog();
        }
    }
}
