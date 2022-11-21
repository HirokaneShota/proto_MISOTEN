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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MISOTEN_APPLICATION.Screen.SystemSetting
{
    /// <summary>
    /// SystemSetting_Window.xaml の相互作用ロジック
    /// </summary>
    public partial class SystemSetting_Window : Window
    {
        public SystemSetting_Window()//Window owner
        {
            InitializeComponent();
            //通信接続待機(Com Setting)ページ表示
            Uri ComSettingUri = new Uri("ComSetting_Page.xaml", UriKind.Relative);
            SystemSettingFrame.Source = ComSettingUri;
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
