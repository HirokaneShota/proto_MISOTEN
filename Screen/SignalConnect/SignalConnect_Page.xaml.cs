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

        public SignalConnect_Page()
        {
            InitializeComponent();
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            // 接続処理
            if (ProtConnect(MasterPort, ReceiveProt) == Retrun.True)
            {
                // 送受信(接続要請信号送信,接続確認信号受信)
                ProtReceve(MasterPort, ReceiveProt);
                // 接続完了
                var SignalConnectComp = new SignalConnectComp_Page(MasterPort, ReceiveProt);
                NavigationService.Navigate(SignalConnectComp);
            }
            else
            {
                // 再接続
                var SignalReConnect = new SignalReConnect_Page(ErrorSentence);
                NavigationService.Navigate(SignalReConnect);
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
        private void ProtReceve(SerialPort masterport, SerialPort receiveprot)
        {
            // 受信値
            ReciveData reciveData = new ReciveData();


            // マスター受信クラスインスタンス
            //ReciveClassSignalClass
            // マスター送信クラスインスタンス
            //SendClass sendClass = new SendClass(masterport);

            SignalClass signalClass = new SignalClass();

            // マスター受信タスク
            signalClass.ReceiveHandler(masterport);
            Task<ReciveData> MRtask = Task.Run(() => { return signalClass.NumReceived(); });
            // ct01 送信
            signalClass.SignalSend(masterport, SignalSendData.MConnectRequest);
            // 受信タスク終了まで待機
            reciveData = MRtask.Result;
            File.AppendAllText(@URI.MasterLog, reciveData.RString + Environment.NewLine);

            /*
            // スレーブ受信
            signalClass.NumReceived(receiveprot);
            File.AppendAllText(@URI.ReceiveLog, reciveData.RString + Environment.NewLine);
            */
        }
    }
}
