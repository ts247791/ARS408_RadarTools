using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace Serialexpample
{
    public partial class Form1 : Form
    {
        //create instance of property page
        //property page is used to set values for stop bits and 
        //baud rate

        PropertyPage pp = new PropertyPage();
        

        //create an Serial Port object
        //SerialPort sp = new SerialPort();

        delegate void UpdateTextEventHandler(string text); //委托,关键所在
        UpdateTextEventHandler updateText;

        delegate void UpdateDepthEventHandler(string textDepth);
        UpdateDepthEventHandler updateDepth;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //PropertyPage pp = new PropertyPage();
            pp.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //sp.Open();
                //sp.BaudRate = int.Parse(pp.bRate);
                //sp.DataBits = int.Parse(pp.dBits);
                //sp.Parity = Parity.None;
                //sp.StopBits = StopBits.One;
                //sp.ReadTimeout = 1000;

                this.updateText = new UpdateTextEventHandler(this.UpdateTextBox); // 实例化委托对象
                this.updateDepth = new UpdateDepthEventHandler(this.UpdateDepthTextBox);
                this.serialPort1.Open();
                //this.serialPort1.BaudRate = int.Parse(pp.bRate);
                //this.serialPort1.DataBits = int.Parse(pp.dBits);
                //this.serialPort1.Parity = Parity.None;
                //this.serialPort1.StopBits = StopBits.One;

                //MessageBox.Show(sp.DsrHolding.ToString());
                //MessageBox.Show(sp.ReadTimeout.ToString());
                

                
                //this.richTextBox1.Text = sp.ReadLine() + "\n";
                //this.richTextBox1.Text = sp.NewLine;
                //int i = 1;
                //while (i < 10)
                //{
                //    string str = sp.ReadLine();
                //    this.richTextBox1.Text = str;
                //    i++;
                //}
                //do
                //{
                //    String line = sp.ReadLine();
                //    this.richTextBox1.Text = line + "\n";
                //}while(sp.DsrHolding == true);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                this.serialPort1.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //sp.Close();
            this.serialPort1.Close();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //sp.Close();
            this.serialPort1.Close();
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //string readString = this.serialPort1.ReadExisting();
            string readString = this.serialPort1.ReadLine();
            string depValueString = readString.Substring(7, 6);
            this.Invoke(updateText, new string[] { readString });
            this.Invoke(updateDepth, new string[] { depValueString });
        }

        private void UpdateTextBox(string text)
        {
            //this.richTextBox1.Text = text + "\n";
            //this.richTextBox1.Text = text;
            if(this.serialPort1.IsOpen)
                this.richTextBox1.Text = text;
            
        }

        private void UpdateDepthTextBox(string textDep)
        {
            int dep = int.Parse(textDep);
            this.richTextBox2.Text = dep.ToString();
            this.label3.Text = dep.ToString();
        }
    }
}