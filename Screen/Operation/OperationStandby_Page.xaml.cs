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
        ArgSignal argSignal = new ArgSignal();
        public OperationStandby_Page(ArgSignal argsignal)
        {
            InitializeComponent();
            argSignal = argsignal;
        }

        void PageLoad(object sender, RoutedEventArgs e)
        {
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // 稼働ページへ移行
            var operation_page = new Operation_Page(argSignal);
            NavigationService.Navigate(operation_page);
        }
    }
}
