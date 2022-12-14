using MISOTEN_APPLICATION.BackProcess;
using MISOTEN_APPLICATION.Screen.Calibration;
using MISOTEN_APPLICATION.Screen.CommonClass;
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

        public Operation_Page(SignalClass signalclass)
        {
            InitializeComponent();
            Signalclass = signalclass;
        }

        void PageLoad(object sender, RoutedEventArgs e)
        {
            // 送信タスク
            Task ReceveTask = Task.Run(() => { Receve(); });

        }

        /* マスター値受信処理 */
        private void Receve()
        {
            // マスター:"ss01" 送信 : センシング開始信号
            Signalclass.SignalSend(DeviceId.MasterId, SendSignal.MSensingStart);
            // スレーブ:"ss02" 送信 : センシング開始信号
            Signalclass.SignalSend(DeviceId.ReceiveId, SendSignal.SSensingStart);

            // 稼働処理タスク
            Task<int> PlayingTask = Task.Run(() => { return Playing(Signalclass); });
            int i = 0;
            i = PlayingTask.Result;

            // 処理終了フラグが立つまで
            while (EndFlog != Flog.End);

            // マスター:"sh01" 送信 : センシング停止信号
            Signalclass.SignalSend(DeviceId.MasterId, SendSignal.MSensingStop);
            // レシーブ:"sh02" 送信 : センシング停止信号
            Signalclass.SignalSend(DeviceId.ReceiveId, SendSignal.SSensingStop);

        }

        /* 稼働時処理 */
        private int Playing(SignalClass signalclass)
        {
            GodHand godhand = new GodHand();

            var reslt = godhand.Threshold_monitoring(signalclass);

            while (EndFlog != Flog.End)
            {
                var Reslt = godhand.run(signalclass);
            };

            return 0;
        }

        /* 終了ボタン処理 */
        private void EndButton_Click(object sender, RoutedEventArgs e)
        {

            // マスター・スレーブ処理タスク終了
            lock (lockObject) EndFlog = Flog.End;

            // システム選択画面へ移行
            var systemselect_page = new SystemSelect_Page(Signalclass);
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
    }
}
