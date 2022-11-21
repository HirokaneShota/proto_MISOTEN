using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MISOTEN_APPLICATION.Screen.SystemSetting
{
    /// <summary>
    /// ComSetting_Page.xaml の相互作用ロジック
    /// </summary>
    /// 
    public partial class ComSetting_Page : Page
    {
        public class Customer
        {
            public string BPSName { get; set; }
            public int BPS { get; set; }
        }
        public class ComPort
        {
            public string DeviceID { get; set; }
            public string Description { get; set; }
        }
        /// <summary>
        /// 編集した入力値を呼び出し元に返すためのプロパティ
        /// </summary>
        // public string Text { get { return uxImputText.Text; } set { uxImputText.Text = value; } }

        /*
        public class PortStr
        {
            public string MasterComName { get; set; }
            public string ReceiveComName { get; set; }
            public int MasterBPSNum { get; set; }
            public int ReceiveBPSNum { get; set; }
        }*/
        System.IO.Ports.SerialPort MasterSerialPort = null;
        System.IO.Ports.SerialPort ReceiveSerialPort = null;

        public ComSetting_Page()
        {
            InitializeComponent();
            // ポート表示設定
            SelPort(MasterPartCmb);
            SelPort(ReceivePartCmb);
            BPSPort(MasterBPSCmb);
            BPSPort(ReceiveBPSCmb);

            // 設定ポート
            MasterSerialPort = SettingPort();
            ReceiveSerialPort = SettingPort();
        }

        //ポートセット
        public SerialPort SettingPort()
        {
            SerialPort serialPort = null;
            // まだポートに繋がっていない場合
            if (serialPort == null)
            {
                // serialPortの設定
                serialPort = new SerialPort();
                serialPort.BaudRate = 115200;
                serialPort.DataBits = 8;
                serialPort.Parity = Parity.None;
                serialPort.StopBits = StopBits.One;
                serialPort.Encoding = Encoding.UTF8;
                serialPort.WriteTimeout = 100000;
            }
            return serialPort;
        }

        /*----------------------------
        * シリアルポート列挙
        *---------------------------*/
        private void SelPort(ComboBox ComName)
        {
            // シリアルポートの列挙
            string[] PortList = SerialPort.GetPortNames();
            var MyList = new ObservableCollection<ComPort>();
            foreach (string p in PortList)
            {
                System.Console.WriteLine(p);
                MyList.Add(new ComPort { DeviceID = p, Description = p });
            }
            ComName.ItemsSource = MyList;
            ComName.SelectedValuePath = "DeviceID";
            ComName.DisplayMemberPath = "Description";
        }

        /*----------------------------
        * BPSポート列挙
        *---------------------------*/
        private void BPSPort(ComboBox ComName)
        {
            // シリアルポートの列挙
            var BpsList = new ObservableCollection<Customer>();

            BpsList.Add(new Customer { BPSName = "115200", BPS = 115200});
            BpsList.Add(new Customer { BPSName = "57600", BPS = 57600 });
            BpsList.Add(new Customer { BPSName = "9600", BPS = 9600 });

            ComName.ItemsSource = BpsList;
            ComName.SelectedValuePath = "BPS";
            ComName.DisplayMemberPath = "BPSName";
        }

        /* マスターCOM選択Combbox */
        private void MasterPartCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var port = MasterPartCmb.SelectedValue.ToString();
            MasterSerialPort.PortName = port;
        }
        /* レシーブCOM選択Combbox */
        private void ReceivePartCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var port = ReceivePartCmb.SelectedValue.ToString();
            ReceiveSerialPort.PortName = port;
        }
        /* マスターBPS選択Combbox */
        private void MasterBPSCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int bps = (int)MasterBPSCmb.SelectedValue;
            MasterSerialPort.BaudRate = bps;
        }
        /* レシーブBPS選択Combbox */
        private void ReceiveBPSCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int bps = (int)ReceiveBPSCmb.SelectedValue;
            ReceiveSerialPort.BaudRate = bps;
        }

    }
}
