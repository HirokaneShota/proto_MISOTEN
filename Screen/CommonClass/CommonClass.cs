using MISOTEN_APPLICATION.BackProcess;
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
    class ReceiveNum
    {
        // 受信最大数 :14byte
        public const int MaxNum = 14;
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

        //センサー値取得許可
        public const int SON = 1;
        //センサー値取得不可
        public const int SOFF = 0;

        // 処理開始
        public const int Start = 1;
        // 処理終了
        public const int End = 2;

        // キャリブレーション : 処理中
        public const int CalibNone = 99;

        // キャリブレーション : 処理中
        public const int CalibPush = 2;

        // キャリブレーション : 手を開く
        public const int CalibOpen = 0;

        // キャリブレーション : 手を閉じる
        public const int CalibClose = 1;
    }

    /* Uri */
    class URI
    {
        // COMデータJSON
        public const string ComJson = "Json\\SerialPort.json";
        // JSONフォルダ
        public const string Json = "Json\\";
        // マスター用LogFile
        public const string MasterLog = "Log\\MasterLog.txt";
        // スレーブ用LogFile
        public const string ReceiveLog = "Log\\ReceiveLog.txt";
        // LogFolder
        public const string LogFolder = "Log\\";
        // マスター用LogFile(開発者用)
        public const string DMasterLog = "Log\\DevelopMasterLog.txt";
        // スレーブ用LogFile(開発者用)
        public const string DReceiveLog = "Log\\DevelopReceiveLog.txt";
        // マスター用LogFile.csv(開発者用)
        public const string DMasterLog_csv = "Log\\DevelopMasterLog.csv";
        // スレーブ用LogFile.csv(開発者用)
        public const string DReceiveLog_csv = "Log\\DevelopReceiveLog.csv";


        // ロゴ画像
        public const string LogoImage = "../../image/ROGO.png";
    }
}
