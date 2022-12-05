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
    }
    /* 受信信号クラスオブジェクト */
    class SendNumSigna
    {
        // マスター数値始まり信号
        public const string MSData = "p";
        // 数値終わり信号
        public const string EData = "e";
    }

    /* 受信信号クラスオブジェクト */
    class ReceveSignal
    {
        // 接続確認信号時間経過
        public const string MConnectRequest = "ca10";
        // 接続完了信号
        public const string MConnectComple = "cc10";
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
