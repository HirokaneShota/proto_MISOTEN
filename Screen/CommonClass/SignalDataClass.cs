using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISOTEN_APPLICATION.Screen.CommonClass
{

    /* 送信信号クラスオブジェクト */
    class SendSignal
    {
        //
        // マスター信号
        //

        // 接続要請信号
        public const string MConnectRequest = "ct01";
        // 接続要請信号
        public const string MConnectComple = "cc01";
        // キャリブレーションスタート信号
        public const string MCalibrationStart = "cs01";
        // キャリブレーション完了信号
        public const string MCalibrationComple = "ce01";
        // センシング開始信号
        public const string MSensingStart = "ss01";
        // センシングリセット信号
        public const string MSensingReset = "sr01";
        // センシング終了信号
        public const string MSensingEnd = "se01";
        // センシング停止信号
        public const string MSensingStop = "sh01";
        // 初期化信号
        public const string MInit = "re01";

        //
        // スレーブ信号
        //

        // 接続要請信号
        public const string SConnectRequest = "ct02";
        // 接続要請信号
        public const string SConnectComple = "cc02";
        // キャリブレーションスタート信号
        public const string SCalibrationStart = "cs02";
        // キャリブレーション完了信号
        public const string SCalibrationComple = "ce02";
        // センシング開始信号
        public const string SSensingStart = "ss02";
        // センシングリセット信号
        public const string SSensingReset = "sr02";
        // センシング終了信号
        public const string SSensingEnd = "se02";
        // センシング停止信号
        public const string SSensingStop = "sh02";
        // 初期化信号
        public const string SInit = "re02";
    }
    /* 受信信号クラスオブジェクト */
    class SendNumSigna
    {
        // 始まり信号 1
        public const string MSData1 = "a";
        // 始まり信号 2
        public const string MSData2 = "b";
        // 数値終わり信号
        public const string EData = "1";
    }

    /* 受信信号クラスオブジェクト */
    class ReceveSignal
    {
        //
        // マスター信号
        //

        // 接続確認信号時間経過
        public const string MConnectRequest = "ca10";
        // 接続完了信号
        public const string MConnectComple = "cc10";

        //
        // スレーブ信号
        //

        // 接続確認信号時間経過
        public const string SConnectRequest = "ca20";
        // 接続完了信号
        public const string SConnectComple = "cc20";
    }
    /* 受信信号クラスオブジェクト */
    class ReceveNumSignal
    {
        // マスター数値始まり信号
        public const string MSData = "m";
        // スレーブ数値始まり信号
        public const string SSData = "s";
        // 数値終わり信号 1
        public const char EData1 = '1';
        // 数値終わり信号 2
        public const char EData2 = '2';
        // 数値終わり信号 3
        public const char EData3 = '3';
        // 数値終わり信号 4
        public const char EData4 = '4';
        // 受信値最終信号
        public const string End = "z";
    }

}
