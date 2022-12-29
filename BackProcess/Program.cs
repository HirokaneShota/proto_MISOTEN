using System.Runtime.ExceptionServices;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Diagnostics;
using System.Linq;
using MISOTEN_APPLICATION.Screen.CommonClass;
using System.Windows;
using ConsoleApp1;

namespace MISOTEN_APPLICATION.BackProcess
{
    static class Constants
    {
        //キャリブレーション許容誤差
        public const int second_joint_allowerror = 5000;
        public const int third_joint_allowerror = 5000;
        public const int tip_pressure_allowerror = 5000;
        public const int palm_pressure_allowerror = 5000;

        /*
        //スレーブ始動閾値
        public const float first_threshold_pressure = 0.0f;
        public const float second_threshold_pressure = 0.0f;
        public const float third_threshold_pressure = 0.0f;
        public const float fourth_threshold_pressure = 0.0f;
        public const float fifth_threshold_pressure = 0.0f;
        */
        //センサの最大値
        public const int sensor_max = 1024;
        public const double prop = 1.2;//比例定数
        public const double pi = 3.141592653589793;//π

    }


    // 限界地格納用構造体


    public class GodHand
    {
        FileClass file_t = new FileClass();
        FileClass file_angle = new FileClass();
        static async Task Main(string[] args)
        {
            {
                GodHand godhand = new GodHand();
            }
        }
        public LENGTH[] Length ={
            new LENGTH(1.0f,1.0f,1.0f),
            new LENGTH(1.0f,1.0f,1.0f),
            new LENGTH(1.0f,1.0f,1.0f),
            new LENGTH(1.0f,1.0f,1.0f),
            new LENGTH(1.0f,1.0f,1.0f)
        };
        public float[] absolute_distance = new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };
        public float[] all_traject = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        //動作している指
        //[0=小指][1=薬指][2=中指][3=人差し指][4=親指]
        public Boolean[] movement_finger = new Boolean[] { true, true, true, true, true };
        GodFinger[] godfinger = new GodFinger[5];
        DateBase dataBase = new DateBase();
        //指同士の距離
        public float[] between_fingeres = new float[] { 0.0f, 0.0f, 0.0f, };
        public GodHand()
        {
            file_t.DMFirst_csv();
            file_t.DSFirst_csv();
            file_angle.First_csv("angle");
            file_t.First_csv("send");
            for (int index = 0; index < 5; index++)
            {
                godfinger[index] = new GodFinger();
                godfinger[index].setLength(Length[index]);
                godfinger[index].setAbsoluteDistance(absolute_distance[index]);
                //データベース作成
                dataBase.TableCreate(index);
            }
            //データベース削除
            dataBase.DataAllDelete();
        }

