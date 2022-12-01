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
        ArgSignal argSignal = new ArgSignal();
        public SystemSelect_Page(ArgSignal argsignal)
        {
            InitializeComponent();
        }

        /* 機体との接続開始 */
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            // マスター:"se01" 送信 : センシング終了信号
            argSignal.Msignalclass.SignalSend(argSignal.Masterport, SendSignal.MSensingEnd);
            ProtCut(argSignal.Masterport);

            SignalConnect_Page signalconnect_page = new SignalConnect_Page();
            NavigationService.Navigate(signalconnect_page);
        }

         /* キャリブレーション */
        private void CalibrationButton_Click(object sender, RoutedEventArgs e)
        {
            // マスター:"sr01" 送信 : センシングリセット信号
            argSignal.Msignalclass.SignalSend(argSignal.Masterport, SendSignal.MSensingReset);

            // キャリブレーション準備画面へ移行
            var calibrationstandby_page = new CalibrationStandby_Page(argSignal);
            NavigationService.Navigate(calibrationstandby_page);
        }

         /* システム終了 */
        private void EndButton_Click(object sender, RoutedEventArgs e)
        {
            // マスター:"se01" 送信 : センシング終了信号
            argSignal.Msignalclass.SignalSend(argSignal.Masterport, SendSignal.MSensingEnd);
            ProtCut(argSignal.Masterport);

            Window.GetWindow(this).Close();
        }

        /* 開発用システム */
        private void DevelopSystemButton_Click(object sender, RoutedEventArgs e)
        {
            // ポート切断
            // ProtCut(argSignal.Masterport);
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

        /* Port切断処理 */
        private void ProtCut(SerialPort serialPort)
        {
            if (serialPort.IsOpen == false) return;
            // ポート切断
            serialPort.Close();
        }

    }
}
