using MISOTEN_APPLICATION.BackProcess;
using MISOTEN_APPLICATION.Screen.CommonClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.Json;
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
    /// SignalConnect_Page.xaml の相互作用ロジック
    /// </summary>
    public partial class SignalConnect_Page : Page
    {
        // エラー文
        string ErrorSentence = "";
        // シリアルポート
        SerialPort MasterPort = null;
        SerialPort SlaveProt = null;

        // 送受信クラス
        SignalClass SignalClass = new SignalClass();
        // 表示用クラス
        FileClass file = new FileClass();

        public SignalConnect_Page()
        {
            InitializeComponent();
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {

            Task MeasurementTask = Task.Run(() => { Window_Load(); });
        }

        private void Window_Load()
        {
            // 接続処理
            if ((ProtConnect(MasterPort, SlaveProt) == Retrun.True) &&
                // 送受信(接続要請信号送信,接続確認信号受信)
                (ProtReceve(MasterPort, SlaveProt) == Retrun.True))
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    // 接続完了
                    var SignalConnectComp = new SignalConnectComp_Page(SignalClass);
                    NavigationService.Navigate(SignalConnectComp);
                }));
            }
            else
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    // 再接続
                    var SignalReConnect = new SignalReConnect_Page(SignalClass, ErrorSentence);
                    NavigationService.Navigate(SignalReConnect);
                }));
            }
        }

        /* ポートセット */
        private SerialPort SettingPort(SerialPort serialPort, SerialPortData serialPortData)
        {

            // まだポートに繋がっていない場合
            if (serialPort == null)
            {
                // serialPortの設定
                serialPort = new SerialPort();
                serialPort.PortName = serialPortData.comName;
                serialPort.BaudRate = serialPortData.baudRate;
                serialPort.DataBits = serialPortData.dataBits;
                serialPort.Parity = Parity.None;
                serialPort.StopBits = StopBits.One;
                serialPort.Encoding = Encoding.UTF8;
                serialPort.WriteTimeout = serialPortData.writeTimeout;
            }
            return serialPort;
        }

        /* ポート接続処理 */
        private int ProtConnect(SerialPort masterport, SerialPort slaveprot)
        {
            Dispatcher.Invoke((Action)(() => { Execution.Content = "JSONファイル読み込み中..."; }));
            //　file読み込み
            string ResumeJson = File.ReadAllText("Json\\SerialPort.json");
            // JSONデータからオブジェクトを復元
            List<SerialPortData> product = JsonSerializer.Deserialize<List<SerialPortData>>(ResumeJson);
            // masterシリアルポートに接続
            try
            {
                // ポートセット
                masterport = SettingPort(masterport, product[DeviceId.MasterId]);
                slaveprot = SettingPort(slaveprot, product[DeviceId.ReceiveId]);
                // ポートオープン
                masterport.Open();
                slaveprot.Open();
                // ファイル書き込み
                file.MFirst();
                file.SFirst();
                // シリアルポートセット
                SignalClass.SetSerialport(masterport, DeviceId.MasterId);
                SignalClass.SetSerialport(slaveprot, DeviceId.ReceiveId);
                Dispatcher.Invoke((Action)(() => { Execution.Content = "受信バンドラ立ち上げ中..."+" ( 5秒 )"; }));
                TimerClass.Sleep(5000);
                MasterPort = masterport;
                SlaveProt = slaveprot;

                return Retrun.True;
            }
            catch (Exception ex)
            {
                ErrorSentence = ex.Message;
                return Retrun.False;
            }
        }

        /* ポート受信処理 */
        private int ProtReceve(SerialPort masterport, SerialPort slaveprot)
        {
            // マスター受信値
            ReciveData MReciveData = new ReciveData();
            // マスター受信値
            ReciveData SReciveData = new ReciveData();
            
            //
            // 「マスター：接続要請信号」
            //
            Task<ReciveData> MRtask = Task.Run(() => { return SignalClass.GetMReciveData(); });
            Dispatcher.Invoke((Action)(() => { Execution.Content = "マスターから 接続確認信号(ca10) 受信待機中"; }));
            // "ct01" 送信
            SignalClass.SignalSend(DeviceId.MasterId, SendSignal.MConnectRequest);
            // 計測開始("ct01" 送信から"ca10"受信まで)
            // 受信タスク終了まで待機
            MReciveData = MRtask.Result;
            file.MLog(MReciveData.RSignal);
            // 受信値チェック
            if (MReciveData.RSignal != ReceveSignal.MConnectRequest)
            {
                ErrorSentence = MReciveData.RSignal;
                return Retrun.False;
            }

            //
            // 「マスター：接続完了信号」
            //
            MRtask = Task.Run(() => { return SignalClass.GetMReciveData(); });
            Dispatcher.Invoke((Action)(() => { Execution.Content = "マスターから 接続完了信号(cc10) 受信待機中"; }));
            // "cc01" 送信
            SignalClass.SignalSend(DeviceId.MasterId, SendSignal.MConnectComple);
            // 計測開始("cc01" 送信から"cc10"受信まで)
            // 受信タスク終了まで待機
            MReciveData = MRtask.Result;
            file.MLog(MReciveData.RSignal);
            // 受信値チェック
            if (MReciveData.RSignal != ReceveSignal.MConnectComple)
            {
                ErrorSentence = MReciveData.RSignal;
                return Retrun.False;
            }

            //
            // 「スレーブ：接続要請信号」
            //

            // バッファ内削除
            SignalClass.ReceiveClearBuffer(DeviceId.ReceiveId);
            Task<ReciveData> SRtask = Task.Run(() => { return SignalClass.GetSReciveData(); });
            Dispatcher.Invoke((Action)(() => { Execution.Content = "スレーブから 接続確認信号(ct20) 受信待機中"; }));
            // "ct02" 送信
            SignalClass.SignalSend(DeviceId.ReceiveId, SendSignal.SConnectRequest);
            // 計測開始("ct02" 送信から"ca20"受信まで)
            // 受信タスク終了まで待機
            SReciveData = SRtask.Result;
            //file.SLog(SReciveData.RSignal);
            // 受信値チェック
            if (SReciveData.RSignal != ReceveSignal.SConnectRequest)
            {
                ErrorSentence = SReciveData.RSignal;
                return Retrun.False;
            }

            //
            // 「スレーブ：接続完了信号」
            //

            SRtask = Task.Run(() => { return SignalClass.GetSReciveData(); });
            Dispatcher.Invoke((Action)(() => { Execution.Content = "スレーブから 接続完了信号(cc20) 受信待機中"; }));
            // "cc02" 送信
            SignalClass.SignalSend(DeviceId.ReceiveId, SendSignal.SConnectComple);
            // 計測開始("cc02" 送信から"cc20"受信まで)
            // 受信タスク終了まで待機
            SReciveData = SRtask.Result;
            //file.SLog(SReciveData.RSignal);
            // 受信値チェック
            if (SReciveData.RSignal != ReceveSignal.SConnectComple)
            {
                ErrorSentence = SReciveData.RSignal;
                return Retrun.False;
            }
            return Retrun.True;
        }
    }
}
