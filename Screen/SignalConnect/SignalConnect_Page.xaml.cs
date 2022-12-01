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
        SerialPort ReceiveProt = null;

        // マスター送受信クラス
        SignalClass MSignalClass = new SignalClass();
        // スレーブ送受信クラス
        SignalClass SSignalClass = new SignalClass();

        // 引数
        ArgSignal argsignal = new ArgSignal();


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
            if ((ProtConnect(MasterPort, ReceiveProt) == Retrun.True) &&
                // 送受信(接続要請信号送信,接続確認信号受信)
                (ProtReceve(MasterPort, ReceiveProt) == Retrun.True))
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    // 引数用格納
                    argsignal.Msignalclass = MSignalClass;
                    argsignal.Ssignalclass = SSignalClass;
                    argsignal.Masterport = MasterPort;
                    argsignal.Seceiveprot = ReceiveProt;

                    // 接続完了
                    var SignalConnectComp = new SignalConnectComp_Page(argsignal);
                    NavigationService.Navigate(SignalConnectComp);
                }));
            }
            else
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    // 再接続
                    var SignalReConnect = new SignalReConnect_Page(ErrorSentence);
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
        private int ProtConnect(SerialPort masterport, SerialPort receiveprot)
        {
            //　file読み込み
            string ResumeJson = File.ReadAllText("Json\\SerialPort.json");
            // JSONデータからオブジェクトを復元
            List<SerialPortData> product = JsonSerializer.Deserialize<List<SerialPortData>>(ResumeJson);
            // masterシリアルポートに接続
            try
            {
                // ポートセット
                masterport = SettingPort(masterport, product[DeviceId.MasterId]);
                //receiveprot = SettingPort(receiveprot, product[DeviceId.ReceiveId]);
                // ポートオープン
                masterport.Open();
                //receiveprot.Open();
                File.WriteAllText(@URI.MasterLog, "マスター接続開始" + Environment.NewLine);
                //File.WriteAllText(@URI.ReceiveLog, "レシーブ接続開始" + Environment.NewLine);
                MasterPort = masterport;
                ReceiveProt = receiveprot;

                return Retrun.True;
            }
            catch (Exception ex)
            {
                ErrorSentence = ex.Message;
                return Retrun.False;
            }
        }

        /* ポート受信処理 */
        private int ProtReceve(SerialPort masterport, SerialPort receiveprot)
        {
            // マスター受信値
            ReciveData MReciveData = new ReciveData();
            // 時間計測
            Timer time = new Timer();

            // マスター受信Handlerタスク
            MSignalClass.ReceiveHandler(masterport);
            // ReceiveHandlerより先に送信しないように
            Timer.Sleep(7000);

            //
            // 「マスター：接続要請信号」
            //
            Task<ReciveData> MRtask = Task.Run(() => { return MSignalClass.NumReceived(); });
            // "ct01" 送信
            MSignalClass.SignalSend(masterport, SendSignal.MConnectRequest);
            // 計測開始("ct01" 送信から"ca10"受信まで)
            time.Start();
            // 受信タスク終了まで待機
            MReciveData = MRtask.Result;

            File.AppendAllText(@URI.MasterLog, time.MiliElapsed()+ "ms :" + MReciveData.RString + Environment.NewLine);

            if (MReciveData.RString != ReceveSignal.MConnectRequest)
            {
                ErrorSentence = MReciveData.RString;
                return Retrun.False;
            }

            //
            // 「マスター：接続完了信号」
            //
            MSignalClass.InitSignal();
            MRtask = Task.Run(() => { return MSignalClass.NumReceived(); });
            // ReceiveHandlerより先に送信しないように
            // "cc01" 送信
            MSignalClass.SignalSend(masterport, SendSignal.MConnectComple);
            // 計測開始("cc01" 送信から"cc10"受信まで)
            time.ReStart();
            // 受信タスク終了まで待機
            MReciveData = MRtask.Result;
            File.AppendAllText(@URI.MasterLog, time.MiliElapsed() + "ms :" + MReciveData.RString + Environment.NewLine);

            if (MReciveData.RString != ReceveSignal.MConnectComple)
            {
                ErrorSentence = MReciveData.RString;
                return Retrun.False;
            }




            /*
            // マスター受信値
            ReciveData MReciveData = new ReciveData();
            // レシーブ送受信クラス
            SignalClass RSignalClass = new SignalClass();
            //  レシーブ受信タスク
            RSignalClass.ReceiveHandler(receiveprot);
            Task<ReciveData> RRtask = Task.Run(() => { return RSignalClass.NumReceived(); });
            // "" 送信
            RSignalClass.SignalSend(receiveprot, SignalSendData.RConnectRequest);
            // 受信タスク終了まで待機
            RReciveData = RRtask.Result;
            File.AppendAllText(@URI.MasterLog, RReciveData.RString + Environment.NewLine);

            */
            return Retrun.True;
        }
    }
}
