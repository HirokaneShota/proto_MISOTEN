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
            float angle = ((0.00129313f * (float)Math.Pow(bend, 2)) +
                            (-1.4934458f * bend) + (431.20591f));
            
            return angle;
        }

        //可変抵抗の値から角度を求める
        public float resistToAngle(float resist)
        {
            float angle = resist * (300.0f / 1024.0f);
            return angle;
        }

        //指の角度から指の先が動いている軌跡を計算
        //引数：GodFingerクラス,マスターハンドorスレーブハンド
        //戻り値：軌跡の移動量
        public float angleToTrajectory(GodFinger finger, int judge)
        {
            SENSOR_VALUE sensor_value = new SENSOR_VALUE();

            float traject = 0.0f;//軌跡の移動量
            float vartical = 0.0f;//縦幅
            float horizontal = 0.0f;//横幅
            LENGTH length = finger.getLength();//指の長さ
            float hight = finger.hight;//前回の縦幅
            float width = finger.width;//前回の横幅
            //マスタースレーブ判別
            if (judge == 0)
            {
                sensor_value = finger.getMSensorValue();
            }
            else
            {
                sensor_value = finger.getSSensorValue();
            }
            //縦幅計算
            vartical = (float)(
                                (length.first * Math.Sin(sensor_value.third_joint)) +
                                (length.second * Math.Sin(sensor_value.third_joint + sensor_value.second_joint)) +
                                length.third * Math.Sin(sensor_value.third_joint + sensor_value.second_joint + (sensor_value.second_joint * 1.2))
                                );
            //横幅計算
            horizontal = (float)(
                                (length.first * Math.Cos(sensor_value.third_joint)) +
                                (length.second * Math.Cos(sensor_value.third_joint + sensor_value.second_joint)) +
                                length.third * Math.Cos(sensor_value.third_joint + sensor_value.second_joint + (sensor_value.second_joint * 1.2))
                                );
            //現在の縦幅セッター
            finger.setHight(vartical + hight);
            //現在の横幅セッター
            finger.setWidth(width - horizontal);
            //軌跡の移動量計算        
            traject = (float)Math.Sqrt(Math.Pow(vartical - hight, 2) + Math.Pow(width - horizontal, 2));

            return traject;
        }

        //人差し指、小指の計算
        public int outFingerCalc(GodFinger finger1,GodFinger finger2,float test_value)
        {
            int output = 0;
            PT_LOGS pt_logs1 = new PT_LOGS();
            PT_LOGS pt_logs2 = new PT_LOGS();
            pt_logs1 = finger1.getPTLogs();
            pt_logs2 = finger2.getPTLogs();

            float old_pressure = 0.0f;
            float difference = 0.0f;
            float area = 0.0f;
            float pressure_increase_ratio = 0.0f;

            if (pt_logs1.this_time.pressure == 0)
            {
                output = 0;
            }else if(pt_logs1.last_time.pressure == 0)
            {
                output = pt_logs1.this_time.pressure;
            }
            else
            {
                old_pressure = finger1.force;

                difference = (pt_logs1.this_time.pressure - pt_logs1.last_time.pressure);

                if(pt_logs2.this_time.pressure == 0 && pt_logs2.last_time.pressure == 0)
                {
                    area = 1.0f;
                }else if(pt_logs2.this_time.pressure == 0)
                {
                    area = (pt_logs1.last_time.pressure +
                        ((pt_logs1.last_time.pressure + pt_logs2.last_time.pressure) / 2) + test_value) /
                        ((pt_logs1.this_time.pressure / 2) + test_value);
                }else if(pt_logs2.last_time.pressure == 0)
                {
                    area = ((pt_logs1.last_time.pressure / 2) + test_value) /
                        (pt_logs1.this_time.pressure +
                        ((pt_logs1.this_time.pressure + pt_logs2.this_time.pressure) / 2) + test_value);
                }
                else
                {
                    area = (pt_logs1.last_time.pressure +
                        ((pt_logs1.last_time.pressure + pt_logs2.last_time.pressure) / 2) + test_value) /
                        (pt_logs1.this_time.pressure +
                        ((pt_logs1.this_time.pressure + pt_logs2.this_time.pressure) / 2) + test_value);
                }

                pressure_increase_ratio = (Math.Abs(pt_logs1.this_time.pressure - pt_logs1.last_time.pressure) /
                    Math.Abs(pt_logs1.this_time.traject - pt_logs1.last_time.traject)) /
                    (Math.Abs(pt_logs1.last_time.pressure - pt_logs1.last_last_time.pressure) /
                    Math.Abs(pt_logs1.last_time.traject - pt_logs1.last_last_time.traject));

                output = (int)(old_pressure + difference * area * pressure_increase_ratio);
            }

            return output;
        }

        public int innerfingercalc(GodFinger main_finger, GodFinger first_finger, GodFinger second_finger, float test_value)
        {
            int reaction_force = 0;
            float first_finger_field = 0.0f;
            float second_finger_field = 0.0f;
            int pre_pressure_diff = 0;
            int pressure_diff = 0;
            float calc_field = 0.0f;
            float pres_upper_ration = 0.0f;
            float[] molecule = new float[] { 0.0f, 0.0f };
            float[] denominato = new float[] { 0.0f, 0.0f };
            PT_LOGS first_ptlogs;
            PT_LOGS second_ptlogs;
            PT_LOGS main_ptlogs;
            main_ptlogs = main_finger.getPTLogs();
            first_ptlogs = first_finger.getPTLogs();
            second_ptlogs = second_finger.getPTLogs();

            //一つ目の分子と分母の計算
            if (first_ptlogs.last_time.pressure == 0)
            {
                molecule[0] = first_finger.pre_traject / 2;
            }
            else
            {
                molecule[0] = main_finger.pre_traject + (first_finger.pre_traject + main_finger.pre_traject) / 2;
            }
            if (first_ptlogs.this_time.pressure == 0)
            {
                denominato[0] = first_finger.now_traject / 2;
            }
            else
            {
                denominato[0] = first_finger.now_traject + (first_finger.now_traject + main_finger.now_traject) / 2;
            }

            //一つ目の面積
            if (first_ptlogs.last_time.pressure == 0 && first_ptlogs.this_time.pressure == 0)
            {
                molecule[0] = 1.0f;
                denominato[0] = 1.0f;
            }

            //二つ目の分子と分母の計算
            if (second_ptlogs.last_time.pressure == 0)
            {
                molecule[1] = second_finger.pre_traject / 2;
            }
            else
            {
                molecule[1] = main_finger.pre_traject + (second_finger.pre_traject + main_finger.pre_traject) / 2;
            }
            if (second_ptlogs.this_time.pressure == 0)
            {
                denominato[1] = second_ptlogs.this_time.traject / 2;
            }
            else
            {
                denominato[1] = main_finger.now_traject + (second_finger.now_traject + main_finger.now_traject) / 2;
            }

            //二つ目の面積
            if (second_ptlogs.last_time.pressure == 0 && second_ptlogs.this_time.pressure == 0)
            {
                molecule[1] = 1.0f;
                denominato[1] = 1.0f;
            }
            pressure_diff = (main_ptlogs.this_time.pressure - main_ptlogs.last_time.pressure);
            pre_pressure_diff = (main_ptlogs.last_time.pressure - main_ptlogs.last_last_time.pressure);
            calc_field = (molecule[0] + molecule[1] + test_value) / (denominato[0] + denominato[1] + test_value);
            pres_upper_ration = ((Math.Abs(pressure_diff) / Math.Abs(main_ptlogs.this_time.traject - main_ptlogs.last_time.traject)) / (Math.Abs(pre_pressure_diff) / Math.Abs(main_ptlogs.last_time.traject - main_ptlogs.last_last_time.traject)));
            reaction_force = (int)(main_finger.force + (pressure_diff * calc_field * pres_upper_ration));

            return reaction_force;
        }

        //親指の計算
        public int onlyFingerCalc(GodFinger finger1, GodFinger finger2,
                                GodFinger finger3, GodFinger finger4,
                                GodFinger finger5, float[] between_fingers, 
                                int palm_pressure, float test_value)
        {
            int output = 0;
            PT_LOGS pt_logs1 = new PT_LOGS();
            PT_LOGS pt_logs2 = new PT_LOGS();
            PT_LOGS pt_logs3 = new PT_LOGS();
            PT_LOGS pt_logs4 = new PT_LOGS();
            PT_LOGS pt_logs5 = new PT_LOGS();
            pt_logs1 = finger1.getPTLogs();
            pt_logs2 = finger2.getPTLogs();
            pt_logs3 = finger3.getPTLogs();
            pt_logs4 = finger4.getPTLogs();
            pt_logs5 = finger5.getPTLogs();

            float[] traject = new float[]
            {
                finger2.now_traject,
                finger3.now_traject,
                finger4.now_traject,
                finger5.now_traject
            };
            float[] absolute_value = new float[]
            {
                finger2.getAbsoluteDistance(),
                finger3.getAbsoluteDistance(),
                finger4.getAbsoluteDistance(),
                finger5.getAbsoluteDistance(),
            };
            float total_move = 0.0f;
            int average_value = 0;

            if (pt_logs1.this_time.pressure == 0)
            {
                output = 0;
            }
            else
            {
                for(int count = 0; count < 4; count++)
                {
                    if(traject[count] != 0.0f)
                    {
                        total_move = total_move + (absolute_value[count] - traject[count]);
                        average_value++;
                    }
                }
                total_move = total_move / average_value;

                output = (int)(pt_logs1.this_time.pressure +
                    (
                    pt_logs2.this_time.pressure *
                    (total_move / (Math.Sqrt(Math.Pow(between_fingers[0], 2) + Math.Pow(total_move, 2))))
                    ) + 
                    pt_logs3.this_time.pressure +
                    (
                    pt_logs4.this_time.pressure *
                    (total_move / (Math.Sqrt(Math.Pow(between_fingers[1], 2) + Math.Pow(total_move, 2))))
                    ) +
                    (
                    pt_logs5.this_time.pressure *
                    (total_move / (Math.Sqrt(Math.Pow(between_fingers[1]+between_fingers[2], 2) + Math.Pow(total_move, 2))))
                    ) + 
                    (palm_pressure * test_value)
                    );
            }

            return output;
        }

        //スレーブ側の出力値を求める
        //Finger1:出力値を求めたい指
        //Finger2:出力値を求めたい指のとなりの指
        //Finger1とFinger2の間の距離
        /*
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
        */
        public GOD_SENTENCE human_converter(SENSOR_VALUE finger)
        {
            GOD_SENTENCE god_sentence = new GOD_SENTENCE();
            god_sentence.tip_pwm = (int)Math.Round(((double)finger.tip_pressure/1024.0f)*255.0);
            god_sentence.palm_pwm = 500 + (int)Math.Round((((double)finger.palm_pressure)/1024.0f)*2000.0);
            return god_sentence;
        }

    }
}
