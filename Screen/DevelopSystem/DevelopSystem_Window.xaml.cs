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

namespace MISOTEN_APPLICATION.Screen.DevelopSystem
{
    /// <summary>
    /// DevelopSystem_Window.xaml の相互作用ロジック
    /// </summary>
    public partial class DevelopSystem_Window : Window
    {
        public DevelopSystem_Window()
        {
            InitializeComponent();
            //通信接続待機(Signal Connect Standby)ページ表示
            Uri DevelopSystemUri = new Uri("DevelopConnectSystem_Page.xaml", UriKind.Relative);
            DevelopSystemFrame.Source = DevelopSystemUri;
        }
    }
}
