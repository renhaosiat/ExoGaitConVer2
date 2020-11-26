﻿using System;
using System.Text;
using System.IO;
using CMLCOMLib;
using System.Windows.Threading;
using System.Threading;
using System.Threading.Tasks;


namespace ExoGaitMonitorVer2
{
    class PVT
    {

        public void StartPVT(Motors motors,  string adress,double unit,int timevalue1,int timevalue2)
        {
            #region 
            //计算轨迹位置，速度和时间间隔序列
            //原始数据
           
            string[] ral = File.ReadAllLines(adress, Encoding.Default); //相对目录是在bin/Debug下，所以要回溯到上两级目录
            int lineCounter = ral.Length; //获取步态数据行数
            string[] col = (ral[0] ?? string.Empty).Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
            int colCounter = col.Length; //获取步态数据列数
            double[,] pos0 = new double[lineCounter, colCounter]; //原始位置数据
            double[,] vel = new double[lineCounter, colCounter]; //速度
            int[] times = new int[lineCounter]; //时间间隔
            for (int i = 0; i < lineCounter; i++)
            {
                
                    times[i] =timevalue2 ; //【设置】时间间隔
                
                
                string[] str = (ral[i] ?? string.Empty).Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < colCounter; j++)
                {
                    pos0[i, j] = double.Parse(str[j]) / (unit / motors.RATIO) * motors.userUnits[j] * -1;
                }
            }
            times[lineCounter - 1] = 0;
            #region
            //一次扩充
            //int richLine = lineCounter * 2 - 1;
            //double[,] pos1 = new double[richLine, colCounter]; //一次扩充位置数据
            //for (int i = 0; i < richLine; i++)
            //{
            //    for (int j = 0; j < colCounter; j++)
            //    {
            //        if (i % 2 == 0)//偶数位存放原始数据
            //        {
            //            pos1[i, j] = pos0[i / 2, j];
            //        }
            //        else//奇数位存放扩充数据
            //        {
            //            pos1[i, j] = (pos0[i / 2 + 1, j] + pos0[i / 2, j]) / 2.0;
            //        }
            //    }
            //}

            ////二次扩充
            //int rich2Line = richLine * 2 - 1;
            //double[,] pos2 = new double[rich2Line, colCounter]; //二次扩充位置数据
            //double[,] vel = new double[rich2Line, colCounter]; //速度
            //int[] times = new int[rich2Line]; //时间间隔
            //for (int i = 0; i < rich2Line; i++)
            //{
            //    times[i] =3; //【设置】时间间隔
            //    for (int j = 0; j < colCounter; j++)
            //    {

            //        if (i % 2 == 0)//偶数位存放原始数据
            //        {
            //            pos2[i, j] = pos1[i / 2, j];
            //        }
            //        else//奇数位存放扩充数据
            //        {
            //            pos2[i, j] = (pos1[i / 2 + 1, j] + pos1[i / 2, j]) / 2.0;
            //        }
            //    }
            //}

            //三次扩充
            //int rich3Line = rich2Line * 2 - 1;

            //double[,] pos3 = new double[rich2Line, colCounter]; //三次扩充位置数据

            //for (int i = 0; i < rich3Line; i++)
            //{

            //    for (int j = 0; j < colCounter; j++)
            //    {
            //        if (i % 2 == 0)//偶数位存放原始数据
            //        {
            //            pos3[i, j] = pos2[i / 2, j];
            //        }
            //        else//奇数位存放扩充数据
            //        {
            //            pos3[i, j] = (pos2[i / 2 + 1, j] + pos2[i / 2, j]) / 2.0;

            //        }
            //        //pos3[i, j] = pos3[i, j] - pos3[0, j];
            //    }
            //}
            #endregion

            for (int i = 0; i < lineCounter - 1; i++)
            {
                for (int j = 0; j < colCounter; j++)
                {

                    vel[i, j] = (pos0[i + 1, j] - pos0[i, j]) * 1000.0 / (times[i]);
                }
            }
            vel[lineCounter - 1, 0] = 0;
            vel[lineCounter - 1, 1] = 0;
            vel[lineCounter - 1, 2] = 0;
            vel[lineCounter - 1, 3] = 0;
            #endregion
            //for (int i = 0; i < motors.motor_num; i++)//开始步态前各电机回到轨迹初始位置
            //{
            //    motors.profileSettingsObj = motors.ampObj[i].ProfileSettings;
            //    motors.profileSettingsObj.ProfileAccel = (motors.ampObj[i].VelocityLoopSettings.VelLoopMaxAcc);
            //    motors.profileSettingsObj.ProfileDecel = (motors.ampObj[i].VelocityLoopSettings.VelLoopMaxDec);
            //    motors.profileSettingsObj.ProfileVel = (motors.ampObj[i].VelocityLoopSettings.VelLoopMaxVel * 0.8);
            //    motors.profileSettingsObj.ProfileType = CML_PROFILE_TYPE.PROFILE_TRAP; //PVT模式下的控制模式类型
            //    motors.ampObj[i].ProfileSettings = motors.profileSettingsObj;
            //    motors.ampObj[i].MoveAbs(pos0[0, i]); //PVT模式开始后先移动到各关节初始位置
            //    motors.ampObj[i].WaitMoveDone(10000); //等待各关节回到初始位置的最大时间
            //}

