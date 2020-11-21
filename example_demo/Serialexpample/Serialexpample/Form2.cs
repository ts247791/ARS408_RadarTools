using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Serialexpample
{
    public partial class PropertyPage : Form
    {
        //variables for storing values of baud rate and stop bits
        private string baudR = "";
        private string stopB = "";
        private string dataB = "";
        private string parityV = "";
        

        //property for setting and getting baud rate and stop bits
        public string bRate
        {
            get
            {
                return baudR;
            }
            set
            {
                baudR = value;
            }
        }

        public string dBits
        {
            get
            {
                return dataB;
            }
            set
            {
                dataB = value;
            }
        }


        public string sBits
        {
            get
            {
                return stopB;
            }
            set
            {
                stopB = value;
            }
        }

        public string pValue
        {
            get
            {
                return parityV;
            }
            set
            {
                parityV = value;
            }
        }
        public PropertyPage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            this.bRate = comboBox2.Text;
            this.dBits = comboBox3.Text;
            this.pValue = comboBox4.Text;
            this.sBits = comboBox5.Text;

            this.Close();
        }
    }
}