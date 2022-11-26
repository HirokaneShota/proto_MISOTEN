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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MISOTEN_APPLICATION.Screen.CommonClass;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MISOTEN_APPLICATION.Screen.DevelopSystem
{
    /// <summary>
    /// DevelopConnectSystem_Page.xaml の相互作用ロジック
    /// </summary>
    public partial class DevelopConnectSystem_Page : Page
    {
        SerialPort MasterPort;
        SerialPort ReceiveProt;

        public DevelopConnectSystem_Page()
        {
            InitializeComponent();
            ReceiveText.IsReadOnly = true;

        }

        
        //ポートセット
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

        private void OpenLogFolderButton_Click(object sender, RoutedEventArgs e)
        {
            // LogFileOpen
            System.Diagnostics.Process.Start("explorer.exe", @"Log");
        }

        /* 接続処理 */
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {

            //　file読み込み
            string ResumeJson = File.ReadAllText("..\\..\\Log\\SerialPort.json");
            // JSONデータからオブジェクトを復元
            List<SerialPortData> product = JsonSerializer.Deserialize<List<SerialPortData>>(ResumeJson);

            if ((bool)MasterConnectCheck.IsChecked == true)
            {
                // masterシリアルポートに接続
                try
                {
                    // ポートセット
                    MasterPort = SettingPort(MasterPort, product[DeviceId.MasterId]);
                    // ポートオープン
                    MasterPort.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if ((bool)ReceiveConnectCheck.IsChecked == true)
            {
                // receveシリアルポートに接続
                try
                {
                    // ポートセット
                    ReceiveProt = SettingPort(ReceiveProt, product[DeviceId.ReceiveId]);
                    // ポートオープン
                    ReceiveProt.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        /* 切断処理 */
        private void CutButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)MasterConnectCheck.IsChecked == true)
            {
                // masterシリアルポート切断
                try
                {
                    if (MasterPort.IsOpen == false) return;
                    // ポート切断
                    MasterPort.Close();
                    MasterPort = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if ((bool)ReceiveConnectCheck.IsChecked == true)
            {
                // receveシリアルポート切断
                try
                {
                    if (ReceiveProt.IsOpen == false) return;
                    // ポート切断
                    ReceiveProt.Close();
                    ReceiveProt = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        /* 送信処理 */
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)MasterSendCheck.IsChecked == true)
            {
                try
                {
                    if (MasterPort == null) return;
                    if (MasterPort.IsOpen == false) return;
                    // 送信処理
                    Send(MasterPort, SendText.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if ((bool)ReceiveSendCheck.IsChecked == true)
            {
                try
                {
                    if (ReceiveProt == null) return;
                    if (ReceiveProt.IsOpen == false) return;
                    // 送信処理
                    Send(ReceiveProt, SendText.Text);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /* 受信処理 */
        private void ReceivedButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)MasterSendCheck.IsChecked == true)
            {
                try
                {
                    if (MasterPort == null) return;
                    if (MasterPort.IsOpen == false) return;
                    // 受信処理
                    MasterPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if ((bool)ReceiveSendCheck.IsChecked == true)
            {
                try
                {
                    if (ReceiveProt == null) return;
                    if (ReceiveProt.IsOpen == false) return;
                    // 受信処理
                    ReceiveProt.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        /* 受信用関数 */
        private void SerialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = (SerialPort)sender;
            // 繋がっているかどうか判断
            if (serialPort == null) return;
            if (serialPort.IsOpen == false) return;

            // TextBox表示へ
            try
            {
                // 受信バッファ内のByte数
                Int32 datanum = serialPort.BytesToRead;
                // byte型受信用変数宣言
                byte[] data = new byte[datanum];
                // dataへdatanum数分格納
                Int32 invale = serialPort.Read(data, 0, datanum);
                // TryParse用変数
                Int32 check = 0;
                string inCuf = "";

                // 送信データ"1byte"ずつ格納
                for (int count = 0, i = 0; i < invale; i++)
                {
                    // Check有＆数値の場合
                    if ((bool)Receive_BinaryConvertCheck.IsChecked == true)
                    {
                        //vale[0] = (ushort)((data[1] << 8) + (data[2] & 0xff));
                        //vale[1] = (ushort)((data[3] << 8) + (data[4] & 0xff));
                    }
                    else
                    // Check無の場合、英数字　Stringへ変換
                    {
                        // 1byte格納
                        byte[] testbyte = new byte[1];
                        testbyte[0] = data[i];
                        // String型へ格納
                        inCuf = inCuf + System.Text.Encoding.ASCII.GetString(testbyte);
                    }

                }
                // 表示or書き込み関数

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /* 送信用関数 */
        public void Send(SerialPort serialPort, string sendString)
        {
            // 送信文字列のbyte数カウント
            int count = Encoding.GetEncoding("Shift_JIS").GetByteCount(sendString);
            // byte型送信用変数宣言
            byte[] outCuf = new byte[count * 2];
            // TryParse用変数
            Int32 check = 0;
            // loop用変数
            int sendcount = 0;
            int i = 0;

            // 送信データ"1byte"ずつ格納
            for (sendcount = 0, i = 0; i < count; i++)
            {
                // 1byte格納
                string testString = "";
                testString = testString + sendString[i];

                // Check有＆数値の場合
                if ((bool)Send_BinaryConvertCheck.IsChecked == true && Int32.TryParse(testString, out check) == true)
                {
                    outCuf[sendcount] = (byte)((Int32)Char.GetNumericValue(sendString[i]) >> 8);
                    outCuf[sendcount + 1] = (byte)((Int32)Char.GetNumericValue(sendString[i]) & 0xFF);
                    sendcount += 2;
                }
                else
                // Check無＆英字の場合byte型へ
                {
                    outCuf[sendcount] = (byte)sendString[i];
                    sendcount++;
                }
            }

            // 送信
            serialPort.Write(outCuf, 0, sendcount);
        }

    }
}
