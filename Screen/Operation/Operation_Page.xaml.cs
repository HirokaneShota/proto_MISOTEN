using MISOTEN_APPLICATION.Screen.Calibration;
using MISOTEN_APPLICATION.Screen.SystemSelect;
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
    /// Operation_Page.xaml の相互作用ロジック
    /// </summary>
    public partial class Operation_Page : Page
    {
        public Operation_Page()
        {
            InitializeComponent();
        }

        void PageLoad(object sender, RoutedEventArgs e)
        {
        }

        private void EndButton_Click(object sender, RoutedEventArgs e)
        {
            // システム選択画面へ移行
            var systemselect_page = new SystemSelect_Page();
            NavigationService.Navigate(systemselect_page);
        }

        private void CalibrationButton_Click(object sender, RoutedEventArgs e)
        {
            // キャリブレーション準備画面へ移行
            var calibrationstandby_page = new CalibrationStandby_Page();
            NavigationService.Navigate(calibrationstandby_page);
        }
    }
}
