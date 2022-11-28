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

    /* 受信データ 数値 */
    class ReciveData_Short
    {
        public short ReciveData { get; set; }
    }
}
