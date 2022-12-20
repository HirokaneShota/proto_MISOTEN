using MISOTEN_APPLICATION.BackProcess;
using MISOTEN_APPLICATION.Screen.Calibration;
using MISOTEN_APPLICATION.Screen.CommonClass;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MISOTEN_APPLICATION.Screen.Operation
{
    /// <summary>
    /// OperationStandby_Page.xaml の相互作用ロジック
    /// </summary>
    public partial class OperationStandby_Page : Page
    {
        SignalClass Signalclass = new SignalClass();
        public OperationStandby_Page(SignalClass signalclass)
        {
            InitializeComponent();
            Signalclass = signalclass;
        }
        void PageLoad(object sender, RoutedEventArgs e)
        {

        }

        private void LogStartButton_Click(object sender, RoutedEventArgs e)
        {
            // 稼働ページへ移行
            var operation_page = new Operation_Page(Signalclass , Flog.LogON);
            NavigationService.Navigate(operation_page);
        }

        private void RealTimeStartButton_Click(object sender, RoutedEventArgs e)
        {
            // 稼働ページへ移行
            var operation_page = new Operation_Page(Signalclass, Flog.RialON);
            NavigationService.Navigate(operation_page);
        }
    }
}
