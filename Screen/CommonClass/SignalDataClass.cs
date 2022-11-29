using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISOTEN_APPLICATION.Screen.CommonClass
{

    /* 送信信号クラスオブジェクト */
    class SignalSendData
    {
        // 接続要請信号
        public const string MConnectRequest = "ct01";
        // 接続要請信号
        public const string MConnectComple = "cc01";
        // キャリブレーション完了信号
        public const string MCalibrationComple = "ce01";
        // センシング開始信号
        public const string MSensingStart = "cs01";
        // センシングリセット信号
        public const string MSensingReset = "cr01";
        // センシング終了信号
        public const string MSensingEnd = "ce01";
        // センシング停止信号
        public const string MSensingStop = "ch01";
    }

    /* 受信信号クラスオブジェクト */
    class SignalReceveData
    {
        // 接続確認信号
        public const string MConnectRequest = "ca10";
        // 接続完了信号
        public const string MConnectComple = "cc10";
    }
    /* 受信信号クラスオブジェクト */
    class SignalNumData
    {
        // 数値始め信号
        public const string SNumData  = "s";
        // 数値終わり信号
        public const string ENumData = "e";
    }

}
