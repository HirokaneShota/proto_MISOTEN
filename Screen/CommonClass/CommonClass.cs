using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISOTEN_APPLICATION.Screen.CommonClass
{

    /* シリアルポートクラスオブジェクト */
    class SerialPortData
    {
        // デバイスid
        public int id { get; set; } = 0;
        public string comName { get; set; } = "";
        public int baudRate { get; set; } = 0;
        public int dataBits { get; set; } = 8;
        public string parity { get; set; } = "Parity.None";
        public string stopBits { get; set; } = "StopBits.One";
        public string encoding { get; set; } = "Encoding.UTF8";
        public int writeTimeout { get; set; } = 100000;
    }
    class DeviceId
    {
        // マスターID
        public const int MasterId = 0;
        // レシーブID
        public const int ReceiveId = 1;
    }

    class Retrun
    {
        // マスターID
        public const int True = 0;
        // レシーブID
        public const int False = 1;
    }

    class Time
    {
        // 画面遷移時間 3秒
        public const int ScreenTrans = 3;
        // マスターキャリブレーション　伸ばす
        public const int MECalibration = 10;
        // マスターキャリブレーション　握る
        public const int MGCalibration = 10;
        // スレーブキャリブレーション　伸ばす
        public const int SECalibration = 10;
        // スレーブキャリブレーション　握る
        public const int SGCalibration = 10;
    }

    class Flog
    {
        // 受信していない
        public const int RNo = 0;
        // 信号受信
        public const int RSignal = 1;
        // センサー値受信
        public const int RNum = 2;
    }

    /* Uri */
    class URI
    {
        // マスター用LogFile
        public const string MasterLog = "Log\\MasterLog.txt";
        // マスター用LogFile
        public const string ReceiveLog = "Log\\ReceiveLog.txt";
        // ロゴ画像
        public const string LogoImage = "../../image/ROGO.png";
    }
}
