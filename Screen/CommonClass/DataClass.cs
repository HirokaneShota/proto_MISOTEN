using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISOTEN_APPLICATION.Screen.CommonClass
{
    /* 受信データ 文字列 */
    class ReciveData_String
    {
        // デバイスid
        public string Port { get; set; }
        public string ReciveData { get; set; }

        public int Task { get; set; }
    }

    /* 受信byte */
    class ReciveBorn
    {
        // 受信データ:byte
        public byte[] RByte { get; set; }
        // 受信byte数
        public int Invale { get; set; }
    }


    public class ReciveData
    {
        // センサー値
        public ReciveData_Sensor RSensor { get; set; }
        // 信号
        public string RSignal { get; set; }
        // 受信したかどうか 0:NO受信 1:信号 2:センサー値
        public int RFlog { get; set; } = Flog.RNo;

    }
    /* 受信データ 数値 */
    public struct ReciveData_Sensor
    {
        public SENSOR_VALUE Little;  // 小指
        public SENSOR_VALUE Ring;    // 薬指
        public SENSOR_VALUE Middle;  // 中指
        public SENSOR_VALUE Index;   // 人差し指
        public SENSOR_VALUE Thumb;   // 親指
    }

    public struct SENSOR_VALUE
    {
        public float second_ioint;    // 第二関節の曲げセンサor可変抵抗
        public float third_ioint;    // 第三関節の曲げセンサor可変抵抗
        public int tip_pressure;    // 指先の圧力センサ
        public int palm_pressure;    // 手のひらの圧力センサ
    }


}
