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
        

        //�Ȃ��Z���T�̒l����p�x�����߂�
        public float bendToAngle(float bend)
        {
            float angle = ((0.00129313f * (float)Math.Pow(bend, 2)) +
                            (-1.4934458f * bend) + (431.20591f));
            
            return angle;
        }

        //�ϒ�R�̒l����p�x�����߂�
        public float resistToAngle(float resist)
        {
            float angle = resist * (300.0f / 1024.0f);
            return angle;
        }

        //�w�̊p�x����w�̐悪�����Ă���O�Ղ��v�Z
        //�����FGodFinger�N���X,�}�X�^�[�n���hor�X���[�u�n���h
        //�߂�l�F�O�Ղ̈ړ���
        public float angleToTrajectory(GodFinger finger, int judge)
        {
            SENSOR_VALUE sensor_value = new SENSOR_VALUE();

            float traject = 0.0f;//�O�Ղ̈ړ���
            float vartical = 0.0f;//�c��
            float horizontal = 0.0f;//����
            LENGTH length = finger.getLength();//�w�̒���
            float hight = finger.hight;//�O��̏c��
            float width = finger.width;//�O��̉���
            //�}�X�^�[�X���[�u����
            if (judge == 0)
            {
                sensor_value = finger.getMSensorValue();
            }
            else
            {
                sensor_value = finger.getSSensorValue();
            }
            //�c���v�Z
            vartical = (float)(
                                (length.first * Math.Sin(sensor_value.third_joint)) +
                                (length.second * Math.Sin(sensor_value.third_joint + sensor_value.second_joint)) +
                                length.third * Math.Sin(sensor_value.third_joint + sensor_value.second_joint + (sensor_value.second_joint * 1.2))
                                );
            //�����v�Z
            horizontal = (float)(
                                (length.first * Math.Cos(sensor_value.third_joint)) +
                                (length.second * Math.Cos(sensor_value.third_joint + sensor_value.second_joint)) +
                                length.third * Math.Cos(sensor_value.third_joint + sensor_value.second_joint + (sensor_value.second_joint * 1.2))
                                );
            //���݂̏c���Z�b�^�[
            finger.setHight(vartical + hight);
            //���݂̉����Z�b�^�[
            finger.setWidth(width - horizontal);
            //�O�Ղ̈ړ��ʌv�Z        
            traject = (float)Math.Sqrt(Math.Pow(vartical - hight, 2) + Math.Pow(width - horizontal, 2));

            return traject;
        }

        //�l�����w�A���w�̌v�Z
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

            //��ڂ̕��q�ƕ���̌v�Z
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

            //��ڂ̖ʐ�
            if (first_ptlogs.last_time.pressure == 0 && first_ptlogs.this_time.pressure == 0)
            {
                molecule[0] = 1.0f;
                denominato[0] = 1.0f;
            }

            //��ڂ̕��q�ƕ���̌v�Z
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

            //��ڂ̖ʐ�
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

        //�e�w�̌v�Z
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

        //�X���[�u���̏o�͒l�����߂�
        //Finger1:�o�͒l�����߂����w
        //Finger2:�o�͒l�����߂����w�̂ƂȂ�̎w
        //Finger1��Finger2�̊Ԃ̋���
        /*
        public GOD_SENTENCE calc(GodFinger Finger1, GodFinger Finger2,int between_finger)
        {
            //�ϐ��錾
            GOD_SENTENCE god_sentence = new GOD_SENTENCE();//�w���Ƃ̏o�͒l
            double m_height = 0.0f;//�}�X�^�[���̍���
            double s1_height = 0.0f;//�X���[�u���̍���
            double s2_height = 0.0f;//�X���[�u���̂ƂȂ�̍���
            double height = 0.0;//�w�̊Ԃ̍���
            double old_height = Finger1.hight;//�O��̃}�X�^�[���̍���
            double m_angle3 = 0.0f;//�}�X�^�[���̎w�̐�[�̊p�x
            double s1_angle3 = 0.0f;//�X���[�u���̎w�̐�[�̊p�x
            double s2_angle3 = 0.0f;//�X���[�u���̂ƂȂ�̎w�̐�[�̊p�x

            double m_angle1 = resistToAngle(Finger1.getMSensorValue().second_joint);//�}�X�^�[���̎w�̕t�����̊p�x
            double m_angle2 = resistToAngle(Finger1.getMSensorValue().third_joint);//�}�X�^�[���̎w�̒��Ԃ̊p�x
            double s1_angle1 = bendToAngle(Finger1.getSSensorValue().second_joint);//�X���[�u���̎w�̕t�����̊p�x
            double s1_angle2 = resistToAngle(Finger1.getSSensorValue().third_joint);//�X���[�u���̎w�̒��Ԃ̊p�x
            double s2_angle1 = bendToAngle(Finger2.getSSensorValue().second_joint);//�X���[�u���̂ƂȂ�̎w�̕t�����̊p�x
            double s2_angle2 = resistToAngle(Finger2.getSSensorValue().third_joint);//�X���[�u���̂ƂȂ�̎w�̒��Ԃ̊p�x

            double ms_stating_value = resistToAngle(Finger1.getStatingValue().master.second);
            double mt_stating_value = resistToAngle(Finger1.getStatingValue().master.third);
            double s1s_stating_value = resistToAngle(Finger1.getStatingValue().slave.second);
            double s1t_stating_value = resistToAngle(Finger1.getStatingValue().slave.third);
            double s2s_stating_value = resistToAngle(Finger2.getStatingValue().slave.second);
            double s2t_stating_value = resistToAngle(Finger2.getStatingValue().slave.third);

            double rad1 = 0.0f;//angle1�̃��W�A��
            double rad2 = 0.0f;//angle2�̃��W�A��
            double rad3 = 0.0f;//angle3�̃��W�A��

            int tip_pressure = Finger1.getMSensorValue().tip_pressure;//�}�X�^�[���̐�[�̈���
            int palm_pressure = Finger1.getMSensorValue().palm_pressure;//�}�X�^�[���̎�̂Ђ�̈���
            int pressure = 0;//���͂̍��v

            double density = 0.0;//���x�W��

            float finger_width = between_finger;//�w�̊Ԃ̋���

            double now_area = 0.0f;//���̖ʐ�
            double start_area = Finger1.stating_field;//���̂�͂񂾎��̖ʐ�
            double power = 0.0;//�o�͒l
        
            float finger1 = Finger1.getLength().third;//�w�̕t�����̋���
            float finger2 = Finger1.getLength().second;//�w�̒��Ԃ̋���
            float finger3 = Finger1.getLength().first;//�w�̐�[�̋���

            m_angle1 = m_angle1 - ms_stating_value;
            m_angle2 = m_angle2 - mt_stating_value;
            s1_angle1 = s1_angle1 - s1s_stating_value;
            s1_angle2 = s1_angle2 - s1t_stating_value;
            s2_angle1 = s2_angle1 - s2s_stating_value;
            s2_angle2 = s2_angle2 - s2t_stating_value;


            //�}�X�^�[���̍����v�Z
            m_angle3 = m_angle2 * Constants.prop;
            rad1 = m_angle1 * Constants.pi / 180;
            rad1 = Math.Sin(rad1);
            rad2 = (m_angle1 + m_angle2) * Constants.pi / 180;
            rad2 = Math.Sin(rad2);
            rad3 = (m_angle1 + m_angle2 + m_angle3) * Constants.pi / 180;
            rad3 = Math.Sin(rad3);
            m_height = finger1 * rad1 + finger2 * rad2 + finger3 * rad3;
            // setHight�֐�
            Finger1.setHight((float)m_height);

            //�X���[�u���̍����v�Z
            s1_angle3 = s1_angle2 * Constants.prop;
            rad1 = s1_angle1 * Constants.pi / 180;
            rad1 = Math.Sin(rad1);
            rad2 = (s1_angle1 + s1_angle2) * Constants.pi / 180;
            rad2 = Math.Sin(rad2);
            rad3 = (s1_angle1 + s1_angle2 + s1_angle3) * Constants.pi / 180;
            rad3 = Math.Sin(rad3);
            s1_height = finger1 * rad1 + finger2 * rad2 + finger3 * rad3;

            //�ƂȂ�̃X���[�u���̍����v�Z
            s2_angle3 = s2_angle2 * Constants.prop;
            rad1 = s2_angle1 * Constants.pi / 180;
            rad1 = Math.Sin(rad1);
            rad2 = (s2_angle1 + s2_angle2) * Constants.pi / 180;
            rad2 = Math.Sin(rad2);
            rad3 = (s2_angle1 + s2_angle2 + s2_angle3) * Constants.pi / 180;
            rad3 = Math.Sin(rad3);
            s2_height = finger1 * rad1 + finger2 * rad2 + finger3 * rad3;

            //�}�X�^�[���̖��x�v�Z
            m_height = Math.Abs(m_height - old_height);
            pressure = tip_pressure + palm_pressure;
            density = pressure / m_height;
            //�X���[�u���̖ʐόv�Z
            height = (s1_height + s2_height) / 2;
            finger_width = finger_width / 2;
            now_area = (s1_height + height) * (finger_width / 2);
            double max_area= 2 * (finger_width / 2);

            //�o�͒l�v�Z
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
