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
        // シリアルポート
        SerialPort MasterPort;
        SerialPort ReceiveProt;

        public SignalConnect_Page()
        {
            InitializeComponent();
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            // 接続処理
            if (ProtConnect() != Retrun.True)
            {
                // 接続完了
                var SignalConnectComp = new SignalConnectComp_Page();
                NavigationService.Navigate(SignalConnectComp);
            }
            else
            {
                // 再接続
                var SignalReConnect = new SignalReConnect_Page();
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
        private int ProtConnect()
        {

            //　file読み込み
            string ResumeJson = File.ReadAllText("Json\\SerialPort.json");
            // JSONデータからオブジェクトを復元
            List<SerialPortData> product = JsonSerializer.Deserialize<List<SerialPortData>>(ResumeJson);
            // masterシリアルポートに接続
            try
            {
                // ポートセット
                MasterPort = SettingPort(MasterPort, product[DeviceId.MasterId]);
                ReceiveProt = SettingPort(ReceiveProt, product[DeviceId.ReceiveId]);
                // ポートオープン
                MasterPort.Open();
                File.WriteAllText(@"Log\MasterLog.txt", "マスター接続開始" + Environment.NewLine);
                ReceiveProt.Open();
                File.WriteAllText(@"Log\ReceiveLog.txt", "レシーブ接続開始" + Environment.NewLine);

                return Retrun.True;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return Retrun.False;
            }
        }
    }
}
