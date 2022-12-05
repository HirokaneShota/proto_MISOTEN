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

        // マスター受信値
        static ReciveData MRecive = new ReciveData();
        // スレーブ受信値
        static ReciveData SRecive = new ReciveData();


        // マスターシリアルポート
        static SerialPort MSerialport;
        // スレーブシリアルポート
        static SerialPort SSerialport;

        /* シリアルポートセッター */
        public void SetSerialport(SerialPort serialport, int id)
        {
            // master受信用ハンドラ作成
            if(id == DeviceId.MasterId)
            {
                ReceiveHandler(serialport);
                MSerialport = serialport;
            }
            // slave受信用ハンドラ作成
            else if (id == DeviceId.ReceiveId)
            {
                ReceiveHandler(serialport);
                SSerialport = serialport;
            }
        }

        /* Flog初期化 */
        public void InitSignal(int id)
        {
            // master受信フラグ初期化
            if (id == DeviceId.MasterId)
            {
                // 初期化
                MRecive.RFlog = Flog.RNo;
            }
            // slave受信フラグ初期化
            else if (id == DeviceId.ReceiveId)
            {
                // 初期化
                SRecive.RFlog = Flog.RNo;
            }
        }

        /* 受信バンドラ関数 */
        public void ReceiveHandler(SerialPort serialport)
        {
            // 繋がっているかどうか判断
            if (serialport == null) return;
            if (serialport.IsOpen == false) return;
            // 受信用ハンドラ
            serialport.DataReceived += new SerialDataReceivedEventHandler(DataNumReceived);
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
                    // 受信した値をbyte型へ変換
                    byte[] data = ByteReadTo(serialPort, ReceveNumSignal.End);
                    // inCufへ"ReceveNumSignal.End"まで格納 読み込み
                    string inCuf = Encoding.ASCII.GetString(data);
                    // 読み込んだデータ数(byte)
                    Int32 invale = inCuf.Length;
                    // センサー値
                    if ((inCuf[0] == ReceveNumSignal.MSData[0]) || (inCuf[0] == ReceveNumSignal.SSData[0]))
                    {
                        // マスター用変数へ格納
                        if (serialPort.PortName == MSerialport.PortName)
                        {
                            // 取得用変数へ格納
                            MRecive.RSensor = Storage(invale, data);
                            // 受信Flog
                            MRecive.RFlog = Flog.RNum;
                        }
                        // スレーブ用変数へ格納
                        else if (serialPort.PortName == SSerialport.PortName)
                        {
                            // 取得用変数へ格納
                            SRecive.RSensor = Storage(invale, data);
                            // 受信Flog
                            SRecive.RFlog = Flog.RNum;
                        }

                    }
                    else
                    // 信号
                    {
                        // マスター用変数へ格納
                        if (serialPort.PortName == MSerialport.PortName)
                        {
                            // 取得用変数へ格納
                            MRecive.RSignal = inCuf;
                            // 受信Flog
                            MRecive.RFlog = Flog.RSignal;
                        }
                        // スレーブ用変数へ格納
                        else if (serialPort.PortName == SSerialport.PortName)
                        {
                            // 取得用変数へ格納
                            SRecive.RSignal = inCuf;
                            // 受信Flog
                            SRecive.RFlog = Flog.RSignal;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /* レシーブ用受信値ゲッター */
        public ReciveData GetSReciveData()
        {
            // 初期化
            InitSignal(DeviceId.ReceiveId);
            // 受信するまで　※送受信時間かかれば処理変更
            for (; SRecive.RFlog == Flog.RNo;) ;

            return SRecive;
        }

        /* マスター用受信値ゲッター */
        public ReciveData GetMReciveData()
        {
            // 初期化
            InitSignal(DeviceId.MasterId);
            // 受信するまで　※送受信時間かかれば処理変更
            for (; MRecive.RFlog == Flog.RNo;) ;

            return MRecive;
        }

        /* レシーブ用センサー値ゲッター */
        public ReciveData_Sensor GetSSensor()
        {
            // 初期化
            InitSignal(DeviceId.ReceiveId);
            // 受信するまで　※送受信時間かかれば処理変更
            for (; SRecive.RFlog == Flog.RNo;) ;

            return SRecive.RSensor;
        }

        /* マスター用センサー値ゲッター */
        public ReciveData_Sensor GetMSensor()
        {
            // 初期化
            InitSignal(DeviceId.MasterId);
            // 受信するまで　※送受信時間かかれば処理変更
            for (; MRecive.RFlog == Flog.RNo;) ;

            return MRecive.RSensor;
        }

         /* 受信用関数 */
        private static byte[] ByteReadTo(SerialPort serialPort, string end)
        {
            // 受信値 : int型格納変数
            int test = 0;
            // 受信値 : byte型格納変数
            byte[] data = new byte[1];
            // 受信byte数
            int num = 0;

            for (num = 0; ; num++)
            {
                // 1byteずつ格納
                // end文字なら抜ける
                if (Convert.ToByte(test = serialPort.ReadByte()) == System.Text.Encoding.ASCII.GetBytes(end)[0]) break;
                // 読み込んだbyte数分配列追加
                Array.Resize(ref data, num + 1);
                // intで取得した値をbyte格納
                data[num] = Convert.ToByte(test);
            }
            return data;
        }

        /* byte(バイナリ)から数値変換 */
        private static ushort[] ByteNum(Int32 invale, byte[] data)
        {
            lock (lockObject)
            {
                // 数値格納変数
                ushort[] vale = new ushort[(invale / 2) - 1];

                // 送信データ"1byte"ずつ格納
                for (int i = 1, j = 0; i < invale - 1; i = i + 2)
                {
                    // 1byte格納
                    byte[] testbyte = new byte[1];
                    testbyte[0] = data[i];
                    if ((Encoding.ASCII.GetString(testbyte) != ReceveNumSignal.MSData) && (Encoding.ASCII.GetString(testbyte) != ReceveNumSignal.SSData))
                    {
                        // 数値変換
                        // ※2byte続きで1センサーの数値が入っている
                        vale[j] = (ushort)((data[i] << 8) + (data[i + 1] & 0xff));
                        j++;
                    }
                }

                return vale;
            }
        }
        /* 受信値変数格納 */
        private static ReciveData_Sensor Storage(Int32 invale, byte[] data)
        {

            // 数値格納変数
            ushort[] shortData = ByteNum(invale, data);

            // 数値信号格納変数
            byte[] startData = new byte[1];
            byte[] endData = new byte[1];
            startData[0] = data[0];
            endData[0] = data[invale - 1];

            // 数値格納変数
            ReciveData_Sensor sensorVale = new ReciveData_Sensor();


            switch (Encoding.ASCII.GetString(endData)[0])
            {
                // 圧力センサー*6(指先(小指+薬指+中指+人差し指+親指)+付け根(小指))
                case ReceveNumSignal.EData1:
                    // 小指・指先
                    sensorVale.Little.tip_pressure = (int)shortData[0];
                    // 薬指・指先
                    sensorVale.Ring.tip_pressure = (int)shortData[1];
                    // 中指・指先
                    sensorVale.Middle.tip_pressure = (int)shortData[2];
                    // 人差し指・指先
                    sensorVale.Index.tip_pressure = (int)shortData[3];
                    // 親指・指先
                    sensorVale.Thumb.tip_pressure = (int)shortData[4];
                    // 小指・付け根
                    sensorVale.Little.palm_pressure = (int)shortData[5];
                    break;
                // 圧力センサー*4(付け根(薬指+中指+人差し指+親指))+可変抵抗*2(第一関節(小指+薬指))
                case ReceveNumSignal.EData2:
                    // 薬指・付け根
                    sensorVale.Ring.palm_pressure = (int)shortData[0];
                    // 中指・付け根
                    sensorVale.Middle.palm_pressure = (int)shortData[1];
                    // 人差し指・付け根
                    sensorVale.Index.palm_pressure = (int)shortData[2];
                    // 親指・付け根
                    sensorVale.Thumb.palm_pressure = (int)shortData[3];
                    // 小指・第二関節
                    sensorVale.Little.second_ioint = (int)shortData[4];
                    // 薬指・第二関節
                    sensorVale.Ring.second_ioint = (int)shortData[5];

                    break;
                // 可変抵抗*6(第一関節(中指+人差し指+親指)+第二関節(小指+薬指+中指))
                case ReceveNumSignal.EData3:
                    // 中指・第二関節
                    sensorVale.Middle.second_ioint = (int)shortData[0];
                    // 人差し指・第二関節
                    sensorVale.Index.second_ioint = (int)shortData[1];
                    // 親指・第二関節
                    sensorVale.Thumb.third_ioint = (int)shortData[2];
                    // 小指・第三関節
                    sensorVale.Little.third_ioint = (int)shortData[3];
                    // 薬指・第三関節
                    sensorVale.Ring.third_ioint = (int)shortData[4];
                    // 中指・第三関節
                    sensorVale.Middle.third_ioint = (int)shortData[5];
                    break;
                // 可変抵抗*1(人差し指)+エンプティ*1(親指関節)+曲げセンサー*4
                case ReceveNumSignal.EData4:
                    //人差し指・第三関節
                    sensorVale.Index.third_ioint = (int)shortData[0];
                    // 親指・曲げセンサー
                    sensorVale.Thumb.third_ioint = (int)shortData[1];
                    break;
            }

            return sensorVale;
        }

        /* 信号送信用関数 */
        public void SignalSend(int id, string sendString)
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

            // master送信
            if (id == DeviceId.MasterId)
            {
                // 送信
                MSerialport.Write(outCuf, 0, sendcount);
            }
            // slave送信
            else if (id == DeviceId.ReceiveId)
            {
                // 送信
                SSerialport.Write(outCuf, 0, sendcount);
            }
        }

        /* 数値送信用関数 */
        public void NumSend(int id, int[] sendNum)
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

            // master送信
            if (id == DeviceId.MasterId)
            {
                // 送信
                MSerialport.Write(outCuf, 0, sendNum.Length + 2);
            }
            // slave送信
            else if (id == DeviceId.ReceiveId)
            {
                // 送信
                SSerialport.Write(outCuf, 0, sendNum.Length + 2);
            }
        }


        /* Port切断処理 */
        public void ProtCut(int id)
        {
            // master切断
            if (id == DeviceId.MasterId)
            {
                if (MSerialport.IsOpen == false) return;
                // ポート切断
                MSerialport.Close();
            }
            // slave切断
            else if (id == DeviceId.ReceiveId)
            {
                if (SSerialport.IsOpen == false) return;
                // ポート切断
                SSerialport.Close();
            }
        }
    }
}
