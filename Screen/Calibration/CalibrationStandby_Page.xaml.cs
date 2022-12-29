using MISOTEN_APPLICATION.BackProcess;
using MISOTEN_APPLICATION.Screen.CommonClass;
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

namespace MISOTEN_APPLICATION.Screen.Calibration
{
    /// <summary>
    /// CalibrationStandby_Page.xaml の相互作用ロジック
    /// </summary>
    public partial class CalibrationStandby_Page : Page
    {
        SignalClass Signalclass = new SignalClass();
        // ゴッドハンド実体化
        GodHand ggodhand = new GodHand();
        public CalibrationStandby_Page(SignalClass signalclass)
        {
            InitializeComponent();
            Signalclass = signalclass;

        }
        public CalibrationStandby_Page(SignalClass signalclass, GodHand _godhand)
        {
            InitializeComponent();
            Signalclass = signalclass;
            ggodhand = _godhand;

        }
        void PageLoad(object sender, RoutedEventArgs e)
        {
            SlaveStart.Visibility = Visibility.Hidden;
            MasterStart.Visibility = Visibility.Hidden;
            CalibrationStartButton.Visibility = Visibility.Hidden;
        }

        /* ログボタン */
        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            SlaveStart.Visibility = Visibility.Visible;
            MasterStart.Visibility = Visibility.Visible;
            RealTime.Visibility = Visibility.Hidden;
            Log.Visibility = Visibility.Hidden;
        }

        /* リアルタイムボタン */
        private void RealTimeButton_Click(object sender, RoutedEventArgs e)
        {
            CalibrationStartButton.Visibility = Visibility.Visible;
            RealTime.Visibility = Visibility.Hidden;
            Log.Visibility = Visibility.Hidden;
        }

        /* リアルタイム開始ボタン */
        private void CalibrationStartButton_Click(object sender, RoutedEventArgs e)
        {
            // キャリブレーション画面へ移行
            var calibration_page = new Calibration_Page(Signalclass,Flog.RialON, ggodhand);
            NavigationService.Navigate(calibration_page);
        }

        /* スレーブ開始ボタン */
        private void SlaveStartButton_Click(object sender, RoutedEventArgs e)
        {
            // キャリブレーション画面へ移行
            var calibration_page = new Calibration_Page(Signalclass, Flog.SLogON, ggodhand);
            NavigationService.Navigate(calibration_page);
        }

        /* マスター開始ボタン */
        private void MasterStartButton_Click(object sender, RoutedEventArgs e)
        {
            // キャリブレーション画面へ移行
            var calibration_page = new Calibration_Page(Signalclass, Flog.MLogON, ggodhand);
            NavigationService.Navigate(calibration_page);
        }
    }
}
