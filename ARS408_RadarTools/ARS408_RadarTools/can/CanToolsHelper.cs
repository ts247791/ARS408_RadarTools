using ARS408_RadarTools;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ARS408_RadarTools.ARS408_State;

//1.ZLGCAN系列接口卡信息的数据类型。
public struct VCI_BOARD_INFO
{
    public UInt16 hw_Version;//硬件版本号
    public UInt16 fw_Version;//固件版本号
    public UInt16 dr_Version;//驱动程序版本号
    public UInt16 in_Version;//接口库版本号
    public UInt16 irq_Num;//保留参数
    public byte can_Num;//表示有几路can通道
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)] public byte[] str_Serial_Num;//此版卡的序列号
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)] public byte[] str_hw_Type;//硬件类型，比如“USBCAN V1.00”（注意：包括字符串结束符’\0’）
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]  public byte[] Reserved;//系统保留
}

/////////////////////////////////////////////////////
//2.定义CAN信息帧的数据类型。
unsafe public struct VCI_CAN_OBJ  //使用不安全代码
{
    public uint ID;
    public uint TimeStamp;
    public byte TimeFlag;
    public byte SendType;
    public byte RemoteFlag;//是否是远程帧
    public byte ExternFlag;//是否是扩展帧
    public byte DataLen;

    public fixed byte Data[8];

    public fixed byte Reserved[3];

}

//3.定义CAN控制器状态的数据类型。
public struct VCI_CAN_STATUS
{
    public byte ErrInterrupt;
    public byte regMode;
    public byte regStatus;
    public byte regALCapture;
    public byte regECCapture;
    public byte regEWLimit;
    public byte regRECounter;
    public byte regTECounter;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] Reserved;
}

//4.定义错误信息的数据类型。
public struct VCI_ERR_INFO
{
    public UInt32 ErrCode;
    public byte Passive_ErrData1;
    public byte Passive_ErrData2;
    public byte Passive_ErrData3;
    public byte ArLost_ErrData;
}

//5.定义初始化CAN的数据类型
public struct VCI_INIT_CONFIG
{
    public UInt32 AccCode;
    public UInt32 AccMask;
    public UInt32 Reserved;
    public byte Filter;
    public byte Timing0;
    public byte Timing1;
    public byte Mode;
}

public struct CHGDESIPANDPORT
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public byte[] szpwd;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
    public byte[] szdesip;
    public Int32 desport;

    public void Init()
    {
        szpwd = new byte[10];
        szdesip = new byte[20];
    }
}

namespace PedestrianSensingRadar.Utility
{
    /// <summary>
    /// CAN工具类
    /// </summary>
    public class CanToolsHelper : ICAN_TOOLS
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DeviceType"></param>
        /// <param name="DeviceInd"></param>
        /// <param name="Reserved"></param>
        /// <returns></returns>
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_OpenDevice(UInt32 DeviceType, UInt32 DeviceInd, UInt32 Reserved);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_CloseDevice(UInt32 DeviceType, UInt32 DeviceInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_InitCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_INIT_CONFIG pInitConfig);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadBoardInfo(UInt32 DeviceType, UInt32 DeviceInd, ref VCI_BOARD_INFO pInfo);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadErrInfo(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_ERR_INFO pErrInfo);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadCANStatus(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_STATUS pCANStatus);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_GetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_GetReceiveNum(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ClearBuffer(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_StartCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ResetCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_Transmit(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pSend, UInt32 Len);

        [DllImport("controlcan.dll", CharSet = CharSet.Ansi)]
        static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, IntPtr pReceive, UInt32 Len, Int32 WaitTime);
        /// <summary>
        /// 摘要：打开CAN口
        /// 
        /// 返回值：-1 CAN打开失败；-2 CAN初始化失败；-3 启动CAN失败
        /// </summary>
        /// <returns></returns>
        public int OpenCan()
        {
            if (VCI_OpenDevice(4, 0, 0) != 1)
            {
                return -1;
            }

            VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();
            config.AccCode = 0x00000000;//Std_11
            config.AccMask = 0xFFFFFFFF;//STD  
            config.Timing0 = 0x00;//500 Kbps
            config.Timing1 = 0x1C;//500 Kbps
            config.Filter = 1;
            config.Mode = 0;//Normal Model  

            if (VCI_InitCAN(4, 0, 0, ref config) != 1)
            {
                return -2;
            }

            if (VCI_StartCAN(4, 0, 0) != 1)
            {
                return -3;
            }
            return 0;
        }
        /// <summary>
        /// 摘要：关闭CAN口
        /// </summary>
        public void CloseCan()
        {
            VCI_CloseDevice(4, 0);
        }

        unsafe public void CanDataReceive()
        {
            UInt32 res = VCI_GetReceiveNum(4, 0, 0);
            if (res == 0)
                return;

            UInt32 con_maxlen = 2500;
            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)con_maxlen);
            res = VCI_Receive(4, 0, 0, pt, con_maxlen, 0);

