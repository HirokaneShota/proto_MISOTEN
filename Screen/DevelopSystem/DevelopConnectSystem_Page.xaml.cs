using System;
using System.Collections.Generic;
using System.IO.Ports;
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

namespace MISOTEN_APPLICATION.Screen.DevelopSystem
{
    /// <summary>
    /// DevelopConnectSystem_Page.xaml の相互作用ロジック
    /// </summary>
    public partial class DevelopConnectSystem_Page : Page
    {
        public DevelopConnectSystem_Page()
        {
            InitializeComponent();
            ReceiveText.IsReadOnly = true;
        }

        private void OpenLogFolderButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", @"Log");
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if((bool)MasterConnectCheck.IsChecked == true)
            {
                // シリアルポートに接続
                try
                {
                    // ポートオープン
                    //serialPort.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if ((bool)ReceiveConnectCheck.IsChecked == true)
            {
                // シリアルポートに接続
                try
                {
                    // ポートオープン
                    //serialPort.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


    }
}
