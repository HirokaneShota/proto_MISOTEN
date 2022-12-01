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

namespace MISOTEN_APPLICATION.Screen.SignalConnect
{
    /// <summary>
    /// SignalConnectComp_Page.xaml の相互作用ロジック
    /// </summary>
    public partial class SignalConnectComp_Page : Page
    {
        public SignalConnectComp_Page(ArgSignal argsignal)
        {
            InitializeComponent();
            // 時間計測タスク
            Task MeasurementTask = Task.Run(() => { Measurement(argsignal); });
        }

        /* 時間計測タスク */
        private void Measurement(ArgSignal argsignal)
        {
            var SW = new Stopwatch();
            TimeSpan TS = SW.Elapsed;

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
                var calibrationstandby_page = new CalibrationStandby_Page(argsignal);
                NavigationService.Navigate(calibrationstandby_page);
            }));
        }
    }
}
