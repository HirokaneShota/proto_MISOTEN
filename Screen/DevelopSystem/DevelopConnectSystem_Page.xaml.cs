using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
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
using MISOTEN_APPLICATION.BackProcess;
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
        SerialPort SlaveProt;
        List<SerialPortData> product;
        SignalClass signalclass = new SignalClass();
        FileClass file = new FileClass();
        // 排他制御に使用するオブジェクト
        private static Object lockObject = new Object();

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
                    file.DMFirst();
                    file.DMFirst_csv();
                    // signalクラスでポートセット＆ハンドラ接続
                    signalclass.SetSerialport(MasterPort, DeviceId.MasterId);
                    // 受信待機＆表示・書き込み
                    Task ReceveTask = Task.Run(() => { MReceived(); });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    MessageBox.Show(ex.StackTrace);
                }
            }
            if ((bool)ReceiveConnectCheck.IsChecked == true)
            {
                // receveシリアルポートに接続
                try
                {
                    // ポートセット
                    SlaveProt = SettingPort(SlaveProt, product[DeviceId.ReceiveId]);
                    // ポートオープン
                    SlaveProt.Open();
                    ReceiveText.AppendText("スレーブ接続開始\n");
                    file.DSFirst();
                    file.DSFirst_csv();
                    // signalクラスでポートセット＆ハンドラ接続
                    signalclass.SetSerialport(SlaveProt, DeviceId.ReceiveId);
                    // 受信待機＆表示・書き込み
                    Task ReceveTask = Task.Run(() => { SReceived(); });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    MessageBox.Show(ex.StackTrace);
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
                    signalclass.ReceiveClearBuffer(DeviceId.MasterId);
                    MasterPort.Close();
                    MasterPort = null;
                    ReceiveText.AppendText("マスター切断\n");
                    file.MDLog("マスター切断");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    MessageBox.Show(ex.StackTrace);
                }
            }
            if ((bool)ReceiveConnectCheck.IsChecked == true)
            {
                // receveシリアルポート切断
                try
                {
                    if (SlaveProt.IsOpen == false) return;
                    // ポート切断
                    signalclass.ReceiveClearBuffer(DeviceId.ReceiveId);
                    SlaveProt.Close();
                    ReceiveText.AppendText("スレーブ切断\n");
                    file.MDLog("スレーブ切断");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    MessageBox.Show(ex.StackTrace);
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
                    MessageBox.Show(ex.StackTrace);
                }
            }
            if ((bool)ReceiveSendCheck.IsChecked == true)
            {
                try
                {
                    if (SlaveProt == null) return;
                    if (SlaveProt.IsOpen == false) return;
                    // 送信処理
                    Send(SlaveProt, SendText.Text);
                    ReceiveText.AppendText("レシーブへ送信\n");

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    MessageBox.Show(ex.StackTrace);
                }
            }
        }

        /* Master受信処理 */
        public void MReceived()
        {
            while ((MasterPort != null) && (MasterPort.IsOpen == true))
            {
                ReciveData rdata = signalclass.GetMReciveData();
                Dispatcher.Invoke((Action)(() =>
                {
                    lock (lockObject)
                    {
                        if ((((bool)Master_TextCheck.IsChecked == true)))
                        {
                            try
                            {
                                if (rdata.RFlog == Flog.RSignal)
                                {
                                    // ディスプレイ表示
                                    ReciiveDisplay(DeviceId.MasterId, rdata);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                MessageBox.Show(ex.StackTrace);
                            }
                        }
                        if (((bool)Master_FileCheck.IsChecked == true))
                        {
                            try
                            {
                                // File書き込み
                                ReciiveFile(DeviceId.MasterId, rdata);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                MessageBox.Show(ex.StackTrace);
                            }
                        }
                        if (((bool)Master_CSVFileCheck.IsChecked == true) && (rdata.RFlog == Flog.RNum))
                        {
                            file.MDLog_csv(rdata.RSensor);
                        }
                    }
                }));
            }
        }
        /* Slave受信処理 */
        public void SReceived()
        {
            while ((SlaveProt.IsOpen == true) && (SlaveProt != null))
            {
                // 受信待機
                ReciveData rdata = signalclass.GetSReciveData();
                Dispatcher.Invoke((Action)(() =>
                {
                    lock (lockObject)
                    {
                        if (((bool)Receive_TextCheck.IsChecked == true))
                        {
                            try
                            {
                                // ディスプレイ表示
                                //ReciiveDisplay(DeviceId.ReceiveId, rdata);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                MessageBox.Show(ex.StackTrace);
                            }
                        }
                        if (((bool)Receive_FileCheck.IsChecked == true))
                        {
                            try
                            {
                                // File書き込み
                                ReciiveFile(DeviceId.ReceiveId, rdata);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                MessageBox.Show(ex.StackTrace);
                            }
                        }
                        if (((bool)Slave_CSVFileCheck.IsChecked == true) && (rdata.RFlog == Flog.RNum))
                        {
                            file.SDLog_csv(rdata.RSensor);
                        }
                    }
                }));
            }
        }

        /* 受信情報表示処理 */
        public void ReciiveDisplay(int id, ReciveData data)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                if (id == DeviceId.MasterId)
                {
                    ReceiveText.AppendText("マスター : ");
                }
                else if (id == DeviceId.ReceiveId)
                {
                    ReceiveText.AppendText("レシーブ : ");
                }

                // 入力内容 信号
                if (data.RFlog == Flog.RSignal)
                {
                    ReceiveText.AppendText(data.RSignal + "\n");
                }
                // 入力内容 数値
                else if (data.RFlog == Flog.RNum)
                {
                    ReceiveText.AppendText("数値" + "\n");
                }

                //表示制限

                if (ReceiveText.LineCount == 20)
                {
                    ReceiveText.Text = "";
                }
            }));
        }
        /* 受信情報File書き込み処理 */
        public void ReciiveFile(int id, ReciveData data)
        {
            if (id == DeviceId.MasterId)
            {
                // 入力内容 信号
                if (data.RFlog == Flog.RSignal)
                {
                    file.MDLog(data.RSignal);
                }
                // 入力内容 数値
                else if (data.RFlog == Flog.RNum)
                {
                    file.MDLog("数値");
                }
            }
            else if (id == DeviceId.ReceiveId)
            {
                // 入力内容 信号
                if (data.RFlog == Flog.RSignal)
                {
                    file.SDLog(data.RSignal);
                }
                // 入力内容 数値
                else if (data.RFlog == Flog.RNum)
                {
                    file.SDLog("数値");
                }

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

        //
        // モータタブ
        //

        /* 入力値制限 */
        private void mottor1_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        private void mottor2_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        private void mottor3_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        private void mottor4_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        private void mottor5_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = !new Regex("[0-9]").IsMatch(e.Text);

        /* pwm値送信 */
        private void PWMSendButton_Click(object sender, RoutedEventArgs e)
        {
            GODS_SENTENCE sendData = new GODS_SENTENCE();
            sendData.frist_godsentence.palm_pwm = int.Parse(mottor1.Text);
            sendData.second_godsentence.palm_pwm = int.Parse(mottor2.Text);
            sendData.third_godsentence.palm_pwm = int.Parse(mottor3.Text);
            sendData.fourth_godsentence.palm_pwm = int.Parse(mottor4.Text);
            sendData.fifth_godsentence.palm_pwm = int.Parse(mottor5.Text);

            signalclass.SetSendMotor(sendData);
        }
    }
}
