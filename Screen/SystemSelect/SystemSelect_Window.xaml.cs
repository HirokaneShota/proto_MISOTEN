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
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using MISOTEN_APPLICATION.Screen.DevelopSystem;
using MISOTEN_APPLICATION.Screen.SystemSetting;
using System.Windows.Navigation;
using MISOTEN_APPLICATION.Screen.SignalConnect;
using MISOTEN_APPLICATION.BackProcess;
using MISOTEN_APPLICATION.Screen.CommonClass;

namespace MISOTEN_APPLICATION.Screen.SystemSelect
{
    /// <summary>
    /// SystemSelect_Window.xaml の相互作用ロジック
    /// </summary>
    public partial class SystemSelect_Window : NavigationWindow
    {
        public SystemSelect_Window()
        {
            InitializeComponent();
            SignalConnectStandby_Page signalconnect_page = new SignalConnectStandby_Page();
            Navigate(signalconnect_page);
        }
        protected virtual void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SignalClass signalclass = new SignalClass();
            if ((signalclass.SetSerialport(DeviceId.MasterId)) == "")
            {
                // マスター:"re01" 送信
                signalclass.SignalSend(DeviceId.MasterId, SendSignal.MInit);
                signalclass.ProtCut(DeviceId.MasterId);
            }
            if ((signalclass.SetSerialport(DeviceId.ReceiveId)) == "")
            {   
                // スレーブ:"re02" 送信
                signalclass.SignalSend(DeviceId.ReceiveId, SendSignal.SInit);
                signalclass.ProtCut(DeviceId.ReceiveId);
            }
        }
    }
}
