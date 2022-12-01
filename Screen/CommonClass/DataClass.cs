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
        public string RString { get; set; }
        // 受信したかどうか 0:NO受信 1:信号 2:センサー値
        public int RFlog { get; set; } = Flog.RNo;

    }
    /* 受信データ 数値 */
    public struct ReciveData_Sensor
    {
        public PressureSensor FingPress;      // 指先・圧力センサー
        public PressureSensor PalmPress;      // 付け根・圧力センサー
        public ResistorSensor FristResis;     // 第一関節・可変抵抗
        public ResistorSensor SecondResis;    // 第二関節・可変抵抗
        public BendSensor FingBend;    // 親指・可変抵抗
    }

    // 圧力センサー
    public struct PressureSensor
    {
        public int Little;  // 小指
        public int Ring;    // 薬指
        public int Middle;  // 中指
        public int Index;   // 人差し指
        public int Thumb;   // 親指
    }
    // 可変抵抗
    public struct ResistorSensor
    {
        public int Little;  // 小指
        public int Ring;    // 薬指
        public int Middle;  // 中指
        public int Index;   // 人差し
        public int Thumb;   // 親指曲げセンサー
    }
    // 曲げセンサー
    public struct BendSensor
    {
        public int Thumb;   // 親指曲げセンサー
    }


}
