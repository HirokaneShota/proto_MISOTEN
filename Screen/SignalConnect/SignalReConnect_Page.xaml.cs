using MISOTEN_APPLICATION.Screen.SystemSelect;
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

namespace MISOTEN_APPLICATION.Screen.SignalConnect
{
    /// <summary>
    /// SignalReConnect_Page.xaml の相互作用ロジック
    /// </summary>
    public partial class SignalReConnect_Page : Page
    {
        public SignalReConnect_Page()
        {
            InitializeComponent();
            this.ShowsNavigationUI = false;
        }

        private void ReConnectButton_Click(object sender, RoutedEventArgs e)
        {
            // 再接続
            var SignalConnect = new SignalConnect_Page();
            NavigationService.Navigate(SignalConnect);
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            // 戻る
            var SignalConnectStandby = new SignalConnectStandby_Page();
            NavigationService.Navigate(SignalConnectStandby);
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            // ヘルプ
        }
    }
}
