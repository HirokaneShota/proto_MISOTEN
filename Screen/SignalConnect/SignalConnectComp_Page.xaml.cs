using MISOTEN_APPLICATION.Screen.Calibration;
using MISOTEN_APPLICATION.Screen.CommonClass;
using MISOTEN_APPLICATION.BackProcess;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using MISOTEN_APPLICATION.Screen.Operation;

namespace MISOTEN_APPLICATION.Screen.SignalConnect
{
    /// <summary>
    /// SignalConnectComp_Page.xaml の相互作用ロジック
    /// </summary>
    public partial class SignalConnectComp_Page : Page
    {
        public SignalConnectComp_Page(SignalClass signalclass)
        {
            InitializeComponent();
            // 時間計測タスク
            Task MeasurementTask = Task.Run(() => { Measurement(signalclass); });
        }

        /* 時間計測タスク */
        private void Measurement(SignalClass signalclass)
        {
            var SW = new Stopwatch();
            TimeSpan TS = SW.Elapsed;

            //
            // 「キャリブレーションスタート信号」
            //

            // "cs01" 送信 : キャリブレーションスタート
            signalclass.SignalSend(DeviceId.MasterId, SendSignal.MCalibrationStart);
            // "cs02" 送信 : キャリブレーションスタート
            signalclass.SignalSend(DeviceId.ReceiveId, SendSignal.SCalibrationStart);



            // 経過時間計測 3秒
            while (TS.Seconds < Time.ScreenTrans)
            {
                // 計測開始
                SW.Start();
                // 経過時間合計
                TS = SW.Elapsed;

                Dispatcher.Invoke((Action)(() =>
                {
                    CountLabel.Content = (Time.ScreenTrans-TS.Seconds) + "後にキャリブレーション画面へ移行します";
                }));
            }
            Dispatcher.Invoke((Action)(() =>
            {
                // キャリブレーション準備画面へ
                var calibrationstandby_page = new CalibrationStandby_Page(signalclass);
                NavigationService.Navigate(calibrationstandby_page);


                /*
                //
                // 「キャリブレーション完了信号」
                //

                // "ce02" 送信 : キャリブレーション終了
                signalclass.SignalSend(DeviceId.ReceiveId, SendSignal.SCalibrationComple);
                // "ce01" 送信 : キャリブレーション終了
                signalclass.SignalSend(DeviceId.MasterId, SendSignal.MCalibrationComple);

                // 稼働準備画面へ移行
                var operationstandby_page = new OperationStandby_Page(signalclass);
                NavigationService.Navigate(operationstandby_page);*/
            }));
        }
    }
}
