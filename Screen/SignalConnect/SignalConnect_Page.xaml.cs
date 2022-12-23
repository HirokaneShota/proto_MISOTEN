using MISOTEN_APPLICATION.BackProcess;
using MISOTEN_APPLICATION.Screen.CommonClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.Json;
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

namespace MISOTEN_APPLICATION.Screen.SignalConnect
{
    /// <summary>
    /// SignalConnect_Page.xaml の相互作用ロジック
    /// </summary>
    public partial class SignalConnect_Page : Page
    {
        // エラー文
        string ErrorSentence = "";

        // 送受信クラス
        SignalClass signalclass = new SignalClass();
        // 表示用クラス
        FileClass file = new FileClass();

        public SignalConnect_Page()
        {
            InitializeComponent();
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {

            Task MeasurementTask = Task.Run(() => { Window_Load(); });
        }

        private void Window_Load()
        {
            // 接続処理
            if ((ProtConnect() == Retrun.True) &&
                // 送受信(接続要請信号送信,接続確認信号受信)
                (ProtReceve() == Retrun.True))
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    // 接続完了
                    var SignalConnectComp = new SignalConnectComp_Page(signalclass);
                    NavigationService.Navigate(SignalConnectComp);
                }));
            }
            else
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    // 再接続
                    var SignalReConnect = new SignalReConnect_Page(signalclass, ErrorSentence);
                    NavigationService.Navigate(SignalReConnect);
                }));
            }
        }

        /* ポート接続処理 */
        private int ProtConnect()
        {
            Dispatcher.Invoke((Action)(() => { Execution.Content = "JSONファイル読み込み中..."; }));

            try
            {
                // ポートセット&オープン&ハンドラ
                if ((ErrorSentence = signalclass.SetSerialport(DeviceId.MasterId)) != "") { return Retrun.False; }
                if ((ErrorSentence = signalclass.SetSerialport(DeviceId.ReceiveId)) != "") { return Retrun.False; }

                // ファイル書き込み
                file.MFirst();
                file.SFirst();

                Dispatcher.Invoke((Action)(() => { Execution.Content = "受信バンドラ立ち上げ中..." + " ( 1秒 )"; }));
                TimerClass.Sleep(1000);

                return Retrun.True;
            }
            catch (Exception ex)
            {
                ErrorSentence = ex.Message;
                return Retrun.False;
            }
        }

        /* ポート受信処理 */
        private int ProtReceve()
        {
            // マスター受信値
            ReciveData MReciveData = new ReciveData();
            // スレーブ受信値
            ReciveData SReciveData = new ReciveData();

            //
            // 「初期化信号」
            //

            // "re01" 送信
            //signalclass.SignalSend(DeviceId.MasterId, SendSignal.MInit);
            // "re02" 送信
            //signalclass.SignalSend(DeviceId.ReceiveId, SendSignal.SInit);

            //
            // 「マスター：接続要請信号」
            //
            Task<ReciveData> MRtask = Task.Run(() => { return signalclass.GetMReciveData(); });
            Dispatcher.Invoke((Action)(() => { Execution.Content = "マスターから 接続確認信号(ca10) 受信待機中"; }));
            // "ct01" 送信
            signalclass.SignalSend(DeviceId.MasterId, SendSignal.MConnectRequest);
            // 計測開始("ct01" 送信から"ca10"受信まで)
            // 受信タスク終了まで待機
            MReciveData = MRtask.Result;
            file.MLog(MReciveData.RSignal);
            // 受信値チェック
            if (MReciveData.RSignal != ReceveSignal.MConnectRequest)
            {
                ErrorSentence = MReciveData.RSignal;
                return Retrun.False;
            }

            //
            // 「マスター：接続完了信号」
            //
            MRtask = Task.Run(() => { return signalclass.GetMReciveData(); });
            Dispatcher.Invoke((Action)(() => { Execution.Content = "マスターから 接続完了信号(cc10) 受信待機中"; }));
            // "cc01" 送信
            signalclass.SignalSend(DeviceId.MasterId, SendSignal.MConnectComple);
            // 計測開始("cc01" 送信から"cc10"受信まで)
            // 受信タスク終了まで待機
            MReciveData = MRtask.Result;
            file.MLog(MReciveData.RSignal);
            // 受信値チェック
            if (MReciveData.RSignal != ReceveSignal.MConnectComple)
            {
                ErrorSentence = MReciveData.RSignal;
                return Retrun.False;
            }

            //
            // 「スレーブ：接続要請信号」
            //
            
            // バッファ内削除
            signalclass.ReceiveClearBuffer(DeviceId.ReceiveId);
            Task<ReciveData> SRtask = Task.Run(() => { return signalclass.GetSReciveData(); });
            Dispatcher.Invoke((Action)(() => { Execution.Content = "スレーブから 接続確認信号(ct20) 受信待機中"; }));
            // "ct02" 送信
            signalclass.SignalSend(DeviceId.ReceiveId, SendSignal.SConnectRequest);
            // 計測開始("ct02" 送信から"ca20"受信まで)
            // 受信タスク終了まで待機
            SReciveData = SRtask.Result;
            file.SLog(SReciveData.RSignal);
            // 受信値チェック
            if (SReciveData.RSignal != ReceveSignal.SConnectRequest)
            {
                ErrorSentence = SReciveData.RSignal;
                return Retrun.False;
            }

            //
            // 「スレーブ：接続完了信号」
            //

            SRtask = Task.Run(() => { return signalclass.GetSReciveData(); });
            Dispatcher.Invoke((Action)(() => { Execution.Content = "スレーブから 接続完了信号(cc20) 受信待機中"; }));
            // "cc02" 送信
            signalclass.SignalSend(DeviceId.ReceiveId, SendSignal.SConnectComple);
            // 計測開始("cc02" 送信から"cc20"受信まで)
            // 受信タスク終了まで待機
            SReciveData = SRtask.Result;
            file.SLog(SReciveData.RSignal);
            // 受信値チェック
            if (SReciveData.RSignal != ReceveSignal.SConnectComple)
            {
                ErrorSentence = SReciveData.RSignal;
                return Retrun.False;
            }
            return Retrun.True;
        }
    }
}
