using MISOTEN_APPLICATION.BackProcess;
using MISOTEN_APPLICATION.Screen.Calibration;
using MISOTEN_APPLICATION.Screen.CommonClass;
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

namespace MISOTEN_APPLICATION.Screen.Operation
{
    /// <summary>
    /// OperationStandby_Page.xaml の相互作用ロジック
    /// </summary>
    public partial class OperationStandby_Page : Page
    {
        // 信号クラス実体化
        SignalClass Signalclass = new SignalClass();
        // ゴッドハンド実体化
        GodHand ggodhand ;
        // スタート内容フラグ
        int StartFlog = 0;

        public OperationStandby_Page(SignalClass signalclass, GodHand godhand,int flog)
        {
            InitializeComponent();
            Signalclass = signalclass;
            ggodhand = godhand;
            StartFlog = flog;
        }
        void PageLoad(object sender, RoutedEventArgs e)
        {
            if(StartFlog == Flog.MLogON || StartFlog == Flog.SLogON)
            {
                CountLabel.Content = "手を広げた状態で開始ボタンを押してください";
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // 稼働ページへ移行
            var operation_page = new Operation_Page(Signalclass, ggodhand, StartFlog);
            NavigationService.Navigate(operation_page);
        }
    }
}
