using MISOTEN_APPLICATION.BackProcess;
using MISOTEN_APPLICATION.Screen.CommonClass;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using static System.Net.Mime.MediaTypeNames;

namespace MISOTEN_APPLICATION.Screen.Calibration
{
    /// <summary>
    /// MasterCalibration__Page.xaml の相互作用ロジック
    /// </summary>
    public partial class MasterCalibration__Page : Page
    {
        ArgSignal argSignal = new ArgSignal();
        // 排他制御に使用するオブジェクト
        private static Object lockObject = new Object();

        // マスター受信値
        ReciveData_Sensor MSensor = new ReciveData_Sensor();
        // マスター受信時間(ミリ秒)
        double MTime = 0;
        // センサー値取得フラグ
        int SensFlog = Flog.SON;
        // 処理終了フラグ
        int EndFlog = Flog.Start;

        public MasterCalibration__Page(ArgSignal argsignal)
        {
            InitializeComponent();
            argSignal = argsignal;
            //MasterCalibrationButton.IsEnabled = false;
            // 再計測・レシーブボタン
            SlaveButton.Visibility = Visibility.Hidden;
            ReMeasureButton.Visibility = Visibility.Hidden;
        }
        void PageLoad(object sender, RoutedEventArgs e)
        {
            // 時間計測タスク
            Task MeasurementTask = Task.Run(() => { Measurement(); });
            // マスター値受信タスク
            Task MReceveTask = Task.Run(() => { MReceve(); });
            // キャリブレーション処理タスク
            Task CalibrationTask = Task.Run(() => { Calibration(); });
        }
        /* 時間計測処理 */
        private void Measurement()
        {
            // 手を広げる処理
            expand();
            // 手を握る処理
            Grasp();
            Dispatcher.Invoke((Action)(() =>
            {
                CountLabel.Content = "再計測orスレーブ計測開始を選択してください";
                MasterCalibrationButton.Visibility = Visibility.Hidden;
                SlaveButton.Visibility = Visibility.Visible;
                ReMeasureButton.Visibility = Visibility.Visible;
            }));
        }
        /* 時間計測処理(手を広げる) */
        private void expand()
        {
            var SW = new Stopwatch();
            TimeSpan TS = SW.Elapsed;
            TimeSpan BeforeTime = SW.Elapsed;

            //Dispatcher.Invoke((Action)(() =>{  MoveUli = "../ ../move/test.mp4"; }));

            // 手を広げる
            // 経過時間計測 5秒
            while (TS.Seconds < Time.MECalibration)
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
                        CountLabel.Content = " 手を広げたまま、" + (Time.MECalibration) + "秒間放置してください";
                        MasterCalibrationButton.Content = (Time.MECalibration - TS.Seconds) + "秒後...";
                    }));
                    // 前回経過時間合計
                    BeforeTime = TS;
                }
            }
        }
        /* 時間計測処理(手を握る) */
        private void Grasp()
        {
            var SW = new Stopwatch();
            TimeSpan TS = SW.Elapsed;
            TimeSpan BeforeTime = SW.Elapsed;

            // 手を握る 
            // 経過時間計測 5秒
            while (TS.Seconds < Time.MGCalibration)
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
                        CountLabel.Content = " 　手を握り、" + (Time.MGCalibration) + "秒間放置してください";
                        MasterCalibrationButton.Content = (Time.MGCalibration - TS.Seconds) + "秒後...";
                    }));
                    // 前回経過時間合計
                    BeforeTime = TS;
                }
            }
        }
        /* マスター値受信処理 */
        private void MReceve()
        {
            // マスター受信値
            ReciveData MReciveData = new ReciveData();
            // 時間計測
            Timer time = new Timer();
            // 計測スタート
            time.Start();
            //
            // 「マスター：キャリブレーションスタート信号」
            //

            // "cs01" 送信 : キャリブレーションスタート
            argSignal.Msignalclass.SignalSend(argSignal.Masterport, SendSignal.MCalibrationStart);
            // 処理終了フラグが立つまで
            while (EndFlog != Flog.End)
            {
                // センサー値取得不可
                if (SensFlog == Flog.SOFF) continue;
                // 計測再スタート
                time.ReStart();
                // マスター値受信
                Task<ReciveData> MRtask = Task.Run(() => { return argSignal.Msignalclass.NumReceived(); });
                // 受信タスク終了まで待機
                MReciveData = MRtask.Result;
                lock (lockObject)
                {
                    MTime = time.MiliElapsed();
                    //センサー値格納
                    MSensor = MReciveData.RSensor;
                }
            }

            //
            // 「マスター：キャリブレーション完了信号」
            //

            // "ce01" 送信 : キャリブレーション終了
            //argSignal.Msignalclass.SignalSend(argSignal.Masterport, SendSignal.MCalibrationComple);
        }

        /* キャリブレーション処理 */
        private void Calibration() 
        {
            //
            // マスター受信値：MSensor 
            // マスター受信時間：MTime 
            // 上記の変数へセンサーの値、受信時間()を受信次第格納(上書き)している。
            // 受信値は、構造体になっている為、Screen\CommonClass\DataClass.cs(ReciveData_Sensor)を参照
            // 
            // センサー値の更新を止める際は、センサー値取得フラグ：SensFlog を Flog.SOFF
            // センサー値を更新する際は、センサー値取得フラグ：SensFlog を Flog.SON　へ設定してください。
            //
            // 処理(キャリブレーション)の終了後には、処理終了フラグ：EndFlog を Flog.Endへ設定してください。
            //

        }

        // スレーブ計測
        private void SlaveButton_Click(object sender, RoutedEventArgs e)
        {
            // スレーブキャリブレーション画面へ移行
            var slavecalibration_page = new SlaveCalibration_Page(argSignal);
            NavigationService.Navigate(slavecalibration_page);
        }

        // 再度計測
        private void ReMeasureButton_Click(object sender, RoutedEventArgs e)
        {
            // キャリブレーション準備画面へ移行
            var calibrationstandby_page = new CalibrationStandby_Page(argSignal);
            NavigationService.Navigate(calibrationstandby_page);
        }
    }
}
