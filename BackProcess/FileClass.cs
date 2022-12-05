using MISOTEN_APPLICATION.Screen.CommonClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISOTEN_APPLICATION.BackProcess
{
    class FileClass
    {
        string muri = URI.MasterLog;
        string suri = URI.ReceiveLog;
        Timer mtimer = new Timer();
        Timer stimer = new Timer();

        /* ファイル生成＆開始文字(master) */
        public void MFirst()
        {
            File.WriteAllText(@muri, "マスター接続開始" + Environment.NewLine);
        }
        /* ファイル生成＆開始文字(slave) */
        public void SFirst()
        {
            File.WriteAllText(@suri, "スレーブ接続開始" + Environment.NewLine);
        }
        //
        // 現在時間表示
        //
        /* 年月日曜日時分秒ミリ秒+出力値 表示(master) */
        public void MLog(params string[] letter)
        {
            // 現在時刻を取得
            DateTime time = DateTime.Now;
            File.AppendAllText(@muri, time.ToString("tthh:mm:ss:fff") + " : ");
            for (int i = 0; i< letter.Length; i++)
            {
                File.AppendAllText(@muri, letter[i] );
            }
            File.AppendAllText(@muri, Environment.NewLine);
        }
        /* 年月日曜日時分秒ミリ秒+出力値 表示(slave) */
        public void SLog(params string[] letter)
        {
            // 現在時刻を取得
            DateTime time = DateTime.Now;
            File.AppendAllText(@suri, time.ToString("tthh:mm:ss:fff") + " : ");
            for (int i = 0; i < letter.Length; i++)
            {
                File.AppendAllText(@suri, letter[i] );
            }
            File.AppendAllText(@suri, Environment.NewLine);
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
            File.AppendAllText(@muri, mtimer.MiliElapsed() + "ms :" + letter + Environment.NewLine);
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
            File.AppendAllText(@suri, stimer.MiliElapsed() + "ms :" + letter + Environment.NewLine);
        }
        /* 計測時間＆文字書き込み(us)(slave) */
        public void STimeWrite_us(string letter)
        {
            // master
            File.AppendAllText(@suri, stimer.MiliElapsed() + "us :" + letter + Environment.NewLine);
        }


    }
}
