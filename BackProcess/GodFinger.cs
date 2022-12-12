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

        static LENGTH length = new LENGTH();

        public float stating_field = 0.0f;
        public float hight = 0.0f;

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

        public void setLength(LENGTH _length){
            length=_length;
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
