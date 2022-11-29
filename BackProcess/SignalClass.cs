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
    class SignalClass
    {
        // 受信値
        static ReciveData recive = new ReciveData();

        /* コンストラクタ */
        //public SignalClass(SerialPort serialport)
        //{

        //}
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
                // 受信バッファ内のByte数
                Int32 datanum = serialPort.BytesToRead;
                // byte型受信用変数宣言
                byte[] data = new byte[datanum];
                // dataへdatanum数分格納 読み込み
                Int32 invale = serialPort.Read(data, 0, datanum);
                string inCuf = System.Text.Encoding.ASCII.GetString(data);

                // センサー値
                if ((inCuf[0] == 's') && (inCuf[13] == 'e'))
                {
                    // 数値格納変数
                    //ushort[] vale = new ushort[invale];
                    //もし値が飛び飛びになったらここの処理
                    recive.RShort = ByteNum(invale, data);
                    //recive.RShort = vale;
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /* byte(バイナリ)から数値変換 */
        private static ushort[] ByteNum(Int32 invale, byte[] data)
        {
            // 数値格納変数
            ushort[] vale = new ushort[invale];

            // 送信データ"1byte"ずつ格納
            for (int i = 0, j = 0; i < invale; i++, j++)
            {
                // 1byte格納
                byte[] testbyte = new byte[1];
                testbyte[0] = data[i];
                if ((Encoding.ASCII.GetString(testbyte) != "s") && (Encoding.ASCII.GetString(testbyte) != "e"))
                {
                    // 数値変換
                    // ※2byte続きで1センサーの数値が入っている
                    vale[j] = (ushort)((data[i] << 8) + (data[i++] & 0xff));
                    i += 1;
                }
            }
            return vale;
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
            outCuf[0] = (byte)SignalNumData.SNumData[0];
            outCuf[sendNum.Length - 1] = (byte)SignalNumData.ENumData[0];

            // 送信
            serialPort.Write(outCuf, 0, sendNum.Length + 2);
        }


    }
}