        /*
         * 
         * 同期処理用
         * 
         */
        public async Task<int> run(SignalClass signalclass)
        {
            try
            {
                SENSOR_VALUE[] Temporary_masterdate = new SENSOR_VALUE[5];
                SENSOR_VALUE[] Temporary_slavedate = new SENSOR_VALUE[5];
                SignalClass signalClass = signalclass;
                GodConverter godconverter = new GodConverter();
                ReciveData_Sensor recivedata_sensor = new ReciveData_Sensor();
                JOINT[] finger_angle = new JOINT[5];

                recivedata_sensor = signalClass.GetMSensor();

                // センサー値書き込み(csvfile)
                file_t.MDLog_csv(recivedata_sensor);
                
                Temporary_masterdate[0] = recivedata_sensor.Little;
                Temporary_masterdate[1] = recivedata_sensor.Ring;
                Temporary_masterdate[2] = recivedata_sensor.Middle;
                Temporary_masterdate[3] = recivedata_sensor.Index;
                Temporary_masterdate[4] = recivedata_sensor.Thumb;
                finger_angle[4].third = godconverter.bendToAngle(Temporary_masterdate[4].third_joint);
                file_angle.Log_csv(Temporary_masterdate[4].third_joint.ToString(), finger_angle[4].third.ToString(), "\n");

                GOD_SENTENCE[] finger_power = new GOD_SENTENCE[5];
                GODS_SENTENCE gods_senten = new GODS_SENTENCE();
                for (int count = 0; count < 5; count++)
                {
                    int finger_count = count;
                    finger_power[finger_count] = godconverter.human_converter(Temporary_masterdate[finger_count]);
                }
                gods_senten.frist_godsentence = finger_power[0];
                gods_senten.second_godsentence = finger_power[1];
                gods_senten.third_godsentence = finger_power[2];
                gods_senten.fourth_godsentence = finger_power[3];
                gods_senten.fifth_godsentence = finger_power[4];

                // 50msスリープ  //await Task.Delay(100);
                TimerClass.Sleep(Time.OperatSTime);
                //スレーブに出力値を送信
                signalClass.SetSendMotor(gods_senten);
                // 出力値書き込み(csvfile)
                file_t.Log_csv(gods_senten.frist_godsentence.tip_pwm.ToString(), gods_senten.second_godsentence.tip_pwm.ToString(), gods_senten.third_godsentence.tip_pwm.ToString(), gods_senten.fifth_godsentence.tip_pwm.ToString(), gods_senten.fifth_godsentence.tip_pwm.ToString(), null, gods_senten.frist_godsentence.palm_pwm.ToString(), gods_senten.second_godsentence.palm_pwm.ToString(), gods_senten.third_godsentence.palm_pwm.ToString(), gods_senten.fifth_godsentence.palm_pwm.ToString(), gods_senten.fifth_godsentence.palm_pwm.ToString(), "\n");
                Debug.Print(gods_senten.frist_godsentence.tip_pwm.ToString()+","+ gods_senten.second_godsentence.tip_pwm.ToString()+","+ gods_senten.third_godsentence.tip_pwm.ToString()+","+ gods_senten.fifth_godsentence.tip_pwm.ToString()+","+ gods_senten.fifth_godsentence.tip_pwm.ToString()+","+gods_senten.frist_godsentence.palm_pwm.ToString()+","+ gods_senten.second_godsentence.palm_pwm.ToString()+","+ gods_senten.third_godsentence.palm_pwm.ToString()+","+ gods_senten.fifth_godsentence.palm_pwm.ToString()+","+ gods_senten.fifth_godsentence.palm_pwm.ToString()+",");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
            return 0;
        }

        /*
         * 
         * 非同期処理用
         * 
         */
        //サーボキャリブレーション
        public void servocalibration(GODS_SENTENCE _maxdate)
        {
            godfinger[0].setMaxForce(_maxdate.frist_godsentence.palm_pwm);
            godfinger[1].setMaxForce(_maxdate.second_godsentence.palm_pwm);
            godfinger[2].setMaxForce(_maxdate.third_godsentence.palm_pwm);
            godfinger[3].setMaxForce(_maxdate.fourth_godsentence.palm_pwm);
            godfinger[4].setMaxForce(_maxdate.fifth_godsentence.palm_pwm);
            return;
        }

        //キャリブレーション
        public async Task<int> calibration(int _calibration_select, SignalClass _signalclass)
        {
            //マスターのセンサーデータ格納
            SENSOR_VALUE[] result_master = new SENSOR_VALUE[5];
            //スレーブのセンサーデータ格納
            SENSOR_VALUE[] result_slave = new SENSOR_VALUE[5];
            //手を開いたときの初期位置データ
            STATING_VALUE[] result_start = new STATING_VALUE[5];
            //手を閉じたときの初期データ
            JOINT[] result_end = new JOINT[5];
            //初期の指の角度を格納
            JOINT[] pre_value = new JOINT[5];

            GodConverter godconverter = new GodConverter();

            //引数dでマスターかスレーブを選択
            if (_calibration_select == 0)
            {
                //開いた時のマスター処理
                Task task1 = Task.Run(() =>
                {
                    result_master = calibration_inspection(true, _signalclass).Result;
                    result_master[0].second_joint = godconverter.resistToAngle(result_master[0].second_joint);
                    result_master[0].third_joint = godconverter.resistToAngle(result_master[0].third_joint);
                    result_master[1].second_joint = godconverter.resistToAngle(result_master[1].second_joint);
                    result_master[1].third_joint = godconverter.resistToAngle(result_master[1].third_joint);
                    result_master[2].second_joint = godconverter.resistToAngle(result_master[2].second_joint);
                    result_master[2].third_joint = godconverter.resistToAngle(result_master[2].third_joint);
                    result_master[3].second_joint = godconverter.resistToAngle(result_master[3].second_joint);
                    result_master[3].third_joint = godconverter.resistToAngle(result_master[3].third_joint);
                    result_master[4].second_joint = godconverter.resistToAngle(result_master[4].second_joint);
                    result_master[4].third_joint = godconverter.bendToAngle(result_master[4].third_joint);

                    for (int index = 0; index < 5; index++)
                    {
                        int finger_count = index;
                        result_start[finger_count].master.second = result_master[finger_count].second_joint;
                        result_start[finger_count].master.third = result_master[finger_count].third_joint;
                        pre_value[finger_count].second = result_master[finger_count].second_joint;
                        pre_value[finger_count].third = result_master[finger_count].third_joint;
                        Console.WriteLine("task1:" + finger_count);
                    }
                });
                /*開いたときのスレーブ処理
                 * 
                Task task2 = Task.Run(() =>
                {
                    //スレーブ
                    result_slave = (calibration_inspection(false, _signalclass)).Result;
                    for (int index = 0; index < 5; index++)
                    {
                        int finger_count = index;
                        result_start[finger_count].slave.second = result_slave[finger_count].second_joint;
                        result_start[finger_count].slave.third = result_slave[finger_count].third_joint;
                        Console.WriteLine("task2:" + finger_count);
                    }
                });
                */
                await Task.WhenAll(task1);
                for (int index = 0; index < 5; index++)
                {
                    godfinger[index].setStatingValue(result_start[index]);
                    godfinger[index].setPreValue(pre_value[index]);
                    all_traject[index] = 0.0f;
                }
            }
            else
            {
                //開いたときのスレーブの処理
                Task task2 = Task.Run(() =>
                {
                    result_slave = calibration_inspection(false, _signalclass).Result;
                    result_slave[0].second_joint = godconverter.resistToAngle(result_slave[0].second_joint);
                    result_slave[0].third_joint = godconverter.bendToAngle(result_slave[0].third_joint);
                    result_slave[1].second_joint = godconverter.resistToAngle(result_slave[1].second_joint);
                    result_slave[1].third_joint = godconverter.bendToAngle(result_slave[1].third_joint);
                    result_slave[2].second_joint = godconverter.resistToAngle(result_slave[2].second_joint);
                    result_slave[2].third_joint = godconverter.bendToAngle(result_slave[2].third_joint);
                    result_slave[3].second_joint = godconverter.resistToAngle(result_slave[3].second_joint);
                    result_slave[3].third_joint = godconverter.bendToAngle(result_slave[3].third_joint);
                    result_slave[4].second_joint = godconverter.resistToAngle(result_slave[4].second_joint);
                    result_slave[4].third_joint = godconverter.bendToAngle(result_slave[4].third_joint);

                    for (int index = 0; index < 5; index++)
                    {
                        int finger_count = index;
                        result_start[finger_count].slave.second = result_slave[finger_count].second_joint;
                        result_start[finger_count].slave.third = result_slave[finger_count].third_joint;
                        pre_value[finger_count].second = result_slave[finger_count].second_joint;
                        pre_value[finger_count].third = result_slave[finger_count].third_joint;
                        Console.WriteLine("task2:" + finger_count);
                    }
                });
                await Task.WhenAll(task2);
                for (int index = 0; index < 5; index++)
                {
                    godfinger[index].setStatingValue(result_start[index]);
                    godfinger[index].setPreValue(pre_value[index]);
                    all_traject[index] = 0.0f;
                }
                //閉じたときのスレーブ処理
                /*
                result_slave = (calibration_inspection(false, _signalclass)).Result;
                for (int index = 0; index < 5; index++)
                {
                    Console.WriteLine("task1:" + index);
                    int finger_count = index;
                    result_end[finger_count].second = result_slave[finger_count].second_joint;
                    result_end[finger_count].third = result_slave[finger_count].third_joint;

                    godfinger[finger_count].setEndingValue(result_end[finger_count]);
                }
                */
            }
            return 0;

        }

        //キャリブレーション用メソッド
        public async Task<SENSOR_VALUE[]> calibration_inspection(Boolean _calibration_select, SignalClass _signalclass)
        {
            SENSOR_VALUE test = new SENSOR_VALUE() { second_joint = 0.0f, third_joint = 0.0f, tip_pressure = 0, palm_pressure = 0 };
            System.Timers.Timer timer = new System.Timers.Timer(3000);
            Boolean[] finger_true = new Boolean[5];
            //SENSOR_VALUE[0=小指][1=薬指][2=中指][3=人差し指][4=親指]
            SENSOR_VALUE[] Temporary_sensordate = new SENSOR_VALUE[5];
            SENSOR_VALUE[] receive_log = new SENSOR_VALUE[5];

            ReciveData_Sensor recivedata_sensor = new ReciveData_Sensor();
            Stopwatch sw = new Stopwatch();
            float truetime = 0.0f;
            int log_count = 0;
            SENSOR_VALUE[] predate = new SENSOR_VALUE[5];
            SENSOR_VALUE[] avaragedate = new SENSOR_VALUE[5];
            while (true)
            {
                //通信クラスから構造体リストの5つ分の構造体を取り出す
                if (_calibration_select)
                {
                    //getterで通信クラスからマスターの値を受け取る
                    Console.WriteLine("マスター");
                    recivedata_sensor = _signalclass.GetMSensor();
                }
                else
                {
                    //getterで通信クラスからスレーブの値を受け取る
                    Console.WriteLine("スレーブ");
                    recivedata_sensor = _signalclass.GetSSensor();
                    Debug.Print("小指:"+recivedata_sensor.Little.second_joint + "," + recivedata_sensor.Little.third_joint + "," + recivedata_sensor.Little.tip_pressure + "," + recivedata_sensor.Little.tip_pressure);
                }
                Temporary_sensordate[0] = recivedata_sensor.Little;
                Temporary_sensordate[1] = recivedata_sensor.Ring;
                Temporary_sensordate[2] = recivedata_sensor.Middle;
                Temporary_sensordate[3] = recivedata_sensor.Index;
                Temporary_sensordate[4] = recivedata_sensor.Thumb;
                //小指
                List<Task> arrayTask = new List<Task>();
                for (int count = 0; count < 5; count++)
                {
                    int finger_count = count;
                    Task finger = Task.Run(() =>
                    {
                        finger_true[finger_count] = finger_inspection(Temporary_sensordate[finger_count], predate[finger_count]);
                        receive_log[finger_count].second_joint += Temporary_sensordate[finger_count].second_joint;
                        receive_log[finger_count].third_joint += Temporary_sensordate[finger_count].third_joint;
                        receive_log[finger_count].tip_pressure += Temporary_sensordate[finger_count].tip_pressure;
                        receive_log[finger_count].palm_pressure += Temporary_sensordate[finger_count].palm_pressure;
                        log_count++;
                        predate[finger_count] = Temporary_sensordate[finger_count];
                    });
                    arrayTask.Add(finger);
                }
                await Task.WhenAll(arrayTask);
                arrayTask.Clear();

                if (finger_true.All(i => i == true))
                {
                    if (truetime == 0.0f)
                    {
                        sw.Start();
                        truetime = sw.ElapsedMilliseconds;
                    }
                    else
                    {
                        truetime = sw.ElapsedMilliseconds;
                        if (truetime >= 3000)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    sw.Restart();
                }
            }

            for (int index = 0; index < 5; index++)
            {
                avaragedate[index].second_joint = receive_log[index].second_joint / log_count;
                avaragedate[index].third_joint = receive_log[index].third_joint / log_count;
                avaragedate[index].tip_pressure = receive_log[index].tip_pressure / log_count;
                avaragedate[index].palm_pressure = receive_log[index].palm_pressure / log_count;
            }

            return avaragedate;
        }
        //キャリブレーション用メソッド
        public Boolean finger_inspection(SENSOR_VALUE _sensordate, SENSOR_VALUE _predate)
        {
            Boolean[] finger_true = new Boolean[] { false, false };
            if (Math.Abs(_sensordate.second_joint - _predate.second_joint) < Constants.second_joint_allowerror)
            {
                finger_true[0] = true;
            }
            if (Math.Abs(_sensordate.third_joint - _predate.third_joint) < Constants.third_joint_allowerror)
            {
                finger_true[1] = true;
            }
            if (finger_true.All(i => i == true))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //非同期処理のマスターデータ取得用メソッド
        public async Task<int> walk(SignalClass signalclass)
        {
            //マスター側のセンサデータを取得
            SENSOR_VALUE[] Temporary_masterdate = new SENSOR_VALUE[5];
            SignalClass signalClass = new SignalClass();
            GodConverter godconverter = new GodConverter();
            ReciveData_Sensor recivedata_sensor = new ReciveData_Sensor();
            recivedata_sensor = signalClass.GetMSensor();
            // センサー値書き込み(csvfile)
            file_t.MDLog_csv(recivedata_sensor);
            Temporary_masterdate[0] = recivedata_sensor.Little;
            Temporary_masterdate[1] = recivedata_sensor.Ring;
            Temporary_masterdate[2] = recivedata_sensor.Middle;
            Temporary_masterdate[3] = recivedata_sensor.Index;
            Temporary_masterdate[4] = recivedata_sensor.Thumb;
            //前回の指の角度
            JOINT[] pre_joint = new JOINT[5];//情報を入れる部分がない、情報が一つの指しかない
            //軌跡の移動量
            //float[] traject = new float[5];//情報が一つの指しかない
            JOINT[] now_joint = new JOINT[5];
            //指の角度に変換する部分がない
            Temporary_masterdate[0].second_joint = godconverter.resistToAngle(Temporary_masterdate[0].second_joint);
            Temporary_masterdate[0].third_joint = godconverter.resistToAngle(Temporary_masterdate[0].third_joint);
            Temporary_masterdate[1].second_joint = godconverter.resistToAngle(Temporary_masterdate[1].second_joint);
            Temporary_masterdate[1].third_joint = godconverter.resistToAngle(Temporary_masterdate[1].third_joint);
            Temporary_masterdate[2].second_joint = godconverter.resistToAngle(Temporary_masterdate[2].second_joint);
            Temporary_masterdate[2].third_joint = godconverter.resistToAngle(Temporary_masterdate[2].third_joint);
            Temporary_masterdate[3].second_joint = godconverter.resistToAngle(Temporary_masterdate[3].second_joint);
            Temporary_masterdate[3].third_joint = godconverter.resistToAngle(Temporary_masterdate[3].third_joint);
            Temporary_masterdate[4].second_joint = godconverter.resistToAngle(Temporary_masterdate[4].second_joint);
            Temporary_masterdate[4].third_joint = godconverter.bendToAngle(Temporary_masterdate[4].third_joint);

            List<Task> arrayTask = new List<Task>();
            for (int count = 0; count < 5; count++)
            {
                int finger_count = count;
                Task finger = Task.Run(() => {
                    //前回の指の角度を取得
                    pre_joint[finger_count] = godfinger[finger_count].getPreValue();
                    
                    godfinger[finger_count].setMSensorValue(Temporary_masterdate[finger_count]);

                    //軌跡の移動距離計算
                    if (pre_joint[finger_count].second + pre_joint[finger_count].third + (pre_joint[finger_count].second * 1.2) <
                        Temporary_masterdate[finger_count].second_joint + Temporary_masterdate[finger_count].third_joint +
                        (Temporary_masterdate[finger_count].second_joint * 1.2))
                    {
                        //マスターの指先の個々の軌跡を出力
                        all_traject[finger_count] += 
                        godconverter.angleToTrajectory(godfinger[finger_count], 0) * 1;
                    }
                    else
                    {
                        //マスターの指先の個々の軌跡を出力
                        all_traject[finger_count] += 
                        godconverter.angleToTrajectory(godfinger[finger_count], 0) * -1;
                    }
                    now_joint[finger_count].second = Temporary_masterdate[finger_count].second_joint;
                    now_joint[finger_count].third = Temporary_masterdate[finger_count].third_joint;
                    godfinger[finger_count].setPreValue(now_joint[finger_count]);
                });
                arrayTask.Add(finger);
            }
            await Task.WhenAll(arrayTask);
            arrayTask.Clear();
            //データベース格納
            for (int count = 0; count < 5; count++)
            {
                dataBase.DataInsert(count,all_traject[count],
                    Temporary_masterdate[count].tip_pressure, 
                    Temporary_masterdate[count].palm_pressure);
            }
            return 0;
        }
        
        //非同期処理のスレーブデータ取得およびデータ出力用
        public async Task<int> stand(SignalClass signalclass)
        {
            //スレーブ側のセンサデータを取得
            SENSOR_VALUE[] Temporary_slavedate = new SENSOR_VALUE[5];
            SignalClass signalClass = new SignalClass();
            GodConverter godconverter = new GodConverter();
            ReciveData_Sensor recivedata_sensor = new ReciveData_Sensor();
            GOD_SENTENCE[] finger_power = new GOD_SENTENCE[5];
            GODS_SENTENCE gods_senten = new GODS_SENTENCE();

            recivedata_sensor = signalClass.GetSSensor();
            // センサー値書き込み(csvfile)
            file_t.SDLog_csv(recivedata_sensor);
            Temporary_slavedate[0] = recivedata_sensor.Little;
            Temporary_slavedate[1] = recivedata_sensor.Ring;
            Temporary_slavedate[2] = recivedata_sensor.Middle;
            Temporary_slavedate[3] = recivedata_sensor.Index;
            Temporary_slavedate[4] = recivedata_sensor.Thumb;

            JOINT[] pre_joint = new JOINT[5];//前回の指の角度
            JOINT[] now_joint = new JOINT[5];

            float[] part_traject = new float[5];//軌跡の個別移動量
            PT_LOGS[] pt_logs = new PT_LOGS[5];
            int[] pressure = new int[] { 0, 0 };
            float[] test_data = new float[] { 1.0f, 1.0f, 1.0f, 1.0f ,1.0f};
            SENSOR_VALUE[] output = new SENSOR_VALUE[5];

            Temporary_slavedate[0].second_joint = godconverter.resistToAngle(Temporary_slavedate[0].second_joint);
            Temporary_slavedate[0].third_joint = godconverter.bendToAngle(Temporary_slavedate[0].third_joint);
            Temporary_slavedate[1].second_joint = godconverter.resistToAngle(Temporary_slavedate[1].second_joint);
            Temporary_slavedate[1].third_joint = godconverter.bendToAngle(Temporary_slavedate[1].third_joint);
            Temporary_slavedate[2].second_joint = godconverter.resistToAngle(Temporary_slavedate[2].second_joint);
            Temporary_slavedate[2].third_joint = godconverter.bendToAngle(Temporary_slavedate[2].third_joint);
            Temporary_slavedate[3].second_joint = godconverter.resistToAngle(Temporary_slavedate[3].second_joint);
            Temporary_slavedate[3].third_joint = godconverter.bendToAngle(Temporary_slavedate[3].third_joint);
            Temporary_slavedate[4].second_joint = godconverter.resistToAngle(Temporary_slavedate[4].second_joint);
            Temporary_slavedate[4].third_joint = godconverter.bendToAngle(Temporary_slavedate[4].third_joint);

            //各指の全体ンお軌跡の計算
            List<Task> arrayTask = new List<Task>();
            for (int count = 0; count < 5; count++)
            {
                Task finger = Task.Run(() => {

                    pre_joint[count] = godfinger[count].getPreValue();
                    godfinger[count].setSSensorValue(Temporary_slavedate[count]);
                    //軌跡の移動量
                    if (pre_joint[count].second + pre_joint[count].third + (pre_joint[count].second * 1.2) <
                        Temporary_slavedate[count].second_joint + Temporary_slavedate[count].third_joint +
                        (Temporary_slavedate[count].second_joint * 1.2))
                    {
                        part_traject[count] = godconverter.angleToTrajectory(godfinger[count], 1) * 1;
                        all_traject[count] += part_traject[count];
                    }
                    else
                    {
                        part_traject[count] = godconverter.angleToTrajectory(godfinger[count], 1) * -1;
                        all_traject[count] += part_traject[count];
                    }
                    now_joint[count].second = Temporary_slavedate[count].second_joint;
                    now_joint[count].third = Temporary_slavedate[count].third_joint;
                    godfinger[count].setPreValue(now_joint[count]);
                });
                arrayTask.Add(finger);
            }
            await Task.WhenAll(arrayTask);
            arrayTask.Clear();

            for (int count = 0; count < 5; count++)
            {
                pt_logs[count] = godfinger[count].getPTLogs();
                pt_logs[count].last_last_time = pt_logs[count].last_time;
                pt_logs[count].last_time = pt_logs[count].this_time;
                pt_logs[count].this_time.traject = part_traject[count];
                pressure = dataBase.DataIndex(count, all_traject[count]);
                pt_logs[count].this_time.pressure = pressure[0];
                godfinger[0].setPTLogs(pt_logs[count]);
                output[count].palm_pressure = pressure[1];
            }

            output[0].tip_pressure = godconverter.outFingerCalc(godfinger[0], godfinger[1], test_data[0]);
            output[1].tip_pressure = godconverter.innerfingercalc(godfinger[1], godfinger[0], godfinger[2], test_data[1]);
            output[2].tip_pressure = godconverter.innerfingercalc(godfinger[2], godfinger[2], godfinger[3], test_data[2]);
            output[3].tip_pressure = godconverter.outFingerCalc(godfinger[3], godfinger[2], test_data[3]);
            output[4].tip_pressure = godconverter.onlyFingerCalc(godfinger[4], godfinger[3], godfinger[2],
                                                                    godfinger[1], godfinger[0], between_fingeres,
                                                                    output[4].palm_pressure, test_data[4]);

            for(int count = 0; count < 5; count++)
            {
                finger_power[count] = godconverter.human_converter(output[count]);
            }

            gods_senten.frist_godsentence = finger_power[0];
            gods_senten.second_godsentence = finger_power[1];
            gods_senten.third_godsentence = finger_power[2];
            gods_senten.fourth_godsentence = finger_power[3];
            gods_senten.fifth_godsentence = finger_power[4];

            // 50msスリープ  //await Task.Delay(100);
            TimerClass.Sleep(Time.OperatSTime);
            //スレーブに出力値を送信
            signalClass.SetSendMotor(gods_senten);

            return 0;
        }
       
    }
}

/*
 * 不要メソッド
 * 
//圧力センサ始動監視
public async Task<int> Threshold_monitoring(SignalClass signalclass)
{
    SENSOR_VALUE[] Temporary_masterdate = new SENSOR_VALUE[5];
    SENSOR_VALUE[] Temporary_slavedate = new SENSOR_VALUE[5];
    SignalClass signalClass = signalclass;
    ReciveData_Sensor recivedata_sensor = new ReciveData_Sensor();
    while (true)
    {
        recivedata_sensor = signalClass.GetMSensor();
        Temporary_masterdate[0] = recivedata_sensor.Little;
        Temporary_masterdate[1] = recivedata_sensor.Ring;
        Temporary_masterdate[2] = recivedata_sensor.Middle;
        Temporary_masterdate[3] = recivedata_sensor.Index;
        Temporary_masterdate[4] = recivedata_sensor.Thumb;

        recivedata_sensor=signalClass.GetSSensor();
        Temporary_slavedate[0]=recivedata_sensor.Little;
        Temporary_slavedate[1]=recivedata_sensor.Ring;
        Temporary_slavedate[2]=recivedata_sensor.Middle;
        Temporary_slavedate[3]=recivedata_sensor.Index;
        Temporary_slavedate[4]=recivedata_sensor.Thumb;

        List<Task> arrayTask = new List<Task>();
        float[] startingpoint = new float[] { Constants.first_threshold_pressure, Constants.second_threshold_pressure, Constants.third_threshold_pressure, Constants.fourth_threshold_pressure, Constants.fifth_threshold_pressure };
        for (int count = 0; count < 5; count++)
        {
            int finger_count = count;
            if (movement_finger[finger_count] == false)
            {
                Task finger = Task.Run(() =>
                {
                    if (Temporary_masterdate[finger_count].second_joint == startingpoint[finger_count])
                    {
                        if (finger_count != 4)
                        {
                            movement_finger[finger_count] = true;//finger_starting(Temporary_slavedate[finger_count],Temporary_slavedate[finger_count+1],finger_count);
                        }
                        else
                        {
                            movement_finger[finger_count] = true;//finger_starting(Temporary_slavedate[finger_count],Temporary_slavedate[finger_count-1],finger_count);
                        }
                    }
                    //return;
                });
                arrayTask.Add(finger);
            }
        }
        await Task.WhenAll(arrayTask);
        arrayTask.Clear();
        //全指終了
        if (movement_finger.All(i => i == true))
        {
            Console.WriteLine("fingerall");
            return 0;
        }
    }
}
public Boolean finger_starting(SENSOR_VALUE _sensordate, SENSOR_VALUE _nextsensordate, int _finger_count)
{
    //float bandangle;
    //float resistangle;
    //float field_fingers;
    //float finger_height;
    //float nextfinger_height;
    //GodConverter godconverter=new GodConverter;
    //bandangle=godconverter.bendToAngle(_sensordate.tip_pressure);
    //resistangle=godconverter.resistToAngle(_sensordate.point_tip_pressure);
    //finger_height=forwardKinematics(Length,_sensordate.second_joint,_sensordate.third_joint);
    //nextfinger_height=forwardKinematics(Length[_finger_count],_nextsensordate.second_joint,_nextsensordate.third_joint);
    //field_fingers=godconverter.calcField(finger_height,nextfinger_height,);
    //gotfinger[_finger_count].setField(field_fingers);
    //gotfinger[_finger_count].setHeight(finger_height);
    Console.WriteLine("finger_starting");
    GodConverter godconverter = new GodConverter();
    float angle1 = godconverter.resistToAngle(_sensordate.third_joint);
    float angle2 = godconverter.resistToAngle(_sensordate.second_joint);
    float n_angle1 = godconverter.resistToAngle(_nextsensordate.third_joint);
    float n_angle2 = godconverter.resistToAngle(_nextsensordate.second_joint);
    float field_fingers;
    float finger_height;
    float nextfinger_height;
    finger_height = (float)(Length[_finger_count].first * Math.Sin(angle1 * (Math.PI / 180)) +
                    Length[_finger_count].second * Math.Sin(angle2 * (Math.PI / 180) + angle1 * (Math.PI / 180)) +
                    Length[_finger_count].third * Math.Sin((angle2 * (Math.PI / 180) * 1.2f) + angle2 * (Math.PI / 180) + angle1 * (Math.PI / 180)));

    nextfinger_height = (float)(Length[_finger_count].first * Math.Sin(n_angle1 * (Math.PI / 180)) +
                       Length[_finger_count].second * Math.Sin(n_angle2 * (Math.PI / 180) + n_angle1 * (Math.PI / 180)) +
                       Length[_finger_count].third * Math.Sin((n_angle2 * (Math.PI / 180) * 1.2f) + n_angle2 * (Math.PI / 180) + n_angle1 * (Math.PI / 180)));
    ////                     下底　　　　　　　　　　　　　　　　　　　　上底　　　　　　　　　　　高さ　　　　　　　　
    field_fingers = ((finger_height + nextfinger_height) / 2) + (finger_height) * (between_fingeres[_finger_count] / 2) / 2;
    godfinger[_finger_count].setField(field_fingers);
    godfinger[_finger_count].setHight(finger_height);
    return true;
}
*/