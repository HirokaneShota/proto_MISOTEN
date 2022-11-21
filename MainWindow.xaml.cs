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

namespace MISOTEN_APPLICATION
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // 子画面を生成します。
            SystemSelect_Window window = new SystemSelect_Window();

            // 子画面を表示します。
            window.ShowDialog();

            // 子画面のオーナープロパティにこの画面を設定します。
            //window.Owner = GetWindow(this);

            // Mainwindowを閉じる
            this.Close();

        }
    }
}
