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
            float angle = (-(-0.467009324f)
                +(float)Math.Sqrt(
                    Math.Pow(-0.467009324f,2)
                    -4.0f*(-0.25728287f)*(766.9972028f-bend)))
                    /(2.0f*(-0.25728287f));
            return angle;
        }

        //�ϒ�R�̒l����p�x�����߂�
        public float resistToAngle(float resist)
        {
            float angle = resist * (300 / 1024);
            return angle;
        }

        //�X���[�u���̏o�͒l�����߂�
        //Finger1:�o�͒l�����߂����w
        //Finger2:�o�͒l�����߂����w�̂ƂȂ�̎w
        //Finger1��Finger2�̊Ԃ̋���
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

    }
}
