using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARS408_RadarTools
{
    public static class ARS408_State
    {
        /// <summary>
        /// 雷达状态
        /// </summary>
        public static class Radar_State
        {
            public static int NVMReadStatus;
            public static int NVMwriteStatus;
            public static int MaxDistanceCfg;
            public static int SensorID;
            public static int SortIndex;
            public static int RadarPowerCfg;
            public static int CtrlRelayCfg;
            public static int OutputTypeCfg;
            public static int SendQualityCfg;
            public static int SendExtInfoCfg;
            public static int MotionRxState;
            public static int RCS_Threshold;
        }
        /// <summary>
        /// 过滤器状态
        /// </summary>
        public static class Filter_State
        {
            public static int NofClusterFilterCfg;//集群过滤器数量
            public static int NofObjectFilterCfg; //目标过滤器数量

            public static int Active;//过滤器激活开关
            public static int Index;
            public static int Type;
            public static int Min_NofObj;
            public static int Min_Distance;
            public static int Min_Azimuth;
            public static int Min_VrelOncome;
            public static int Min_VrelDepart;
            public static int Min_RCS;
            public static int Min_Lifetime;
            public static int Min_Size;
            public static int Min_ProbExists;
            public static int Min_Y;
            public static int Min_X;
            public static int Min_VYLeftRight;
            public static int Min_VXOncome;
            public static int Min_VYRightLeft;
            public static int Min_VXDepart;
            public static int Max_NofObj;
            public static int Max_Distance;
            public static int Max_Azimuth;
            public static int Max_VrelOncome;
            public static int Max_VrelDepart;
            public static int Max_RCS;
            public static int Max_Lifetime;
            public static int Max_Size;
            public static int Max_ProbExists;
            public static int Max_Y;
            public static int Max_X;
            public static int Max_VYLeftRight;
            public static int Max_VXOncome;
            public static int Max_VYRightLeft;
            public static int Max_VXDepart;
        }

        public struct RadarCfg_t
        {
            public char MaxDistance_valid;           //1st Bit0
            public char SensorID_valid;              //1st Bit1 
            public char RadarPower_valid;            //1st Bit2
            public char OutputType_valid;            //1st Bit3
            public char SendQuality_valid;           //1st Bit4 
            public char SendExtInfo_valid;           //1st Bit5
            public char SortIndex_valid;             //1st Bit6
            public char StoreInNVM_valid;            //1st Bit7

            public UInt16 MaxDistance;               //2nd Bit8-15  长度10位
                                                     //3th Bit16-21
                                                     //3th Bit22-23 
                                                     //4th Bit24-31
            public char SensorID;                    //5th Bit32-34 
            public char OutputType;                  //5th Bit35-36 
            public char RadarPower;                  //5th Bit37-39 
            public char CtrlRelay_valid;             //6th Bit40
            public char CtrlRelay;                   //6th Bit41
            public char SendQuality;                 //6th Bit42
            public char SendExtInfo;                 //6th Bit43
            public char SortIndex;                   //6th Bit44-46
            public char StoreInNVM;                  //6th Bit47
            public char RCS_Threshold_valid;         //7th Bit48
            public char RCS_Threshold;               //7th Bit49-51
                                              //8th
        };   //20201120-TS
    }   
}
