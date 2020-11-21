using PedestrianSensingRadar.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ARS408_RadarTools.ARS408_State;

namespace ARS408_RadarTools
{
    public partial class Frm_main : Form
    {
        public delegate void radarstateDisplaydelegate();
        public radarstateDisplaydelegate radarstatedisp_delegate;

        public delegate void filterstateDisplaydelegate();
        public filterstateDisplaydelegate filterstatedisp_delegate;

        UInt32 m_bOpen = 0;
        private CanToolsHelper ToolsHelper = new CanToolsHelper();
        private RadarCfg_t radarCfg;

        


        public Frm_main()
        {
            radarstatedisp_delegate = new radarstateDisplaydelegate(showRadarstate);            
            InitializeComponent();
        }

        private void btn_opncan_Click(object sender, EventArgs e)
        {
            if (m_bOpen == 1)
            {
                ToolsHelper.CloseCan();              
                m_bOpen = 0;
            }
            else
            {
                int ret = ToolsHelper.OpenCan();
                if (ret == -1)
                {

                    MessageBox.Show("Open USBCAN Failed!", "ERROR",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (ret == -2)
                {
                    MessageBox.Show("USBCAN Init Failed!!", "ERROR"
                        , MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (ret == -3)
                {
                    MessageBox.Show("USBCAN Start Failed!", "ERROR"
                        , MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                m_bOpen = 1;

            }
            btn_opencan.Text = m_bOpen == 1 ? "关闭CAN" : "打开CAN";
            timer_recive.Enabled = m_bOpen == 1 ? true : false;
        }
        /// <summary>
        /// 雷达参数配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_set_Click(object sender, EventArgs e)
        {           
            RadarParaInit();
            ToolsHelper.SendCmdtoRadar(radarCfg);

        }

        private void timer_recive_Tick(object sender, EventArgs e)
        {
            ToolsHelper.CanDataReceive();
            Invoke(radarstatedisp_delegate);           
        }

        private void showRadarstate()
        {
            lb_maxdistance.Text = (Radar_State.MaxDistanceCfg*2).ToString();
            lb_ID.Text = Radar_State.SensorID.ToString();
            switch (Radar_State.SortIndex)
            {
                case 0:
                    lb_sorted.Text = "未排序";
                    break;
                case 1:
                    lb_sorted.Text = "按照距离排序";
                    break;
                case 2:
                    lb_sorted.Text = "按照RCS排序";
                    break;
            }

            switch (Radar_State.RadarPowerCfg)
            {
                case 0:
                    lb_power.Text = "标准";
                    break;
                case 1:
                    lb_power.Text = "-3db";
                    break;
                case 2:
                    lb_power.Text = "-6db";
                    break;
                case 3:
                    lb_power.Text = "-9db";
                    break;
            }

            switch (Radar_State.OutputTypeCfg)
            {
                case 0:
                    lb_outputtype.Text = "空";
                    break;
                case 1:
                    lb_outputtype.Text = "目标";
                    break;
                case 2:
                    lb_outputtype.Text = "集群";
                    break;
            }

            lb_sendquality.Text = Radar_State.SendQualityCfg.ToString();
            lb_sendextend.Text = Radar_State.SendExtInfoCfg.ToString();

            switch (Radar_State.MotionRxState)
            {
                case 0:
                    lb_MotionRxState.Text = "输入正常";
                    break;
                case 1:
                    lb_MotionRxState.Text = "速度缺失";
                    break;
                case 2:
                    lb_MotionRxState.Text = "偏航角速度缺失";
                    break;
                case 3:
                    lb_MotionRxState.Text = "速度和偏航角速度缺失";
                    break;
            }

            switch (Radar_State.RCS_Threshold)
            {
                case 0:
                    lb_RCS_Threshold.Text = "标准";
                    break;
                case 1:
                    lb_RCS_Threshold.Text = "高灵敏度";
                    break;

            }
        }
        /// <summary>
        /// 初始化雷达配置界面控件值
        /// </summary>
        private void RadarParaInit()
        {
            radarCfg.MaxDistance_valid = (char)1;
            radarCfg.SensorID_valid = (char)1;
            radarCfg.RadarPower_valid = (char)1;
            radarCfg.OutputType_valid = (char)1;
            radarCfg.SendQuality_valid = (char)1;
            radarCfg.SendExtInfo_valid = (char)1;
            radarCfg.SortIndex_valid = (char)1;
            radarCfg.StoreInNVM_valid = (char)1;

           

            radarCfg.MaxDistance = (UInt16)(num_maxdistance.Value/2);//检测最大距离
            radarCfg.SensorID = (char)num_ID.Value;//ID

            //outtype
            if (rb_null.Checked == true)
                radarCfg.OutputType = (char)0;
            if (rb_obj.Checked == true)
                radarCfg.OutputType = (char)1;
            if (rb_cluster.Checked == true)
                radarCfg.OutputType = (char)2;


            //power
            if (rb_standard.Checked == true)
                radarCfg.RadarPower = (char)0;
            if (rb_3.Checked == true)
                radarCfg.RadarPower = (char)1;
            if (rb_6.Checked == true)
                radarCfg.RadarPower = (char)2;
            if (rb_9.Checked == true)
                radarCfg.RadarPower = (char)3;

            //relay 继电器开关
            if (cb_relay.Checked == true)
                radarCfg.CtrlRelay_valid = (char)1;
            else
                radarCfg.CtrlRelay_valid = (char)0;

            //继电器是否有效

            //质量信息
            if (cb_sendquality.Checked == true)
                radarCfg.SendQuality = (char)1;
            else
                radarCfg.SendQuality = (char)0;

            //扩展信息
            if (cb_sendextend.Checked == true)
                radarCfg.SendExtInfo = (char)1;
            else
                radarCfg.SendExtInfo = (char)0;

            //排序
            if (rb_unsorted.Checked == true)
                radarCfg.SortIndex = (char)0;
            if (rb_sortedbyrange.Checked == true)
                radarCfg.SortIndex = (char)1;
            if (rb_sortbyrcs.Checked == true)
                radarCfg.SortIndex = (char)2;

            //是否保存配置
            if (cb_pasave.Checked == true)
                radarCfg.StoreInNVM = (char)1;
            else
                radarCfg.StoreInNVM = (char)0;

            //传感器模式
            radarCfg.RCS_Threshold_valid = (char)1;
            if (rb_standardrcs.Checked == true)
                radarCfg.RCS_Threshold = (char)0;
            if (rb_highsens.Checked ==true)
                radarCfg.RCS_Threshold = (char)1;

        }

        private void btnfilter_maxcount_Click(object sender, EventArgs e)
        {

        }

        private void btn_radial_distance_Click(object sender, EventArgs e)
        {

        }

        private void btn_Azimuth_Click(object sender, EventArgs e)
        {

        }

        private void btn_VrelOncome_Click(object sender, EventArgs e)
        {

        }

        private void btn_VrelDepart_Click(object sender, EventArgs e)
        {

        }

        private void btn_RCS_Click(object sender, EventArgs e)
        {

        }

        private void btn_Lifetime_Click(object sender, EventArgs e)
        {

        }

        private void btn_Size_Click(object sender, EventArgs e)
        {

        }

        private void btn_Y_Click(object sender, EventArgs e)
        {

        }

        private void btn_X_Click(object sender, EventArgs e)
        {

        }

        private void btn_VYRightLeft_Click(object sender, EventArgs e)
        {

        }

        private void btn_VYLeftRight_Click(object sender, EventArgs e)
        {

        }

        private void btn_VXOncome_Click(object sender, EventArgs e)
        {

        }

        private void btn_VXDepart_Click(object sender, EventArgs e)
        {

        }

        private void btn_ProbExists_Click(object sender, EventArgs e)
        {

        }
    }
}