            motors.Linkage.TrajectoryInitialize(pos0, vel, times, 500); //开始步态
            motors.ampObj[0].WaitMoveDone(10000);
            motors.ampObj[1].WaitMoveDone(10000);
            motors.ampObj[2].WaitMoveDone(10000);
            motors.ampObj[3].WaitMoveDone(10000);


        }
        public void start_Sitdown2(Motors motor)
        {
            #region
              double[,] KeyPos ={ { -56888.88889, 227555.5556, -227555.5556, 56888.88889 },
                                { -159288.8889, 599608.8889, -599608.8889, 159288.8889 },
                                { -349297.7778, 893155.5556, -893155.5556, 349297.7778 },
                                { -728177.7778, 1169635.556, -1169635.556, 728177.7778 },
                                { -989866.6667, 1365333.333, -1365333.333, 989866.6667 },
                                { -1035377.778, 1285688.889, -1285688.889, 1035377.778 },
                                { -1137777.778, 1196942.222, -1196942.222, 1137777.778 }};
                #endregion
                ProfileSettingsObj profileParameters = new ProfileSettingsObj();    //用于设置电机参数
                double MotorVelocity = 72;
                double MotorAcceleration = 20;
                double MotorDeceleration = 20;

                double[,] DeltaP = new double[7, 4];
                for (int s = 0; s < 7; s++)
                {
                    for (int j = 0; j < motor.motor_num; j++)
                    {
                        if (s == 0)
                        {
                            DeltaP[s, j] = Math.Abs(KeyPos[s, j] - 0);
                        }
                        else
                        {
                            DeltaP[s, j] = Math.Abs(KeyPos[s, j] - KeyPos[s - 1, j]);
                        }

                    }
                    for (int i = 0; i < motor.motor_num; i++)
                    {
                        double MaxDeltaP = DeltaP[s, 0];
                        if (MaxDeltaP < DeltaP[s, i])
                        {
                            MaxDeltaP = DeltaP[s, i];
                        }
                        profileParameters = motor.ampObj[i].ProfileSettings;
                        profileParameters.ProfileVel = MotorVelocity * 6400 * 4 * 160 / 360 * DeltaP[s, i] / MaxDeltaP;    //单位为°/s
                        profileParameters.ProfileAccel = MotorAcceleration * 6400 * 4 * 160 / 360;    //单位为°/s2
                        profileParameters.ProfileDecel = MotorDeceleration * 6400 * 4 * 160 / 360;    //单位为°/s
                        profileParameters.ProfileType = CML_PROFILE_TYPE.PROFILE_TRAP;
                        motor.ampObj[i].ProfileSettings = profileParameters;
                        motor.ampObj[i].MoveAbs(KeyPos[s, i]);
                    }
                  
                }
                                   
        }
        public void start_Standup2(Motors motor)
        {
            #region
            double[,] KeyPos ={ { -1132088.889 , 1228800, -1228800 ,1132088.889 },
                                { -1120711.111, 1251555.556, -1251555.556 ,1120711.111 },
                                { -1092266.667, 1308444.444, -1308444.444,1092266.667 },
                                { -1069511.111 , 1388088.889, -1388088.889,1069511.111},
                                { -932977.7778, 1331200, -1331200,   932977.7778},
                                { -762311.1111 ,1228800, -1228800,   762311.1111},
                                { -568888.8889, 955733.3333, -955733.3333 , 568888.8889},
                                {-443733.3333,  762311.1111, -762311.1111 , 443733.3333 },
                                {-113777.7778, 432355.5556, -432355.5556 , 113777.7778  },
                                { -56888.88889, 227555.5556, -227555.5556, 56888.88889 },
                                {0 ,  0 ,  0 ,  0  } };
            #endregion
            ProfileSettingsObj profileParameters = new ProfileSettingsObj();    //用于设置电机参数
                double MotorVelocity = 82;
                double MotorAcceleration = 20;
                double MotorDeceleration = 20;

                double[,] DeltaP = new double[11, 4];
                for (int s = 0; s < 11; s++)
                {
                    for (int j = 0; j < motor.motor_num; j++)
                    {
                        if (s == 0)
                        {
                            DeltaP[s, j] = Math.Abs(KeyPos[s, j] - motor.ampObj[j].PositionActual);
                        }
                        else
                        {
                            DeltaP[s, j] = Math.Abs(KeyPos[s, j] - KeyPos[s - 1, j]);
                        }

                    }
                    for (int i = 0; i < motor.motor_num; i++)
                    {
                        double MaxDeltaP = DeltaP[s, 0];
                        if (MaxDeltaP < DeltaP[s, i])
                        {
                            MaxDeltaP = DeltaP[s, i];
                        }
                        profileParameters = motor.ampObj[i].ProfileSettings;
                        profileParameters.ProfileVel = MotorVelocity * 6400 * 4 * 160 / 360 * DeltaP[s, i] / MaxDeltaP;    //单位为°/s
                        profileParameters.ProfileAccel = MotorAcceleration * 6400 * 4 * 160 / 360;    //单位为°/s2
                        profileParameters.ProfileDecel = MotorDeceleration * 6400 * 4 * 160 / 360;    //单位为°/s
                        profileParameters.ProfileType = CML_PROFILE_TYPE.PROFILE_TRAP;
                        motor.ampObj[i].ProfileSettings = profileParameters;
                        motor.ampObj[i].MoveAbs(KeyPos[s, i]);
                    }

                }
                        
             }
        }
    }
