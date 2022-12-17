using MISOTEN_APPLICATION.BackProcess;
using MISOTEN_APPLICATION.Screen.CommonClass;
using MISOTEN_APPLICATION.Screen.Operation;
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

namespace MISOTEN_APPLICATION.Screen.Calibration
{
    /// <summary>
    /// Calibration_Page.xaml の相互作用ロジック
    /// </summary>
    public partial class Calibration_Page : Page
    {
        SignalClass Signalclass = new SignalClass();
        // 排他制御に使用するオブジェクト
        private static Object lockObject = new Object();
        // 処理フラグ
        int ProcFlog = Flog.CalibNone;
        // 処理終了フラグ
        int EndFlog = Flog.Start;

        public Calibration_Page(SignalClass signalclass)
        {
            InitializeComponent();
            Signalclass = signalclass;
            // 再計測・計測終了ボタン
            EndButton.Visibility = Visibility.Hidden;
            ReMeasureButton.Visibility = Visibility.Hidden;

            CalibrationButton.IsEnabled = false;
            SliderHidden();
        }

        void PageLoad(object sender, RoutedEventArgs e)
        {
            // 時間計測タスク
            Task MeasurementTask = Task.Run(() => { Measurement(); });
            // 値受信タスク
            Task ReceveTask = Task.Run(() => { Receve(); });
        }

        /* Slider隠す */
        private void SliderHidden()
        {
            LittleSlider.Visibility = Visibility.Hidden;
            RingSlider.Visibility = Visibility.Hidden;
            MiddleSlider.Visibility = Visibility.Hidden;
            IndexSlider.Visibility = Visibility.Hidden;
            ThumbSlider.Visibility = Visibility.Hidden;
            Label.Visibility = Visibility.Hidden;
            LLabel.Visibility = Visibility.Hidden;
            RLabel.Visibility = Visibility.Hidden;
            MLabel.Visibility = Visibility.Hidden;
            ILabel.Visibility = Visibility.Hidden;
            TLabel.Visibility = Visibility.Hidden;
            LittleLabel.Visibility = Visibility.Hidden;
            RingLabel.Visibility = Visibility.Hidden;
            MiddleLabel.Visibility = Visibility.Hidden;
            IndexLabel.Visibility = Visibility.Hidden;
            ThumbLabel.Visibility = Visibility.Hidden;
        }

        /* 時間計測処理 */
        private void Measurement()
        {
            /*
            // 手を広げる処理
            Expand();
            // 手を握る処理
            Grasp();*/
            // 手のひら最大数設定処理
            Pushing();
            
            Dispatcher.Invoke((Action)(() =>
            {
                CountLabel.Content = "再計測or計測終了を選択してください";
                SliderHidden();
                CalibrationButton.Visibility = Visibility.Hidden;
                EndButton.Visibility = Visibility.Visible;
                ReMeasureButton.Visibility = Visibility.Visible;
            }));

            return;
        }
        /* 時間計測処理(手を広げる) */
        private void Expand()
        {
            var SW = new Stopwatch();
            TimeSpan TS = SW.Elapsed;
            TimeSpan BeforeTime = SW.Elapsed;

            // キャリブレーション処理タスク
            Task CalibrationTask = Task.Run(() => { CalibrationAsync(Flog.CalibOpen, Signalclass); });

            // 手を広げる
            while (ProcFlog != Flog.CalibOpen)
            {
                // 計測開始
                SW.Start();
                // 経過時間合計
                TS = SW.Elapsed;

                if (BeforeTime.Seconds < TS.Seconds)
                {
                    // 1秒ごとに書き込み処理 
                    Dispatcher.Invoke((Action)(() =>
                    {
                        CountLabel.Content = " 手を広げたまま、" + "放置してください";
                        CalibrationButton.Content = TS.Seconds + "秒経過";
                    }));
                    // 前回経過時間合計
                    BeforeTime = TS;
                }
            }
            return;
        }
        /* 時間計測処理(手を握る) */
        private void Grasp()
        {
            var SW = new Stopwatch();
            TimeSpan TS = SW.Elapsed;
            TimeSpan BeforeTime = SW.Elapsed;

            // キャリブレーション処理タスク
            Task CalibrationTask = Task.Run(() => { CalibrationAsync(Flog.CalibClose, Signalclass); });

            // 手を握る 
            while (ProcFlog != Flog.CalibClose)
            {
                // 計測開始
                SW.Start();
                // 経過時間合計
                TS = SW.Elapsed;

                if (BeforeTime.Seconds < TS.Seconds)
                {
                    // 1秒ごとに書き込み処理 
                    Dispatcher.Invoke((Action)(() =>
                    {
                        CountLabel.Content = " 　手を握り、" + "放置してください";
                        CalibrationButton.Content = TS.Seconds + "秒経過";
                    }));
                    // 前回経過時間合計
                    BeforeTime = TS;
                }
            }
            return;
        }
        /* 時間計測処理(手のひら) */
        private void Pushing()
        {
            /* Slider隠す */
            Dispatcher.Invoke((Action)(() =>
            {
                CalibrationButton.IsEnabled = true;
                LittleSlider.Visibility = Visibility.Visible;
                RingSlider.Visibility = Visibility.Visible;
                MiddleSlider.Visibility = Visibility.Visible;
                IndexSlider.Visibility = Visibility.Visible;
                ThumbSlider.Visibility = Visibility.Visible;
                Label.Visibility = Visibility.Visible;
                LLabel.Visibility = Visibility.Visible;
                RLabel.Visibility = Visibility.Visible;
                MLabel.Visibility = Visibility.Visible;
                ILabel.Visibility = Visibility.Visible;
                TLabel.Visibility = Visibility.Visible;
                LittleLabel.Visibility = Visibility.Visible;
                RingLabel.Visibility = Visibility.Visible;
                MiddleLabel.Visibility = Visibility.Visible;
                IndexLabel.Visibility = Visibility.Visible;
                ThumbLabel.Visibility = Visibility.Visible;
                CountLabel.Content = "手のひらの最大感度を設定してください";
                CalibrationButton.Content = "終了";

            }));
            while (ProcFlog != Flog.CalibPush) ;
          }
        /* 終了ボタンクリック処理 */
        private void CalibrationButton_Click(object sender, RoutedEventArgs e)
        {
            ProcFlog = Flog.CalibPush;

        }


