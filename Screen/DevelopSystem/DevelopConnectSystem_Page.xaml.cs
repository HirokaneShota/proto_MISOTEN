using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Markup;
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
        List<SerialPortData> product;
        // 受信データバインディング用class
        ReciveData_String RString = new ReciveData_String();

        public DevelopConnectSystem_Page()
        {
            InitializeComponent();
            ReceiveText.IsReadOnly = true;
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

        /* LogFileOpen */
        private void OpenLogFolderButton_Click(object sender, RoutedEventArgs e)
        {
            // LogFileOpen
            System.Diagnostics.Process.Start("explorer.exe", @"Log");
        }

        /* 接続処理 */
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {

            //　file読み込み
            string ResumeJson = File.ReadAllText("Json\\SerialPort.json");
            // JSONデータからオブジェクトを復元
            product = JsonSerializer.Deserialize<List<SerialPortData>>(ResumeJson);

            if ((bool)MasterConnectCheck.IsChecked == true)
            {
                // masterシリアルポートに接続
                try
                {
                    // ポートセット
                    MasterPort = SettingPort(MasterPort, product[DeviceId.MasterId]);
                    // ポートオープン
                    MasterPort.Open();
                    ReceiveText.AppendText("マスター接続開始\n");
                    File.WriteAllText(@"Log\DevelopMasterLog.txt", "マスター接続開始" + Environment.NewLine);

                    // 受信処理
                    MasterPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);


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
                    ReceiveText.AppendText("スレーブ接続開始\n");
                    // 受信処理
                    ReceiveProt.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
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
                    ReceiveText.AppendText("マスター切断\n");
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
                    ReceiveText.AppendText("スレーブ切断\n");
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
                    ReceiveText.AppendText("マスターへ送信\n");
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
                    ReceiveText.AppendText("レシーブへ送信\n");

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

            // 受信情報処理
            try
            {
                // 受信バッファ内のByte数
                Int32 datanum = serialPort.BytesToRead;
                // byte型受信用変数宣言
                byte[] data = new byte[datanum];
                // dataへdatanum数分格納
                Int32 invale = serialPort.Read(data, 0, datanum);
                string inCuf = "";
                // 数値格納変数
                ushort[] vale = new ushort[datanum];

                Dispatcher.Invoke((Action)(() =>
                {
                    // 送信データ"1byte"ずつ格納
                    for (int i = 0, j = 0; i < invale; i++, j++)
                    {
                        // 1byte格納
                        byte[] testbyte = new byte[1];
                        testbyte[0] = data[i];

                        // Check有＆数値の場合
                        if ((Receive_BinaryConvertCheck.IsChecked == true) && (Encoding.ASCII.GetString(testbyte) != "s" && Encoding.ASCII.GetString(testbyte) != "e"))
                        {
                            // 数値変換

                            // ※2byte続きで数値が入っている場合のみ
                            vale[j] = (ushort)((data[i] << 8) + (data[i++] & 0xff));
                            inCuf = inCuf + Convert.ToString(vale[j]);
                            i += 1;

                        }
                        else
                        // Check無の場合、英数字　Stringへ変換
                        {
                            // String型へ格納
                            inCuf = inCuf + System.Text.Encoding.ASCII.GetString(testbyte);
                        }

                    }
                }));

                //　受信データ格納
                RString.Port = serialPort.PortName;
                RString.ReciveData = inCuf;
                Received();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /* 受信処理 */
        public void Received()
        {

            Dispatcher.Invoke((Action)(() =>
            {

                if ((((bool)Master_TextCheck.IsChecked == true) || ((bool)Receive_TextCheck.IsChecked == true)))
                {
                    try
                    {
                        // ディスプレイ表示
                        ReciiveDisplay(RString.Port, RString.ReciveData);
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message);
                    }
                }
                if (((bool)Master_FileCheck.IsChecked == true) || ((bool)Receive_FileCheck.IsChecked == true))
                {
                    try
                    {
                        // File書き込み
                        ReciiveFile(RString.Port, RString.ReciveData);
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message);
                    }
                }
            }));
        }
        /* 受信情報表示処理 */
        public void ReciiveDisplay(string Port, string displayString)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                if (Port == product[DeviceId.MasterId].comName)
                {
                    ReceiveText.AppendText("マスター : ");
                }
                else if (Port == product[DeviceId.ReceiveId].comName)
                {
                    ReceiveText.AppendText("レシーブ : ");
                }
                ReceiveText.AppendText(displayString + "\n");

                //表示制限

                if (ReceiveText.LineCount == 20)
                {
                    ReceiveText.Text = "";
                }
            }));
        }
        /* 受信情報File書き込み処理 */
        public void ReciiveFile(string Port, string displayString)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                if (Port == product[DeviceId.MasterId].comName)
                {
                    File.AppendAllText(@"Log\DevelopMasterLog.txt", "マスター : " + displayString + Environment.NewLine);
                }
                else if (Port == product[DeviceId.ReceiveId].comName)
                {
                }
            }));

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
