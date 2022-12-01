using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using MISOTEN_APPLICATION.Screen.CommonClass;

namespace MISOTEN_APPLICATION.BackProcess
{
    /* 送受信クラス */
    public class SignalClass
    {
        // 排他制御に使用するオブジェクト
        private static Object lockObject = new Object();

        // 受信値
        static ReciveData recive = new ReciveData();
        static ReciveBorn BRecive = new ReciveBorn();

        public void InitSignal()
        {
            // 初期化
            recive.RFlog = Flog.RNo;
        }

        /* 受信バンドラ関数 */
        public void ReceiveHandler(SerialPort serialport)
        {
            // 初期化
            recive.RFlog = Flog.RNo;
            // 受信用ハンドラ
            serialport.DataReceived += new SerialDataReceivedEventHandler(DataNumReceived);
        }

        /* 受信待機 */
        public ReciveData NumReceived()
        {
            // 受信するまで　※送受信時間かかれば処理変更
            for (; recive.RFlog == Flog.RNo;) ;

            if (recive.RFlog == Flog.RNum)
            {
                // 数値格納変数
                ushort[] shortData = new ushort[(BRecive.Invale / 2) - 1];
                //もし値が飛び飛びになったらここの処理
                // byte→short変換
                shortData = ByteNum(BRecive.Invale, BRecive.RByte);

                // 数値信号格納変数
                byte[] startData = new byte[1];
                byte[] endData = new byte[1];
                startData[0] = BRecive.RByte[0];
                endData[0] = BRecive.RByte[BRecive.Invale - 1];

                recive.RSensor = Storage((Encoding.ASCII.GetString(startData)[0]), (Encoding.ASCII.GetString(endData)[0]), shortData);
            }


            return recive;
        }
        /* ReceivedEventHandler */
        public static void DataNumReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = (SerialPort)sender;
            // 繋がっているかどうか判断
            if (serialPort == null) return;
            if (serialPort.IsOpen == false) return;

