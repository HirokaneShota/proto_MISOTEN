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
}
