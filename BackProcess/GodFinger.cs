using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MISOTEN_APPLICATION.Screen.CommonClass;
namespace MISOTEN_APPLICATION.BackProcess
{
    class GodFinger
    {
        static SENSOR_VALUE master_data = new SENSOR_VALUE();
        static SENSOR_VALUE slave_data  = new SENSOR_VALUE();
        static STATING_VALUE stating_value = new STATING_VALUE();
        static JOINT ending_value = new JOINT();
        static JOINT pre_value = new JOINT();

        static LENGTH length = new LENGTH();
        //非同期処理用
        static PT_LOGS pt_logs = new PT_LOGS();

        //軌跡の移動量計算用
        public float stating_field = 0.0f;
        public float hight = 0.0f;
        public float width = 0.0f;

        public float now_traject = 0.0f;//今回の全体移動距離
        public float pre_traject = 0.0f;//前回の全体移動距離

        public int force = 0;//反力
        static float absolute_distance = 0.0f;//絶対移動距離

        //サーボモーターの限界値
        public int maxforce = 0;

        /* マスターセンサー値　setter */
        public void setMSensorValue(SENSOR_VALUE msensor_data) 
        {
            master_data = msensor_data;
        }

        /* スレーブセンサー値　setter */
        public void setSSensorValue(SENSOR_VALUE ssensor_data) 
        {
            slave_data = ssensor_data;
        }

        /* 広げた角度初期値　setter */
        public void setStatingValue(STATING_VALUE stating_data)
        {
            stating_value = stating_data;
        }

        /* 閉じた角度初期値　setter */
        public void setEndingValue(JOINT ending_data)
        {
            ending_value = ending_data;
        }

        /* 面積　setter */
        public void setField(float field) 
        {
            stating_field = field;
        }

        /* 高さ　setter */
        public void setHight(float high)
        {
            hight = high;
        }
        /* 横幅　setter */
        public void setWidth(float widh)
        {
            width = widh;
        }

        public void setLength(LENGTH _length){
            length=_length;
        }

        public void setPTLogs(PT_LOGS _pt_logs)
        {
            pt_logs = _pt_logs;
        }

        public void setAbsoluteDistance(float _absolute_distance)
        {
            absolute_distance = _absolute_distance;
        }

        public void setMaxForce(int _maxforce)
        {
            maxforce = _maxforce;
        }

        /* マスターセンサー値　getter */
        public SENSOR_VALUE getMSensorValue()
        {
            return master_data;
        }

        /* スレーブセンサー値　getter */
        public SENSOR_VALUE getSSensorValue()
        {
            return slave_data;
        }

        /* 広げた角度初期値　getter */
        public STATING_VALUE getStatingValue()
        {
            return stating_value;
        }

        /* 閉じた角度初期値　getter */
        public JOINT getEndingValue()
        {
            return ending_value;
        }

        /* 閉じた角度初期値　getter */
        public LENGTH getLength()
        {
            return length;
        }

        /* 面積　getter */
        public float getField()
        {
            return stating_field;
        }

        /* 高さ　getter */
        public float getHight()
        {
            return hight;
        }

        /* 前回の角度値　getter */
        public JOINT getPreValue()
        {
            return pre_value;
        }

        public PT_LOGS getPTLogs()
        {
            return pt_logs;
        }

        public float getAbsoluteDistance()
        {
            return absolute_distance;
        }

        /* public struct SENSOR_VALUE
         {
             public float    second_joint;
             public float    third_joint;
             public int      tip_pressure;
             public int      palm_pressure;
         }
         */
        /*public struct JOINT
        {
            public int      second;
            public int      third;
        }
        */
        /*public struct STATING_VALUE
        {
            JOINT master;
            JOINT slave;
        }*/

        /*public struct LENGTH
        {
            public float first ;
            public float second;
            public float third ;
            public LENGTH(float first, float second,float third)
            {
                this.first = first;
                this.second = second;
                this.third=third;
            }
        }
        */
    }
}