            // 受信情報処理
            try
            {
                lock (lockObject)
                {
                    // 受信バッファ内のByte数
                    Int32 datanum = serialPort.BytesToRead;
                    // byte型受信用変数宣言
                    byte[] data = new byte[datanum];
                    // dataへdatanum数分格納 読み込み
                    Int32 invale = serialPort.Read(data, 0, datanum);
                    string inCuf = System.Text.Encoding.ASCII.GetString(data);
                    // センサー値
                    if ((inCuf[0] == ReceveNumSignal.MSData[0]) || (inCuf[0] == ReceveNumSignal.SSData[0]))
                    {

                        BRecive.RByte = data;
                        BRecive.Invale = invale;
                        // 受信Flog
                        recive.RFlog = Flog.RNum;
                    }
                    else
                    // 信号
                    {
                        recive.RString = inCuf;
                        // 受信Flog
                        recive.RFlog = Flog.RSignal;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /* byte(バイナリ)から数値変換 */
        private static ushort[] ByteNum(Int32 invale, byte[] data)
        {
            // 数値格納変数
            ushort[] vale = new ushort[(invale / 2) - 1];

            // 送信データ"1byte"ずつ格納
            for (int i = 1, j = 0; i < invale - 1; i = +2, j++)
            {
                // 1byte格納
                byte[] testbyte = new byte[1];
                testbyte[0] = data[i];
                if ((Encoding.ASCII.GetString(testbyte) != ReceveNumSignal.MSData) && (Encoding.ASCII.GetString(testbyte) != ReceveNumSignal.SSData))
                {
                    // 数値変換
                    // ※2byte続きで1センサーの数値が入っている
                    vale[j] = (ushort)((data[i] << 8) + (data[i++] & 0xff));
                }
            }
            return vale;
        }

        private static ReciveData_Sensor Storage(char sdata, char edata, ushort[] num)
        {
            // 数値格納変数
            ReciveData_Sensor sensorVale = new ReciveData_Sensor();

            // マスター値処理
            if (sdata == ReceveNumSignal.MSData[0])
            {
                switch (edata)
                {
                    // 圧力センサー*6(指先(小指+薬指+中指+人差し指+親指)+付け根(小指))
                    case ReceveNumSignal.EData1:
                        // 小指・指先
                        sensorVale.FingPress.Little = (int)num[0];
                        // 薬指・指先
                        sensorVale.FingPress.Ring = (int)num[1];
                        // 中指・指先
                        sensorVale.FingPress.Middle = (int)num[2];
                        // 人差し指・指先
                        sensorVale.FingPress.Index = (int)num[3];
                        // 親指・指先
                        sensorVale.FingPress.Thumb = (int)num[4];
                        // 小指・付け根
                        sensorVale.PalmPress.Little = (int)num[5];
                        break;
                    // 圧力センサー*4(付け根(薬指+中指+人差し指+親指))+可変抵抗*2(第一関節(小指+薬指))
                    case ReceveNumSignal.EData2:
                        // 薬指・付け根
                        sensorVale.PalmPress.Ring = (int)num[0];
                        // 中指・付け根
                        sensorVale.PalmPress.Middle = (int)num[1];
                        // 人差し指・付け根
                        sensorVale.PalmPress.Index = (int)num[2];
                        // 親指・付け根
                        sensorVale.PalmPress.Thumb = (int)num[3];
                        // 小指・第一関節
                        sensorVale.FristResis.Little = (int)num[4];
                        // 薬指・第一関節
                        sensorVale.FristResis.Ring = (int)num[5];

                        break;
                    // 可変抵抗*6(第一関節(中指+人差し指)+第二関節(小指+薬指+中指+人差し指))
                    case ReceveNumSignal.EData3:
                        // 中指・第一関節
                        sensorVale.FristResis.Middle = (int)num[0];
                        // 人差し指・第一関節
                        sensorVale.FristResis.Index = (int)num[1];
                        // 小指・第二関節
                        sensorVale.SecondResis.Little = (int)num[2];
                        // 薬指・第二関節
                        sensorVale.SecondResis.Ring = (int)num[3];
                        // 中指・第二関節
                        sensorVale.SecondResis.Middle = (int)num[4];
                        //人差し指・第二関節
                        sensorVale.SecondResis.Index = (int)num[5];
                        break;
                    // 可変抵抗*1(親指関節)+曲げセンサー*1(親指関節)+エンプティ*4
                    case ReceveNumSignal.EData4:
                        // 親指・第二関節
                        sensorVale.SecondResis.Thumb = (int)num[0];
                        // 親指・曲げセンサー
                        sensorVale.FingBend.Thumb = (int)num[1];
                        break;
                }
            }
            // レシーブ値処理
            else if (sdata == ReceveNumSignal.SSData[0])
            {
                //レシーブのセンサー値格納処理
            }

            return sensorVale;
        }

        /* 信号送信用関数 */
        public void SignalSend(SerialPort serialPort, string sendString)
        {
            // 送信文字列のbyte数カウント
            int count = Encoding.GetEncoding("Shift_JIS").GetByteCount(sendString);
            // byte型送信用変数宣言
            byte[] outCuf = new byte[count];
            int sendcount = 0;

            // 送信データ"1byte"ずつ格納
            for (; sendcount < count; sendcount++)
            {
                // 1byte格納
                outCuf[sendcount] = (byte)sendString[sendcount];

            }

            // 送信
            serialPort.Write(outCuf, 0, sendcount);
        }

        /* 数値送信用関数 */
        public void NumSend(SerialPort serialPort, int[] sendNum)
        {
            // byte型送信用変数宣言 :+ 2='s'&'e'
            byte[] outCuf = new byte[sendNum.Length + 2];

            // 送信データ"1byte"ずつ格納
            for (int sendcount = 1; sendcount < sendNum.Length - 2; sendcount = +2)
            {
                // 数値格納
                outCuf[sendcount] = (byte)(sendNum[sendcount] >> 8);
                outCuf[sendcount + 1] = (byte)(sendNum[sendcount] & 0xFF);

            }
            // 数値データ信号 
            outCuf[0] = (byte)SendNumSigna.MSData[0];
            outCuf[sendNum.Length - 1] = (byte)SendNumSigna.EData[0];

            // 送信
            serialPort.Write(outCuf, 0, sendNum.Length + 2);
        }


    }
}
