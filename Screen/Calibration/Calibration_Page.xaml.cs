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
        // スタート内容フラグ
        int StartFlog = Flog.CalibNone;
        // ゴッドハンド実体化
        GodHand ggodhand;

        public Calibration_Page(SignalClass _signalclass,int _flog, GodHand _godhand)
        {
            InitializeComponent();
            Signalclass = _signalclass;
            // 再計測・計測終了ボタン
            EndButton.Visibility = Visibility.Hidden;
            ReMeasureButton.Visibility = Visibility.Hidden;
            MottorEndButton.Visibility = Visibility.Hidden;
            SendButton.Visibility = Visibility.Hidden;

            CalibrationButton.IsEnabled = false;
            SliderHidden();
            StartFlog = _flog;
            ggodhand = _godhand;
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
            LittleTextBlock.Visibility = Visibility.Hidden;
            RingTextBlock.Visibility = Visibility.Hidden;
            MiddleTextBlock.Visibility = Visibility.Hidden;
            IndexTextBlock.Visibility = Visibility.Hidden;
            ThumbTextBlock.Visibility = Visibility.Hidden;
        }

        /* 時間計測処理 */
        private void Measurement()
        {
            // ログあり開始(マスター)&ログあり開始(スレーブ)
            if (StartFlog == Flog.MLogON || StartFlog == Flog.SLogON)
            {
                // 手を広げる処理
                Expand();
                // 手を握る処理
                //Grasp();
            }
            // リアルタイム&ログあり開始(スレーブ)
            if(StartFlog == Flog.RialON || StartFlog == Flog.SLogON)
            {
                // 手のひら最大数設定処理
                Pushing();
            }
            
            Dispatcher.Invoke((Action)(() =>
            {
                CountLabel.Content = "再計測or計測終了を選択してください";
                SliderHidden();

                CalibrationButton.IsEnabled = true;
                CalibrationButton.Content = "終了";
                CalibrationButton.Visibility = Visibility.Hidden;

                MottorEndButton.Visibility = Visibility.Hidden;
                SendButton.Visibility = Visibility.Hidden;

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
            Task CalibrationTask = Task.Run(() => { CalibrationAsync(StartFlog, Signalclass); });

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
                // サーボモータキャリブレーション
                CalibrationButton.IsEnabled = true;
                CalibrationButton.Content = "終了";
                CalibrationButton.Visibility = Visibility.Hidden;

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
                LittleTextBlock.Visibility = Visibility.Visible;
                RingTextBlock.Visibility = Visibility.Visible;
                MiddleTextBlock.Visibility = Visibility.Visible;
                IndexTextBlock.Visibility = Visibility.Visible;
                ThumbTextBlock.Visibility = Visibility.Visible;
                CountLabel.Content = "手のひらの最大感度を設定してください";
                MottorEndButton.Visibility = Visibility.Visible;
                SendButton.Visibility = Visibility.Visible;

            }));
            while (ProcFlog != Flog.CalibPush) ;
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

            // "ce01" 送信 : キャリブレーション終了
            Signalclass.SignalSend(DeviceId.MasterId, SendSignal.MCalibrationComple);
            // "ce02" 送信 : キャリブレーション終了
            Signalclass.SignalSend(DeviceId.ReceiveId, SendSignal.SCalibrationComple);
            return;
        }

        /* キャリブレーション処理 */
        private async void CalibrationAsync(int flog, SignalClass signalclass)
        {
            // マスター
            if(flog == Flog.MLogON)
            {
                var reslt = await ggodhand.calibration(0, signalclass);
            }
            // スレーブ
            else if(flog == Flog.SLogON)
            {
                var reslt = await ggodhand.calibration(1, signalclass);
            }
            ProcFlog = Flog.CalibOpen;
            return;
        }

        /* 計測終了 */
        private void EndButton_Click(object sender, RoutedEventArgs e)
        {
            // レシーブ受信タスク終了
            lock (lockObject) EndFlog = Flog.End;
            // 稼働準備画面へ移行
            var operationstandby_page = new OperationStandby_Page(Signalclass, ggodhand, StartFlog);
            NavigationService.Navigate(operationstandby_page);
        }

        /* 再度計測 */
        private void ReMeasureButton_Click(object sender, RoutedEventArgs e)
        {
            // キャリブレーション準備画面へ移行
            var calibrationstandby_page = new CalibrationStandby_Page(Signalclass);
            NavigationService.Navigate(calibrationstandby_page);
        }

        /* Slider数値変更 */
        
        private void LittleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { LittleTextBlock.Text = ((int)e.NewValue).ToString();}
        private void RingSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { RingTextBlock.Text = ((int)e.NewValue).ToString();}
        private void MiddleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { MiddleTextBlock.Text = ((int)e.NewValue).ToString();}
        private void IndexSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { IndexTextBlock.Text = ((int)e.NewValue).ToString();}
        private void ThumbSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { ThumbTextBlock.Text = ((int)e.NewValue).ToString();}

        /* 終了ボタンクリック処理 */
        private void MottorEndButton_Click(object sender, RoutedEventArgs e)
        {
            // 設定値格納
            ggodhand.servocalibration(MottorSend());
            // 終了フラグ
            ProcFlog = Flog.CalibPush;
        }
        /* 送信ボタンクリック処理 */
        private void SendButton_Click(object sender, RoutedEventArgs e) 
        { 
            // 送信処理
            Signalclass.SetSendMotor(MottorSend());
        }


        /* モータ値送信処理 */
        private GODS_SENTENCE MottorSend()
        {
            GODS_SENTENCE sendData = new GODS_SENTENCE();
            // 格納
            sendData.frist_godsentence.palm_pwm = Convert.ToInt32(LittleTextBlock.Text);
            sendData.second_godsentence.palm_pwm = Convert.ToInt32(RingTextBlock.Text);
            sendData.third_godsentence.palm_pwm = Convert.ToInt32(MiddleTextBlock.Text);
            sendData.fourth_godsentence.palm_pwm = Convert.ToInt32(IndexTextBlock.Text);
            sendData.fifth_godsentence.palm_pwm = Convert.ToInt32(ThumbTextBlock.Text);

            return sendData;
        }
    }
}
