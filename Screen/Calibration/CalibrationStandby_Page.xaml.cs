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

namespace MISOTEN_APPLICATION.Screen.Calibration
{
    /// <summary>
    /// CalibrationStandby_Page.xaml の相互作用ロジック
    /// </summary>
    public partial class CalibrationStandby_Page : Page
    {
        public CalibrationStandby_Page()
        {
            InitializeComponent();
        }
        void PageLoad(object sender, RoutedEventArgs e)
        {
        }


        private void CalibrationStartButton_Click(object sender, RoutedEventArgs e)
        {
            // マスターキャリブレーション画面へ移行
            var mastercalibration__page = new MasterCalibration__Page();
            NavigationService.Navigate(mastercalibration__page);
        }
    }
}
