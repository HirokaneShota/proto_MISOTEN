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
    /// SlaveCalibration_Page.xaml の相互作用ロジック
    /// </summary>
    public partial class SlaveCalibration_Page : Page
    {
        ArgSignal argSignal = new ArgSignal();
        // 排他制御に使用するオブジェクト
        private static Object lockObject = new Object();
        // スレーブ受信値
        ReciveData_Sensor SSensor = new ReciveData_Sensor();
        //スレーブ受信時間(ミリ秒)
        double STime = 0;
        // センサー値取得フラグ
        int SensFlog = Flog.SON;
        // 処理終了フラグ
        int EndFlog = Flog.Start;

        public SlaveCalibration_Page(ArgSignal argsignal)
        {
            InitializeComponent();
            argSignal = argsignal;
            // 再計測・計測終了ボタン
            EndButton.Visibility = Visibility.Hidden;
            ReMeasureButton.Visibility = Visibility.Hidden;
        }

        void PageLoad(object sender, RoutedEventArgs e)
        {
            // 時間計測タスク
            Task MeasurementTask = Task.Run(() => { Measurement(); });
            // スレーブ値受信タスク
            Task SReceveTask = Task.Run(() => { SReceve(); });
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
                CountLabel.Content = "再計測or計測終了を選択してください";
                SlaveCalibrationButton.Visibility = Visibility.Hidden;
                EndButton.Visibility = Visibility.Visible;
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
            while (TS.Seconds < Time.SECalibration)
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
                        CountLabel.Content = " 手を広げたまま、" + (Time.SECalibration) + "秒間放置してください";
                        SlaveCalibrationButton.Content = (Time.SECalibration - TS.Seconds) + "秒後...";
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
            while (TS.Seconds < Time.SGCalibration)
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
                        CountLabel.Content = " 　手を握り、" + (Time.SGCalibration) + "秒間放置してください";
                        SlaveCalibrationButton.Content = (Time.SGCalibration - TS.Seconds) + "秒後...";
                    }));
                    // 前回経過時間合計
                    BeforeTime = TS;
                }
            }
        }

        /* スレーブ値受信処理 */
        private void SReceve()
        {

        }

        /* キャリブレーション処理 */
        private void Calibration()
        {
            //
            // スレーブ受信値：SSensor
            // スレーブ受信時間：STime
            // 上記の変数へセンサーの値、受信時間()を受信次第格納(上書き)している。
            // 受信値は、構造体になっている為、Screen\CommonClass\DataClass.cs(ReciveData_Sensor)を参照
            // 
            // センサー値の更新を止める際は、センサー値取得フラグ：SensFlog を Flog.SOFF
            // センサー値を更新する際は、センサー値取得フラグ：SensFlog を Flog.SON　へ設定してください。
            //

        }

        // 計測終了
        private void EndButton_Click(object sender, RoutedEventArgs e)
        {
            // レシーブ受信タスク終了
            lock (lockObject) EndFlog = Flog.End;
            // 稼働準備画面へ移行
            var operationstandby_page = new OperationStandby_Page(argSignal);
            NavigationService.Navigate(operationstandby_page);
        }

        // 再度計測
        private void ReMeasureButton_Click(object sender, RoutedEventArgs e)
        {
            // スレーブキャリブレーション画面へ移行
            var slavecalibration_page = new SlaveCalibration_Page(argSignal);
            NavigationService.Navigate(slavecalibration_page);
        }
    }
}