        /* スレーブ値受信処理 */
        private void Receve()
        {
            //
            // 「キャリブレーションスタート信号」
            //

            // "cs01" 送信 : キャリブレーションスタート
            Signalclass.SignalSend(DeviceId.MasterId, SendSignal.MCalibrationStart);
            // "cs02" 送信 : キャリブレーションスタート
            Signalclass.SignalSend(DeviceId.ReceiveId, SendSignal.SCalibrationStart);

            while (EndFlog != Flog.End) ;

            //
            // 「キャリブレーション完了信号」
            //

            // "ce02" 送信 : キャリブレーション終了
            Signalclass.SignalSend(DeviceId.ReceiveId, SendSignal.SCalibrationComple);
            // "ce01" 送信 : キャリブレーション終了
            Signalclass.SignalSend(DeviceId.MasterId, SendSignal.MCalibrationComple);
            return;
        }

        /* キャリブレーション処理 */
        private async void CalibrationAsync(int flog, SignalClass signalclass)
        {
            GodHand godhand = new GodHand();
            var reslt = await godhand.calibration(flog, signalclass);
            ProcFlog = flog;
            return;
        }

        // 計測終了
        private void EndButton_Click(object sender, RoutedEventArgs e)
        {
            // レシーブ受信タスク終了
            lock (lockObject) EndFlog = Flog.End;
            // 稼働準備画面へ移行
            var operationstandby_page = new OperationStandby_Page(Signalclass);
            NavigationService.Navigate(operationstandby_page);
        }

        // 再度計測
        private void ReMeasureButton_Click(object sender, RoutedEventArgs e)
        {
            // キャリブレーション準備画面へ移行
            var calibrationstandby_page = new CalibrationStandby_Page(Signalclass);
            NavigationService.Navigate(calibrationstandby_page);
        }

        /* Slider数値変更 */
        private void LittleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { LittleLabel.Content = ((int)e.NewValue).ToString(); MottorSend(); }
        private void RingSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { RingLabel.Content = ((int)e.NewValue).ToString(); MottorSend(); }
        private void MiddleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { MiddleLabel.Content = ((int)e.NewValue).ToString(); MottorSend(); }
        private void IndexSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { IndexLabel.Content = ((int)e.NewValue).ToString(); MottorSend(); }
        private void ThumbSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { ThumbLabel.Content = ((int)e.NewValue).ToString(); MottorSend(); }

        /* モータ値送信処理 */
        private void MottorSend()
        {
            GODS_SENTENCE sendData = new GODS_SENTENCE();
            SignalClass signalclass = new SignalClass();
            // 初期化
            sendData.frist_godsentence.tip_pwm = 0;
            sendData.second_godsentence.tip_pwm = 0;
            sendData.third_godsentence.tip_pwm = 0;
            sendData.fourth_godsentence.tip_pwm = 0;
            sendData.fifth_godsentence.tip_pwm = 0;
            // 格納
            sendData.frist_godsentence.palm_pwm = Convert.ToInt32(LittleLabel.Content);
            sendData.second_godsentence.palm_pwm = Convert.ToInt32(RingLabel.Content);
            sendData.third_godsentence.palm_pwm = Convert.ToInt32(MiddleLabel.Content);
            sendData.fourth_godsentence.palm_pwm = Convert.ToInt32(IndexLabel.Content);
            sendData.fifth_godsentence.palm_pwm = Convert.ToInt32(ThumbLabel.Content);
            // 送信処理
            signalclass.SetSendData(sendData);
        }
    }
}
