using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MISOTEN_APPLICATION.Screen.CommonClass;

namespace MISOTEN_APPLICATION.BackProcess
{
    class GodConverter
    {
       
        //曲げセンサの値から角度を求める
        public float bendToAngle(float bend)
        {
            float angle = (-(-0.467009324f)
                +(float)Math.Sqrt(
                    Math.Pow(-0.467009324f,2)
                    -4.0f*(-0.25728287f)*(766.9972028f-bend)))
                    /(2.0f*(-0.25728287f));
            return angle;
        }

        //可変抵抗の値から角度を求める
        public float resistToAngle(float resist)
        {
            float angle = resist * (300 / 1024);
            return angle;
        }

        //スレーブ側の出力値を求める
        //Finger1:出力値を求めたい指
        //Finger2:出力値を求めたい指のとなりの指
        //Finger1とFinger2の間の距離
        public GOD_SENTENCE calc(GodFinger Finger1, GodFinger Finger2,int between_finger)
        {
            //変数宣言
            GOD_SENTENCE god_sentence = new GOD_SENTENCE();//指ごとの出力値
            double m_height = 0.0f;//マスター側の高さ
            double s1_height = 0.0f;//スレーブ側の高さ
            double s2_height = 0.0f;//スレーブ側のとなりの高さ
            double height = 0.0;//指の間の高さ
            double old_height = Finger1.hight;//前回のマスター側の高さ
            double m_angle3 = 0.0f;//マスター側の指の先端の角度
            double s1_angle3 = 0.0f;//スレーブ側の指の先端の角度
            double s2_angle3 = 0.0f;//スレーブ側のとなりの指の先端の角度

            double m_angle1 = resistToAngle(Finger1.getMSensorValue().second_joint);//マスター側の指の付け根の角度
            double m_angle2 = resistToAngle(Finger1.getMSensorValue().third_joint);//マスター側の指の中間の角度
            double s1_angle1 = bendToAngle(Finger1.getSSensorValue().second_joint);//スレーブ側の指の付け根の角度
            double s1_angle2 = resistToAngle(Finger1.getSSensorValue().third_joint);//スレーブ側の指の中間の角度
            double s2_angle1 = bendToAngle(Finger2.getSSensorValue().second_joint);//スレーブ側のとなりの指の付け根の角度
            double s2_angle2 = resistToAngle(Finger2.getSSensorValue().third_joint);//スレーブ側のとなりの指の中間の角度

            double ms_stating_value = resistToAngle(Finger1.getStatingValue().master.second);
            double mt_stating_value = resistToAngle(Finger1.getStatingValue().master.third);
            double s1s_stating_value = resistToAngle(Finger1.getStatingValue().slave.second);
            double s1t_stating_value = resistToAngle(Finger1.getStatingValue().slave.third);
            double s2s_stating_value = resistToAngle(Finger2.getStatingValue().slave.second);
            double s2t_stating_value = resistToAngle(Finger2.getStatingValue().slave.third);

            double rad1 = 0.0f;//angle1のラジアン
            double rad2 = 0.0f;//angle2のラジアン
            double rad3 = 0.0f;//angle3のラジアン

            int tip_pressure = Finger1.getMSensorValue().tip_pressure;//マスター側の先端の圧力
            int palm_pressure = Finger1.getMSensorValue().palm_pressure;//マスター側の手のひらの圧力
            int pressure = 0;//圧力の合計

            double density = 0.0;//密度係数

            float finger_width = between_finger;//指の間の距離

            double now_area = 0.0f;//今の面積
            double start_area = Finger1.stating_field;//ものを掴んだ時の面積
            double power = 0.0;//出力値
        
            float finger1 = Finger1.getLength().third;//指の付け根の距離
            float finger2 = Finger1.getLength().second;//指の中間の距離
            float finger3 = Finger1.getLength().first;//指の先端の距離

            m_angle1 = m_angle1 - ms_stating_value;
            m_angle2 = m_angle2 - mt_stating_value;
            s1_angle1 = s1_angle1 - s1s_stating_value;
            s1_angle2 = s1_angle2 - s1t_stating_value;
            s2_angle1 = s2_angle1 - s2s_stating_value;
            s2_angle2 = s2_angle2 - s2t_stating_value;


            //マスター側の高さ計算
            m_angle3 = m_angle2 * Constants.prop;
            rad1 = m_angle1 * Constants.pi / 180;
            rad1 = Math.Sin(rad1);
            rad2 = (m_angle1 + m_angle2) * Constants.pi / 180;
            rad2 = Math.Sin(rad2);
            rad3 = (m_angle1 + m_angle2 + m_angle3) * Constants.pi / 180;
            rad3 = Math.Sin(rad3);
            m_height = finger1 * rad1 + finger2 * rad2 + finger3 * rad3;
            // setHight関数
            Finger1.setHight((float)m_height);

            //スレーブ側の高さ計算
            s1_angle3 = s1_angle2 * Constants.prop;
            rad1 = s1_angle1 * Constants.pi / 180;
            rad1 = Math.Sin(rad1);
            rad2 = (s1_angle1 + s1_angle2) * Constants.pi / 180;
            rad2 = Math.Sin(rad2);
            rad3 = (s1_angle1 + s1_angle2 + s1_angle3) * Constants.pi / 180;
            rad3 = Math.Sin(rad3);
            s1_height = finger1 * rad1 + finger2 * rad2 + finger3 * rad3;

            //となりのスレーブ側の高さ計算
            s2_angle3 = s2_angle2 * Constants.prop;
            rad1 = s2_angle1 * Constants.pi / 180;
            rad1 = Math.Sin(rad1);
            rad2 = (s2_angle1 + s2_angle2) * Constants.pi / 180;
            rad2 = Math.Sin(rad2);
            rad3 = (s2_angle1 + s2_angle2 + s2_angle3) * Constants.pi / 180;
            rad3 = Math.Sin(rad3);
            s2_height = finger1 * rad1 + finger2 * rad2 + finger3 * rad3;

            //マスター側の密度計算
            m_height = Math.Abs(m_height - old_height);
            pressure = tip_pressure + palm_pressure;
            density = pressure / m_height;
            //スレーブ側の面積計算
            height = (s1_height + s2_height) / 2;
            finger_width = finger_width / 2;
            now_area = (s1_height + height) * (finger_width / 2);
            double max_area= 2 * (finger_width / 2);

            //出力値計算
            double max_power = ((start_area / max_area) * density) /2;
            power = ((start_area / now_area) * density)/2;
            double retio =  (now_area / max_area);

            god_sentence.tip_pwm = (int)Math.Round((255 * retio));
            god_sentence.palm_pwm = 500+(int)Math.Round((1900 * retio));
            return god_sentence;
        }

    }
}
