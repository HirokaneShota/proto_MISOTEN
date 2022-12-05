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

        // マスター受信値
        ReciveData_Sensor MSensor = new ReciveData_Sensor();
        // スレーブ受信値
        ReciveData_Sensor SSensor = new ReciveData_Sensor();
        // マスター受信時間(ミリ秒)
        double MTime = 0;
        //スレーブ受信時間(ミリ秒)
        double STime = 0;
        // センサー値取得フラグ
        int SensFlog = Flog.SON;
        // 処理終了フラグ
        int EndFlog = Flog.Start;

        public Operation_Page(SignalClass signalclass)
        {
            InitializeComponent();
            Signalclass = signalclass;
        }

        void PageLoad(object sender, RoutedEventArgs e)
        {
            // マスター値受信タスク
            Task MReceveTask = Task.Run(() => { MReceve(); });
            // スレーブ値受信タスク
            Task SReceveTask = Task.Run(() => { SReceve(); });
            // 稼働処理タスク
            Task PlayingTask = Task.Run(() => { Playing(); });
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

            // マスター:"ss01" 送信 : センシング開始信号
            Signalclass.SignalSend(DeviceId.MasterId, SendSignal.MSensingStart);
            // 処理終了フラグが立つまで
            while (EndFlog != Flog.End)
            {
                // センサー値取得不可
                if (SensFlog == Flog.SOFF) continue;
                // 計測再スタート
                time.ReStart();
                // マスター値受信
                Task<ReciveData> MRtask = Task.Run(() => { return Signalclass.GetMReciveData(); });
                // 受信タスク終了まで待機
                MReciveData = MRtask.Result;
                lock (lockObject)
                {
                    MTime = time.MiliElapsed();
                    //センサー値格納
                    MSensor = MReciveData.RSensor;
                }
            }

        }

        /* スレーブ値受信処理 */
        private void SReceve()
        {

        }
        /* 稼働時処理 */
        private void Playing()
        {
            //
            // マスター受信値：MSensor & スレーブ受信値：SSensor
            // マスター受信時間：MTime & スレーブ受信時間：STime
            // 上記の変数へセンサーの値、受信時間()を受信次第格納(上書き)している。
            // 受信値は、構造体になっている為、Screen\CommonClass\DataClass.cs(ReciveData_Sensor)を参照
            // 
            // センサー値の更新を止める際は、センサー値取得フラグ：SensFlog を Flog.SOFF
            // センサー値を更新する際は、センサー値取得フラグ：SensFlog を Flog.SON　へ設定してください。
            //

        }

        /* 終了ボタン処理 */
        private void EndButton_Click(object sender, RoutedEventArgs e)
        {

            // マスター・スレーブ処理タスク終了
            lock (lockObject) EndFlog = Flog.End;
            // マスター:"sh01" 送信 : センシング停止信号
            Signalclass.SignalSend(DeviceId.MasterId, SendSignal.MSensingStop);

            // システム選択画面へ移行
            var systemselect_page = new SystemSelect_Page(Signalclass);
            NavigationService.Navigate(systemselect_page);
        }
        /* キャリブレーションボタン処理 */
        private void CalibrationButton_Click(object sender, RoutedEventArgs e)
        {
            // マスター・スレーブ処理タスク終了
            lock (lockObject) EndFlog = Flog.End;
            // マスター:"sh01" 送信 : センシング停止信号
            Signalclass.SignalSend(DeviceId.MasterId, SendSignal.MSensingStop);
            // マスター:"sr01" 送信 : センシングリセット信号
            Signalclass.SignalSend(DeviceId.MasterId, SendSignal.MSensingReset);

            // キャリブレーション準備画面へ移行
            var calibrationstandby_page = new CalibrationStandby_Page(Signalclass);
            NavigationService.Navigate(calibrationstandby_page);
        }
    }
}
