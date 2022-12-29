using MISOTEN_APPLICATION.BackProcess;
using MISOTEN_APPLICATION.Screen.Calibration;
using MISOTEN_APPLICATION.Screen.CommonClass;
using MISOTEN_APPLICATION.Screen.DevelopSystem;
using MISOTEN_APPLICATION.Screen.SignalConnect;
using MISOTEN_APPLICATION.Screen.SystemSetting;
using System;
using System.Collections.Generic;
using System.IO.Ports;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MISOTEN_APPLICATION.Screen.SystemSelect
{
    /// <summary>
    /// SystemSelect_Page.xaml の相互作用ロジック
    /// </summary>
    public partial class SystemSelect_Page : Page
    {
        SignalClass Signalclass = new SignalClass();
        GodHand ggodhand = new GodHand();
        public SystemSelect_Page(SignalClass signalclass,GodHand godhand)
        {
            Signalclass = signalclass;
            ggodhand = godhand;
            InitializeComponent();
        }

        /* 機体との接続開始 */
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            // マスター:"se01" 送信 : センシング終了信号
            Signalclass.SignalSend(DeviceId.MasterId, SendSignal.MSensingEnd);
            Signalclass.ProtCut(DeviceId.MasterId);

            // スレーブ:"se02" 送信 : センシング終了信号
            Signalclass.SignalSend(DeviceId.ReceiveId, SendSignal.SSensingEnd);
            Signalclass.ProtCut(DeviceId.ReceiveId);

            SignalConnect_Page signalconnect_page = new SignalConnect_Page();
            NavigationService.Navigate(signalconnect_page);
        }

         /* キャリブレーション */
        private void CalibrationButton_Click(object sender, RoutedEventArgs e)
        {
            
            // マスター:"sr01" 送信 : センシングリセット信号
            Signalclass.SignalSend(DeviceId.MasterId, SendSignal.MSensingReset);
            // スレーブ:"sr02" 送信 : センシングリセット信号
            Signalclass.SignalSend(DeviceId.ReceiveId, SendSignal.SSensingReset);
            
            // キャリブレーション準備画面へ移行
            var calibrationstandby_page = new CalibrationStandby_Page(Signalclass, ggodhand);
            NavigationService.Navigate(calibrationstandby_page);
        }

         /* システム終了 */
        private void EndButton_Click(object sender, RoutedEventArgs e)
        {
            // マスター:"se01" 送信 : センシング終了信号
            Signalclass.SignalSend(DeviceId.MasterId, SendSignal.MSensingEnd);
            Signalclass.ProtCut(DeviceId.MasterId);
            // スレーブ:"se02" 送信 : センシング終了信号
            Signalclass.SignalSend(DeviceId.ReceiveId, SendSignal.SSensingEnd);
            Signalclass.ProtCut(DeviceId.ReceiveId);

            Window.GetWindow(this).Close();
        }

        /* 開発用システム */
        private void DevelopSystemButton_Click(object sender, RoutedEventArgs e)
        {
            // 子画面を生成します。
            DevelopSystem_Window window = new DevelopSystem_Window();
            window.ShowDialog();
        }

        /* Com設定 */
        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            // 子画面を生成します。
            SystemSetting_Window window = new SystemSetting_Window();
            window.ShowDialog();
        }

    }
}