            for (UInt32 i = 0; i < res; i++)
            {
                VCI_CAN_OBJ obj = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));
                if (obj.ID == 0x201)
                {
                    RadarStateParse(obj);                   
                }  
                if(obj.ID==0x203)
                {

                }
                if(obj.ID==0x204)
                {

                }
            }
        }

        unsafe public void SendCmdtoRadar(RadarCfg_t cfg)
        {
        
            VCI_CAN_OBJ obj = new VCI_CAN_OBJ();
            string tmpstr="";
            tmpstr = System.Convert.ToString(cfg.StoreInNVM_valid, 2).PadLeft(8, '0').Substring(7, 1) +
                System.Convert.ToString(cfg.SortIndex_valid, 2).PadLeft(8, '0').Substring(7, 1) +
                System.Convert.ToString(cfg.SendExtInfo_valid, 2).PadLeft(8, '0').Substring(7, 1) +
                System.Convert.ToString(cfg.SendQuality_valid, 2).PadLeft(8, '0').Substring(7, 1) +
                System.Convert.ToString(cfg.OutputType_valid, 2).PadLeft(8, '0').Substring(7, 1) +
                System.Convert.ToString(cfg.RadarPower_valid, 2).PadLeft(8, '0').Substring(7, 1) +
                System.Convert.ToString(cfg.SensorID_valid, 2).PadLeft(8, '0').Substring(7, 1) +
                System.Convert.ToString(cfg.MaxDistance_valid, 2).PadLeft(8, '0').Substring(7, 1) +

                System.Convert.ToString(cfg.MaxDistance, 2).PadLeft(16, '0').Substring(6, 8) +

                System.Convert.ToString(cfg.MaxDistance, 2).PadLeft(16, '0').Substring(14, 2).PadRight(8, '0') +

                "00000000" +

                System.Convert.ToString(cfg.RadarPower, 2).PadLeft(8, '0').Substring(5, 3) +
                System.Convert.ToString(cfg.OutputType, 2).PadLeft(8, '0').Substring(6, 2) +
                System.Convert.ToString(cfg.SensorID, 2).PadLeft(8, '0').Substring(5, 3) +

                System.Convert.ToString(cfg.StoreInNVM, 2).PadLeft(8, '0').Substring(7, 1) +
                System.Convert.ToString(cfg.SortIndex, 2).PadLeft(8, '0').Substring(5, 3) +
                System.Convert.ToString(cfg.SendExtInfo, 2).PadLeft(8, '0').Substring(7, 1) +
                System.Convert.ToString(cfg.SendQuality, 2).PadLeft(8, '0').Substring(7, 1) +
                System.Convert.ToString(cfg.CtrlRelay, 2).PadLeft(8, '0').Substring(7, 1) +
                System.Convert.ToString(cfg.CtrlRelay_valid, 2).PadLeft(8, '0').Substring(7, 1) +

                (System.Convert.ToString(cfg.RCS_Threshold, 2).PadLeft(8, '0').Substring(5, 3) +
                System.Convert.ToString(cfg.RCS_Threshold_valid, 2).PadLeft(8, '0').Substring(7, 1)).PadLeft(8,'0')+

                "00000000";



            //拼接下发指令
            obj.ID = 0x200;
            obj.TimeFlag = 0;
            obj.DataLen = 8;
            obj.Data[0]= (byte)GetCanMsg(tmpstr, 0, 8);
            obj.Data[1]= (byte)GetCanMsg(tmpstr, 8, 8);
            obj.Data[2] = (byte)GetCanMsg(tmpstr, 16, 8);
            obj.Data[3] = (byte)GetCanMsg(tmpstr, 24, 8);
            obj.Data[4] = (byte)GetCanMsg(tmpstr, 32, 8);
            obj.Data[5] = (byte)GetCanMsg(tmpstr, 40, 8);
            obj.Data[6] = (byte)GetCanMsg(tmpstr, 48, 8);
            obj.Data[7] = (byte)GetCanMsg(tmpstr, 56, 8);

            uint res = VCI_Transmit(4, 0, 0, ref obj, 1);
            if (res != 1)
            {
                MessageBox.Show("send target to CAN failed", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }





        }

        /// <summary>
        /// 二进制字符串转换成指定长度的二进制数
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="start_pos"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public UInt16 GetCanMsg(String msg, int start_pos, int length)
        {
            return System.Convert.ToUInt16(msg.Substring(start_pos, length), 2);
        }

        /// <summary>
        /// 0x201雷达状态数据解析
        /// </summary>
        /// <param name="stateout"></param>
        /// <param name="obj"></param>
        unsafe public void RadarStateParse(VCI_CAN_OBJ obj)
        {
            String obj_str = "";

            for (int i = 0; i < 8; i++)
            {
                obj_str += System.Convert.ToString(obj.Data[i], 2).PadLeft(8, '0');
            }

            Radar_State.NVMwriteStatus = GetCanMsg(obj_str, 0, 1);
            Radar_State.NVMReadStatus = GetCanMsg(obj_str, 1, 1);
            Radar_State.MaxDistanceCfg = GetCanMsg(obj_str, 8, 10);
            Radar_State.RadarPowerCfg = GetCanMsg(obj_str, 30, 3) ;
            Radar_State.SortIndex = GetCanMsg(obj_str, 33, 3);
            Radar_State.SensorID = GetCanMsg(obj_str, 37, 3);
            Radar_State.MotionRxState = GetCanMsg(obj_str, 40, 2);
            Radar_State.SendExtInfoCfg = GetCanMsg(obj_str, 42, 1);
            Radar_State.SendQualityCfg = GetCanMsg(obj_str, 43, 1);
            Radar_State.OutputTypeCfg = GetCanMsg(obj_str, 44, 2);
            Radar_State.CtrlRelayCfg = GetCanMsg(obj_str, 46, 1);
            Radar_State.RCS_Threshold = GetCanMsg(obj_str, 59, 3);
        }

        /// <summary>
        /// 0x203过滤器个数解析
        /// </summary>
        /// <param name="obj"></param>
        unsafe public void FilterCountParse(VCI_CAN_OBJ obj)
        {
            String obj_str = "";

            for (int i = 0; i < 8; i++)
            {
                obj_str += System.Convert.ToString(obj.Data[i], 2).PadLeft(8, '0');
            }        
        }

        /// <summary>
        /// 0x204过滤器状态解析
        /// </summary>
        /// <param name="obj"></param>
        unsafe public void FilterStateParse(VCI_CAN_OBJ obj)
        {
            String obj_str = "";

            for (int i = 0; i < 8; i++)
            {
                obj_str += System.Convert.ToString(obj.Data[i], 2).PadLeft(8, '0');
            }
        }

    }
}
