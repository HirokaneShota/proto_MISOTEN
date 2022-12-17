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
        static ReciveData MRecive;
        // センサー受信Flog
        static Boolean[] MRSFlog = new Boolean[4] { false, false, false, false };
        // スレーブ受信値
        static ReciveData SRecive;
        // センサー受信Flog
        static Boolean[] SRSFlog = new Boolean[2] { false, false };

        // マスターシリアルポート
        static SerialPort MSerialport;
        // スレーブシリアルポート
        static SerialPort SSerialport;

        static FileClass file = new FileClass();

        // 仮コンストラクタ
        public SignalClass() {
            //file.First_csv("kumicho");
        }


        /* シリアルポートセッター */
        public void SetSerialport(SerialPort serialport, int id)
        {
            // master受信用ハンドラ作成
            if (id == DeviceId.MasterId)
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

        /* 送信データセッター */
        public void SetSendData(GODS_SENTENCE sendData)
        {
            // 一度に送信するデータ
            int[] SendNum = new int[6];
            // 送信用データ格納
            SendNum[0] = sendData.frist_godsentence.tip_pwm;
            SendNum[1] = sendData.second_godsentence.tip_pwm;
            SendNum[2] = sendData.third_godsentence.tip_pwm;
            SendNum[3] = sendData.fourth_godsentence.tip_pwm;
            SendNum[4] = sendData.fifth_godsentence.tip_pwm;
            SendNum[5] = 0;
            // 送信
            NumSend(DeviceId.ReceiveId, SendNum, SendNumSigna.MSData1[0]);
            // 送信用データ格納
            SendNum[0] = sendData.frist_godsentence.palm_pwm;
            SendNum[1] = sendData.second_godsentence.palm_pwm;
            SendNum[2] = sendData.third_godsentence.palm_pwm;
            SendNum[3] = sendData.fourth_godsentence.palm_pwm;
            SendNum[4] = sendData.fifth_godsentence.palm_pwm;
            SendNum[5] = 0;
            // 送信
            NumSend(DeviceId.ReceiveId, SendNum, SendNumSigna.MSData2[0]);

        }
        /* 送信データ(モーター値)セッター */
        public void SetSendMotor(GODS_SENTENCE sendData)
        {
            // 一度に送信するデータ
            int[] SendNum = new int[6];
            // 送信用データ格納
            SendNum[0] = sendData.frist_godsentence.palm_pwm;
            SendNum[1] = sendData.second_godsentence.palm_pwm;
            SendNum[2] = sendData.third_godsentence.palm_pwm;
            SendNum[3] = sendData.fourth_godsentence.palm_pwm;
            SendNum[4] = sendData.fifth_godsentence.palm_pwm;
            SendNum[5] = 0;
            // 送信
            NumSend(DeviceId.ReceiveId, SendNum, SendNumSigna.MSData2[0]);
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

            FileClass file_sss = new FileClass();
            file_sss.SStartTime();
            // 受信情報処理
            try
            {
                lock (lockObject)
                {
                    // 受信データ格納変数
                    List<byte[]> list = new List<byte[]>();

                    // バッファがなくなるまで
                    while (serialPort.BytesToRead != 0)
                    {
                        // 受信した値をbyte型へ変換
                        byte[] indata = ByteReadTo(serialPort, ReceveNumSignal.End);

                        // 読み取った要素を追加
                        list.Add(indata);
                    }
                    // グローバル変数へ格納
                    SerialStorage(list, serialPort);

                    file_sss.STimeWrite_ms(MSerialport.BytesToRead.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
        }
        /* 受信データ　信号・数値ごとに格納 */
        private static void SerialStorage(List<byte[]> list, SerialPort serialPort)
        {
            for (int i = 0; i < list.Count; i++)
            {
                byte[] data = list[i];//list.Count;

                // inCufへ"ReceveNumSignal.End"まで格納 読み込み
                string inCuf = Encoding.ASCII.GetString(data);
                // 読み込んだデータ数(byte)
                Int32 invale = inCuf.Length;

                //FileClass file = new FileClass();
                //string start = Encoding.ASCII.GetString(data)[0] + "," +Encoding.ASCII.GetString(data)[invale - 1];
                //file.SLog(start);

                // センサー値
                if ((inCuf[0] == ReceveNumSignal.MSData[0]) || (inCuf[0] == ReceveNumSignal.SSData[0]))
                {
                    // マスター用変数へ格納
                    if ((MSerialport != null) && (serialPort.PortName == MSerialport.PortName))
                    {
                        // 取得用変数へ格納
                        Storage(invale, data);
                        // 受信Flog
                        MRecive.RFlog = Flog.RNum;
                    }
                    // スレーブ用変数へ格納
                    else if ((SSerialport != null) && (serialPort.PortName == SSerialport.PortName))
                    {
                        // 取得用変数へ格納
                        Storage(invale, data);
                        // 受信Flog
                        SRecive.RFlog = Flog.RNum;
                    }

                }
                else
                // 信号
                {
                    // マスター用変数へ格納
                    if ((MSerialport != null) && (serialPort.PortName == MSerialport.PortName))
                    {
                        // 取得用変数へ格納
                        MRecive.RSignal = inCuf;
                        // 受信Flog
                        MRecive.RFlog = Flog.RSignal;
                    }
                    // スレーブ用変数へ格納
                    else if ((SSerialport != null) && (serialPort.PortName == SSerialport.PortName))
                    {
                        // 取得用変数へ格納
                        SRecive.RSignal = inCuf;
                        // 受信Flog
                        SRecive.RFlog = Flog.RSignal;
                        if (inCuf == "er02") MessageBox.Show("スレーブマイコンオーバーフロー");
                        //file.Log_csv(inCuf, "\n");
                    }
                }
            }
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

            // zまで読み取る
            for (num = 0; ; num++) // serialPort.BytesToRead != 0
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

        /* 受信バッファー削除 */
        public void ReceiveClearBuffer(int id)
        {
            // master削除
            if (id == DeviceId.MasterId)
            {
                // 繋がっているかどうか判断
                if (MSerialport == null) return;
                if (MSerialport.IsOpen == false) return;
                // バッファークリア
                MSerialport.DiscardInBuffer();
            }
            // slave削除
            else if (id == DeviceId.ReceiveId)
            {
                // 繋がっているかどうか判断
                if (SSerialport == null) return;
                if (SSerialport.IsOpen == false) return;
                // バッファークリア
                SSerialport.DiscardInBuffer();
            }
        }

        /* レシーブ用受信値ゲッター */
        public ReciveData GetSReciveData()
        {
            // 受信するまで　※送受信時間かかれば処理変更
            while ((!SRSFlog.All(i => i == true))&&(SRecive.RFlog != Flog.RSignal)&& (SSerialport.IsOpen == true)) ;
            ReciveData SendData = SRecive;
            InitSignal(DeviceId.ReceiveId);
            return SendData;
        }

        /* マスター用受信値ゲッター */
        public ReciveData GetMReciveData()
        {
            // 受信するまで　※送受信時間かかれば処理変更
            while ((!MRSFlog.All(i => i == true)) && (MRecive.RFlog != Flog.RSignal)) ;
            ReciveData SendData = MRecive;
            InitSignal(DeviceId.MasterId);
            return SendData;
        }

        /* スレーブ用センサー値ゲッター */
        public ReciveData_Sensor GetSSensor()
        {
            TimerClass time = new TimerClass();
            double j = 0;
            time.Start();
            //file.SLog("受信待機");
            // 受信するまで　※送受信時間かかれば処理変更
            while (!SRSFlog.All(i => i == true)) ;
            j = time.MiliElapsed();
            //file.SLog(j.ToString());
            
            lock (lockObject)
            {
                // 初期化
                InitSignal(DeviceId.ReceiveId);
                Array.Clear(SRSFlog, 0, 2);
                return SRecive.RSensor;
            }
        }

        /* マスター用センサー値ゲッター */
        public ReciveData_Sensor GetMSensor()
        {
            TimerClass time = new TimerClass();
            time.Start();
            file.MLog("受信待機");
            // 受信するまで　※送受信時間かかれば処理変更
            while (!MRSFlog.All(i => i == true)) ;
            //while (!(MRecive.RFlog == Flog.RNum)) ;
            double j = time.MiliElapsed();
            file.MLog(j.ToString());
            lock (lockObject)
            {
                // 初期化
                MRecive.RFlog = Flog.RNo;
                InitSignal(DeviceId.MasterId);
                Array.Clear(MRSFlog, 0, 4);
                return MRecive.RSensor;
            }
        }

        /* byte(バイナリ)から数値変換 */
        private static ushort[] ByteNum(Int32 invale, byte[] data)
        {
            lock (lockObject)
            {
                // 数値格納変数
                ushort[] vale111 = new ushort[(14 / 2) - 1];
                // ***********応急処置*******************************************
                if (invale != 14) return vale111;

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
        private static void Storage(Int32 invale, byte[] data)
        {
            // 数値格納変数
            ushort[] shortData = ByteNum(invale, data);

            // 数値信号格納変数
            byte[] startData = new byte[1];
            byte[] endData = new byte[1];
            startData[0] = data[0];
            endData[0] = data[invale - 1];

            lock (lockObject)
            {
                // マスター値処理
                if (Encoding.ASCII.GetString(startData)[0] == ReceveNumSignal.MSData[0])
                {
                    switch (Encoding.ASCII.GetString(endData)[0])
                    {
                        // 圧力センサー*6(指先(小指+薬指+中指+人差し指+親指)+付け根(小指))
                        case ReceveNumSignal.EData1:
                            // 小指・指先
                            MRecive.RSensor.Little.tip_pressure = (int)shortData[0];
                            // 薬指・指先
                            MRecive.RSensor.Ring.tip_pressure = (int)shortData[1];
                            // 中指・指先
                            MRecive.RSensor.Middle.tip_pressure = (int)shortData[2];
                            // 人差し指・指先
                            MRecive.RSensor.Index.tip_pressure = (int)shortData[3];
                            // 親指・指先
                            MRecive.RSensor.Thumb.tip_pressure = (int)shortData[4];
                            // 小指・付け根
                            MRecive.RSensor.Little.palm_pressure = (int)shortData[5];

                            MRSFlog[0] = true;

                            break;
                        // 圧力センサー*4(付け根(薬指+中指+人差し指+親指))+可変抵抗*2(第一関節(小指+薬指))
                        case ReceveNumSignal.EData2:
                            // 薬指・付け根
                            MRecive.RSensor.Ring.palm_pressure = (int)shortData[0];
                            // 中指・付け根
                            MRecive.RSensor.Middle.palm_pressure = (int)shortData[1];
                            // 人差し指・付け根
                            MRecive.RSensor.Index.palm_pressure = (int)shortData[2];
                            // 親指・付け根
                            MRecive.RSensor.Thumb.palm_pressure = (int)shortData[3];
                            // 小指・第二関節
                            MRecive.RSensor.Little.second_joint = (int)shortData[4];
                            // 薬指・第二関節
                            MRecive.RSensor.Ring.second_joint = (int)shortData[5];

                            MRSFlog[1] = true;
                            break;
                        // 可変抵抗*6(第一関節(中指+人差し指+親指)+第二関節(小指+薬指+中指))
                        case ReceveNumSignal.EData3:
                            // 中指・第二関節
                            MRecive.RSensor.Middle.second_joint = (int)shortData[0];
                            // 人差し指・第二関節
                            MRecive.RSensor.Index.second_joint = (int)shortData[1];
                            // 親指・第二関節
                            MRecive.RSensor.Thumb.second_joint = (int)shortData[2];
                            // 小指・第三関節
                            MRecive.RSensor.Little.third_joint = (int)shortData[3];
                            // 薬指・第三関節
                            MRecive.RSensor.Ring.third_joint = (int)shortData[4];
                            // 中指・第三関節
                            MRecive.RSensor.Middle.third_joint = (int)shortData[5];

                            MRSFlog[2] = true;
                            break;
                        // 可変抵抗*1(人差し指)+エンプティ*1(親指関節)+曲げセンサー*4
                        case ReceveNumSignal.EData4:
                            //人差し指・第三関節
                            MRecive.RSensor.Index.third_joint = (int)shortData[0];
                            // 親指・曲げセンサー
                            MRecive.RSensor.Thumb.third_joint = (int)shortData[1];

                            MRSFlog[3] = true;
                            break;
                    }
                    // file書き込み
                    //file.MLog(Array.ConvertAll(shortData, x => x.ToString()));
                }
                // スレーブ値処理
                else if (Encoding.ASCII.GetString(startData)[0] == ReceveNumSignal.SSData[0])
                {
                    //スレーブのセンサー値格納処理
                    switch (Encoding.ASCII.GetString(endData)[0])
                    {
                        // 可変抵抗*6(第一関節(小指+薬指+中指+人差し指+親指))+第二関節(小指))

                        case ReceveNumSignal.EData1:
                            // 小指・第二関節
                            SRecive.RSensor.Little.second_joint = (int)shortData[0];
                            // 薬指・第二関節
                            SRecive.RSensor.Ring.second_joint = (int)shortData[1];
                            // 中指・第二関節
                            SRecive.RSensor.Middle.second_joint = (int)shortData[2];
                            // 人差し指・第二関節
                            SRecive.RSensor.Index.second_joint = (int)shortData[3];
                            // 親指・第二関節
                            SRecive.RSensor.Thumb.third_joint = (int)shortData[4];
                            // 小指・第三関節
                            SRecive.RSensor.Little.third_joint = (int)shortData[5];
                            SRSFlog[0] = true;
                            break;
                        // 可変抵抗*3(第二関節(薬指+中指+人差し指))+曲げセンサー*1(親指関節)+エンプティ*2
                        case ReceveNumSignal.EData2:

                            // 薬指・第三関節
                            SRecive.RSensor.Ring.third_joint = (int)shortData[0];
                            // 中指・第三関節
                            SRecive.RSensor.Middle.third_joint = (int)shortData[1];
                            //人差し指・第三関節
                            SRecive.RSensor.Index.third_joint = (int)shortData[2];
                            // 親指・曲げセンサー
                            SRecive.RSensor.Thumb.third_joint = (int)shortData[3];
                            SRSFlog[1] = true;
                            break;
                    }
                    // file書き込み
                    //file.SLog(Array.ConvertAll(shortData, x => x.ToString()));
                }
            }
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
        public void NumSend(int id, int[] sendNum, char frist_data)
        {
            // byte型送信用変数宣言     送信数字数 *　1数字を2byte
            byte[] outCuf = new byte[(sendNum.Length * 2) + 2];

            // 送信データ"1byte"ずつ格納
            for (int sendcount = 0, num = 1; sendcount < sendNum.Length; sendcount += 1, num += 2)
            {
                // 数値格納
                outCuf[num] = (byte)(sendNum[sendcount] >> 8);
                outCuf[num + 1] = (byte)(sendNum[sendcount] & 0xFF);
            }
            // 数値データ信号 SendNumSigna.MSData1[0]
            outCuf[0] = (byte)frist_data;
            outCuf[outCuf.Length - 1] = (byte)SendNumSigna.EData[0];

            // master送信
            if (id == DeviceId.MasterId)
            {
                // 送信
                MSerialport.Write(outCuf, 0, outCuf.Length);
            }
            // slave送信
            else if (id == DeviceId.ReceiveId)
            {
                // 送信
                SSerialport.Write(outCuf, 0, outCuf.Length);
            }
        }

        /* Port切断処理 */
        public void ProtCut(int id)
        {
            // master切断
            if (id == DeviceId.MasterId)
            {
                if (MSerialport == null) return;
                if (MSerialport.IsOpen == false) return;
                // ポート切断
                MSerialport.Close();
                MSerialport = null;
            }
            // slave切断
            else if (id == DeviceId.ReceiveId)
            {
                if (SSerialport == null) return;
                if (SSerialport.IsOpen == false) return;
                // ポート切断
                SSerialport.Close();
                SSerialport = null;
            }
        }
    }
}
