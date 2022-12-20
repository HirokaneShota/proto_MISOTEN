using MISOTEN_APPLICATION.Screen.CommonClass;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MISOTEN_APPLICATION.BackProcess
{
    class FileClass
    {
        string muri = URI.MasterLog;
        string suri = URI.ReceiveLog;

        string dmuri = URI.DMasterLog;
        string dsuri = URI.DReceiveLog;

        string dmuri_csv = URI.DMasterLog_csv;
        string dsuri_csv = URI.DReceiveLog_csv;

        string csv = "";

        TimerClass mtimer = new TimerClass();
        TimerClass stimer = new TimerClass();
        /* コンストラクタ */
        public FileClass()
        {
            if (!Directory.Exists(URI.LogFolder))
            {
                Directory.CreateDirectory(URI.LogFolder);
            }
        }

        /* ファイル生成＆開始文字(master) */
        public void MFirst() => File.WriteAllText(@muri, "マスター接続開始" + Environment.NewLine);
        /* ファイル生成＆開始文字(slave) */
        public void SFirst() => File.WriteAllText(@suri, "スレーブ接続開始" + Environment.NewLine);
        /* ファイル生成＆開始文字(master)(開発者用) */
        public void DMFirst() => File.WriteAllText(@dmuri, "マスター接続開始" + Environment.NewLine);
        /* ファイル生成＆開始文字(slave)(開発者用) */
        public void DSFirst() => File.WriteAllText(@dsuri, "スレーブ接続開始" + Environment.NewLine);

        /* CSVファイル */
        /* ファイル生成(master)(開発者用) */
        public void DMFirst_csv()
        {
            try
            {
                // 現在時刻を取得
                DateTime time = DateTime.Now;
                dmuri_csv = URI.LogFolder + time.ToString("MMddhhmmss") + "_" + dmuri_csv;
                FileStream file = File.Create(dmuri_csv);
                file.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
        }
        /* ファイル生成(slave)(開発者用) */
        public void DSFirst_csv()
        {
            try
            {
                // 現在時刻を取得
                DateTime time = DateTime.Now;
                dsuri_csv = URI.LogFolder + time.ToString("MMddhhmmss") + "_" + dsuri_csv;
                FileStream file = File.Create(dsuri_csv);
                file.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
        }
   
        /* ファイル生成(開発者用) */
        public void First_csv(string name)
        {
            try
            {
                // 現在時刻を取得
                DateTime time = DateTime.Now;
                csv = URI.LogFolder + time.ToString("MMddhhmmss") + "_" + name + ".csv";
                FileStream file = File.Create(csv);
                file.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
}


        //
        // 現在時間表示
        //
        /* 年月日曜日時分秒ミリ秒+出力値 表示(master) */
        public void MLog(params string[] letter)
        {
            // 現在時刻を取得
            DateTime time = DateTime.Now;
            string Letter = Aggregation_string(letter);
            File.AppendAllText(@muri, time.ToString("hh:mm:ss:fff") + " : " + Letter +  Environment.NewLine);
        }
        /* 年月日曜日時分秒ミリ秒+出力値 表示(slave) */
        public void SLog(params string[] letter)
        {
            // 現在時刻を取得
            DateTime time = DateTime.Now;
            string Letter = Aggregation_string(letter);
            File.AppendAllText(@suri, time.ToString("hh:mm:ss:fff") + " : " + Letter + Environment.NewLine);
        }

        //
        // 現在時間表示 (開発者用)
        //
        /* 年月日曜日時分秒ミリ秒+出力値 表示(master) */
        public void MDLog(params string[] letter)
        {
            // 現在時刻を取得
            DateTime time = DateTime.Now;
            string Letter = Aggregation_string(letter);
            File.AppendAllText(@dmuri, time.ToString("hh:mm:ss:fff") + " : " + Letter + Environment.NewLine);
        }
        /* 年月日曜日時分秒ミリ秒+出力値 表示(slave) */
        public void SDLog(params string[] letter)
        {
            // 現在時刻を取得
            DateTime time = DateTime.Now;
            string Letter = Aggregation_string(letter);
            File.AppendAllText(@dsuri, time.ToString("hh:mm:ss:fff") + " : " + Letter + Environment.NewLine);
        }

        /* 年月日曜日時分秒ミリ秒+出力値 表示 (csvFile)*/
        public void Log_csv(params string[] letter)
        {
            // 現在時刻を取得
            DateTime time = DateTime.Now;
            string Letter = Aggregation_string(letter);
            File.AppendAllText(@csv, time.ToString("hh:mm:ss:fff") + "," + Letter);
        }

        /* 複数のstringを一つのstringへ "追記:12/15\n(改行)処理追加"*/
        private string Aggregation_string(params string[] letter)
        {
            string Letter = "";
            for (int i = 0; i < letter.Length; i++)
            {
                // "\n"あれば改行(csvでのみ使用)
                if(letter[i] == "\n") Letter = Letter + Environment.NewLine;
                else Letter = Letter + letter[i];
                if (i != letter.Length - 1)
                {
                    Letter = Letter + ",";
                }
            }
            return Letter;
        }

        //
        // 現在時間表示 (開発者用　数値のみ　csvファイル)
        //
        /* 年月日曜日時分秒ミリ秒+出力値 表示(master) */
        public void MDLog_csv(ReciveData_Sensor sensor)
        {
            // 現在時刻を取得
            DateTime time = DateTime.Now;
            string Letter = Aggregation_num(sensor);
            File.AppendAllText(@dmuri_csv, time.ToString("hh:mm:ss:fff") + "," + Letter + Environment.NewLine);
            //Debug.Print(Letter);
        }
        /* 年月日曜日時分秒ミリ秒+出力値 表示(slave) */
        public void SDLog_csv(ReciveData_Sensor sensor)
        {
            // 現在時刻を取得
            DateTime time = DateTime.Now;
            string Letter = Aggregation_num(sensor);
            File.AppendAllText(@dsuri_csv, time.ToString("hh:mm:ss:fff") + "," + Letter + Environment.NewLine);
        }
        /* ReciveData_Sensor→一文(string型)変換 */
        private string Aggregation_num(ReciveData_Sensor sensor)
        {
            string Letter = "";

            // **** 小指 *****
            //指先 圧力
            Letter +=  sensor.Little.tip_pressure.ToString() + ",";
            //　第二関節　抵抗
            Letter += sensor.Little.second_joint.ToString() + ",";
            //　第三関節 抵抗
            Letter += sensor.Little.third_joint.ToString() + "," + null + " ,";

            // **** 薬指 *****
            //　指先 圧力
            Letter += sensor.Ring.tip_pressure.ToString() + ",";
            //　第二関節　抵抗
            Letter += sensor.Ring.second_joint.ToString() + ",";
            //　第三関節 抵抗
            Letter += sensor.Ring.third_joint.ToString() + "," + null + " ,";

            // **** 中指 *****
            //　指先 圧力
            Letter += sensor.Middle.tip_pressure.ToString() + ",";
            //　第二関節　抵抗
            Letter += sensor.Middle.second_joint.ToString() + ",";
            //　第三関節 抵抗
            Letter += sensor.Middle.third_joint.ToString() + "," + null + " ,";
            
            // **** 人差し指 *****
            //　指先 圧力
            Letter += sensor.Index.tip_pressure.ToString() + ",";
            //　第二関節　抵抗
            Letter += sensor.Index.second_joint.ToString() + ",";
            //　第三関節 抵抗
            Letter += sensor.Index.third_joint.ToString() + "," + null +" ,";

            // **** 親指 *****
            //　指先 圧力
            Letter += sensor.Thumb.tip_pressure.ToString() + ",";
            //　第二関節　抵抗
            Letter += sensor.Thumb.second_joint.ToString() + ",";
            //　第三関節 抵抗
            Letter += sensor.Thumb.third_joint.ToString() + "," + null+",";

            // **** 手のひら　圧力 *****
            //　小指
            Letter += sensor.Little.palm_pressure.ToString() + ",";
            //　薬指
            Letter += sensor.Ring.palm_pressure.ToString() + ",";
            //　中指
            Letter += sensor.Middle.palm_pressure.ToString() + ",";
            //　人差し指
            Letter += sensor.Index.palm_pressure.ToString() + ",";
            //　親指
            Letter += sensor.Thumb.palm_pressure.ToString();

            return Letter;
        }

        //
        // 計測時間のみ表示
        //
        /* 計測時間スタート(master) */
        public void MStartTime()
        {
            // master
            mtimer.Start();
        }
        /* 計測時間リスタート(master) */
        public void MReStartTime()
        {
            // master
            mtimer.ReStart();
        }
        /* 計測時間＆文字書き込み(ms)(master) */
        public void MTimeWrite_ms(string letter)
        {
            // master
            File.AppendAllText(@muri, mtimer.MiliElapsed() + "" + letter + Environment.NewLine);//ms :
        }
        /* 計測時間＆文字書き込み(us)(master) */
        public void MTimeWrite_us(string letter)
        {
            // master
            File.AppendAllText(@muri, mtimer.MiliElapsed() + "us :" + letter + Environment.NewLine);
        }

        /* 計測時間スタート(slave) */
        public void SStartTime()
        {
            // slave
            stimer.Start();
        }
        /* 計測時間リスタート(slave) */
        public void SReStartTime()
        {
            // slave
            stimer.ReStart();
        }
        /* 計測時間＆文字書き込み(ms)(slave) */
        public void STimeWrite_ms(string letter)
        {
            // master
            File.AppendAllText(@suri, stimer.MiliElapsed() + "ms :" + letter + Environment.NewLine);//ms :
        }
        /* 計測時間＆文字書き込み(us)(slave) */
        public void STimeWrite_us(string letter)
        {
            // master
            File.AppendAllText(@suri, stimer.MiliElapsed() + "us :" + letter + Environment.NewLine);
        }


    }
}
