using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using MISOTEN_APPLICATION.Screen.CommonClass;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Newtonsoft.Json;

namespace MISOTEN_APPLICATION.Screen.SystemSetting
{
    /// <summary>
    /// ComSetting_Page.xaml の相互作用ロジック
    /// </summary>
    /// 
    public partial class ComSetting_Page : Page
    {
        Window Owner;
        SerialPortData master = new SerialPortData { id = DeviceId.MasterId };
        SerialPortData receive = new SerialPortData { id = DeviceId.ReceiveId };

        /* シリアルポートBPS　クラスオブジェクト */
        public class Customer
        {
            public string BPSName { get; set; }
            public int BPS { get; set; }
        }
        /* シリアルポートName　クラスオブジェクト */
        public class ComPort
        {
            public string DeviceID { get; set; }
            public string Description { get; set; }
        }

        public ComSetting_Page(Window owner)
        {
            InitializeComponent();
            //オブジェクト
            Owner = owner;
            // ポート表示
            SelPort(MasterPartCmb);
            SelPort(ReceivePartCmb);
            BPSPort(MasterBPSCmb);
            BPSPort(ReceiveBPSCmb);

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

            BpsList.Add(new Customer { BPSName = "115200", BPS = 115200 });
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
            master.comName = port;
        }
        /* レシーブCOM選択Combbox */
        private void ReceivePartCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var port = ReceivePartCmb.SelectedValue.ToString();
            receive.comName = port;
        }
        /* マスターBPS選択Combbox */
        private void MasterBPSCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int bps = (int)MasterBPSCmb.SelectedValue;
            master.baudRate = bps;
        }
        /* レシーブBPS選択Combbox */
        private void ReceiveBPSCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int bps = (int)ReceiveBPSCmb.SelectedValue;
            receive.baudRate = bps;
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            /* デバイス */
            List<SerialPortData> JsonList = new List<SerialPortData>();
            // 0番目要素追加
            JsonList.Insert(DeviceId.MasterId, master);
            // 1番目要素追加
            JsonList.Insert(DeviceId.ReceiveId, receive);
            string jsonStr = JsonSerializer.Serialize(JsonList);
            //string jsonStr = JsonConvert.SerializeObject(JsonList, Formatting.Indented);

            File.WriteAllText(@"..\..\Log\SerialPort.json", jsonStr);
            Owner.Close();
        }
    }
}
