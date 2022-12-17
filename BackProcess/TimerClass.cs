using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISOTEN_APPLICATION.BackProcess
{
    class TimerClass
    {
        Stopwatch SWatch = new Stopwatch();
        TimeSpan TSpan = new TimeSpan();

        // タイマースタート
        public void Start()
        {
            // 計測開始
            SWatch.Start();
        }
        // タイマーリセット
        public void Reset()
        {
            // 計測時間リセット
            SWatch.Reset();
        }

        // タイマーリセット＆スタート
        public void ReStart()
        {
            // 計測時間リセット＆計測開始
            SWatch.Restart();
        }


        // タイマー途中時間出力(ミリ秒)
        public double MiliElapsed()
        {
            TSpan = SWatch.Elapsed;

            return TSpan.TotalMilliseconds;
        }

        // タイマー途中時間出力(マイクロ秒)
        public double MicrElapsed()
        {
            TSpan = SWatch.Elapsed;

            return (TSpan.TotalMilliseconds * 1000);
        }

        // タイマー途中時間出力(ナノ秒)
        public double NanoElapsed()
        {
            TSpan = SWatch.Elapsed;

            return (TSpan.TotalMilliseconds * 1000 * 1000);
        }

        /* 処理中断 */
        public static void Sleep( double mili )
        {
            var SW = new Stopwatch();
            TimeSpan TS = SW.Elapsed;
            
            // 計測開始
            SW.Start();

            // 引数時間(ミリ秒)分待機
            for (; TS.TotalMilliseconds < mili;)
            {
                // 現在の計測時間
                TS = SW.Elapsed;
            }
        }

    }
}
