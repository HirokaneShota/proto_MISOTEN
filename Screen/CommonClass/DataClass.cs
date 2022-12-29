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

    /* 受信データ(数値+信号+Flog) */
    public struct ReciveData
    {
        // センサー値
        public ReciveData_Sensor RSensor;
        // 信号
        public string RSignal;
        // 受信したかどうか 0:NO受信 1:信号 2:センサー値
        public int RFlog;
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
        public float second_joint;    // 第二関節の曲げセンサor可変抵抗
        public float third_joint;    // 第三関節の曲げセンサor可変抵抗
        public int tip_pressure;    // 指先の圧力センサ
        public int palm_pressure;    // 手のひらの圧力センサ
        public SENSOR_VALUE(float second_joint, float third_joint, int tip_pressure, int palm_pressure)
        {
            this.second_joint = second_joint;
            this.third_joint = third_joint;
            this.tip_pressure = tip_pressure;
            this.palm_pressure = palm_pressure;
        }
    }

    // 角度格納用構造体
    public struct JOINT
    {
        public float second;
        public float third;
    }
    public struct STATING_VALUE
    {
        public JOINT master;
        public JOINT slave;
    }
    // 第一、第二、第三関節間の長さ
    public struct LENGTH
    {
        public float first;
        public float second;
        public float third;
        public LENGTH(float first, float second, float third)
        {
            this.first = first;
            this.second = second;
            this.third = third;
        }
    }

    /* 送信データ pwm値 */
    public struct GOD_SENTENCE
    {
        public int tip_pwm;        //指先の出力値
        public int palm_pwm;       //手のひらの圧力
    }

    public struct GODS_SENTENCE
    {
        public GOD_SENTENCE frist_godsentence;   // 小指
        public GOD_SENTENCE second_godsentence;  // 薬指
        public GOD_SENTENCE third_godsentence;   // 中指
        public GOD_SENTENCE fourth_godsentence;  // 人差し指
        public GOD_SENTENCE fifth_godsentence;   // 親指
    }

    //圧力と軌跡の個別の移動量
    public struct PRESS_TRAJECT
    {
        public int pressure;
        public float traject;
    }

    //前々回、前回、今回のデータ
    public struct PT_LOGS
    {
        public PRESS_TRAJECT last_last_time;
        public PRESS_TRAJECT last_time;
        public PRESS_TRAJECT this_time;
    }
}
