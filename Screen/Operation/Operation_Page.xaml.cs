using MISOTEN_APPLICATION.BackProcess;
using MISOTEN_APPLICATION.Screen.Calibration;
using MISOTEN_APPLICATION.Screen.CommonClass;
using MISOTEN_APPLICATION.Screen.SystemSelect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Operation_Page.xaml の相互作用ロジック
    /// </summary>
    public partial class Operation_Page : Page
    {
        SignalClass Signalclass = new SignalClass();
        // 排他制御に使用するオブジェクト
        private static Object lockObject = new Object();
        // 処理終了フラグ
        int EndFlog = Flog.Start;
        // スタート内容フラグ
        int StartFlog = Flog.NotStart;
        // ゴッドハンド実体化
        GodHand ggodhand;

        public Operation_Page(SignalClass signalclass , GodHand godhand, int flog)
        {
            InitializeComponent();
            Signalclass = signalclass;
            StartFlog = flog;
            ggodhand = godhand;
        }

        void PageLoad(object sender, RoutedEventArgs e)
        {
            CalibrationButton.IsEnabled = false;
            // センシング用タスク
            Task ReceveTask = Task.Run(() => { ReceveAsync(); });

            if(StartFlog == Flog.MLogON)
            {
                StatusTextBlock.Text = "LOG(マスター)";
            }
            else if (StartFlog == Flog.SLogON)
            {
                StatusTextBlock.Text = "LOG(スレーブ)";
            }
            else if (StartFlog == Flog.RialON)
            {
                StatusTextBlock.Text = "リアルタイム実装";
            }
        }

        /* センシング処理 */
        private async Task ReceveAsync()
        {
            // マスター:"ss01" 送信 : センシング開始信号
            Signalclass.SignalSend(DeviceId.MasterId, SendSignal.MSensingStart);
            // スレーブ:"ss02" 送信 : センシング開始信号
            Signalclass.SignalSend(DeviceId.ReceiveId, SendSignal.SSensingStart);
            
            // 稼働処理
            //int ret = Playing(Signalclass);
            //var task = Task.Run(async () => { await Playing(Signalclass); });
            int result = await Playing(Signalclass);
            
            // マスター:"sh01" 送信 : センシング停止信号
            Signalclass.SignalSend(DeviceId.MasterId, SendSignal.MSensingStop);
            // スレーブ:"sh02" 送信 : センシング停止信号
            Signalclass.SignalSend(DeviceId.ReceiveId, SendSignal.SSensingStop);
            
        }

        /* 稼働時処理 */
        private async Task<int> Playing(SignalClass signalclass)
        {
            Debug.Print("稼働状態start:FlogNo."+ StartFlog);
            while (EndFlog != Flog.End)
            {
                // リアルタイム計測
                if(StartFlog == Flog.RialON)
                {
                    var Reslt = await ggodhand.run(signalclass);
                }
                // マスター計測
                else if(StartFlog == Flog.MLogON)
                {
                    var Reslt = await ggodhand.walk(signalclass);
                }
                // スレーブ計測
                else if (StartFlog == Flog.SLogON)
                {
                    var Reslt = await ggodhand.stand(signalclass);
                }
            };

            return 0;
        }

        /* 終了ボタン処理 */
        private void EndButton_Click(object sender, RoutedEventArgs e)
        {
            // マスター・スレーブ処理タスク終了
            lock (lockObject) EndFlog = Flog.End;

            // システム選択画面へ移行
            var systemselect_page = new SystemSelect_Page(Signalclass, ggodhand);
            NavigationService.Navigate(systemselect_page);
        }
        /* キャリブレーションボタン処理 */
        private void CalibrationButton_Click(object sender, RoutedEventArgs e)
        {
            // マスター・スレーブ処理タスク終了
            lock (lockObject) EndFlog = Flog.End;

            // マスター:"sr01" 送信 : センシングリセット信号
            Signalclass.SignalSend(DeviceId.MasterId, SendSignal.MSensingReset);

            // スレーブ:"sr02" 送信 : センシングリセット信号
            Signalclass.SignalSend(DeviceId.ReceiveId, SendSignal.SSensingReset);

            // キャリブレーション準備画面へ移行
            var calibrationstandby_page = new CalibrationStandby_Page(Signalclass);
            NavigationService.Navigate(calibrationstandby_page);
        }
        // ロゴ画像ボタン
        private void RogoButton_Click(object sender, RoutedEventArgs e)
        {
            // LogFileOpen
            System.Diagnostics.Process.Start("explorer.exe", @"Log");
        }
    }
}
