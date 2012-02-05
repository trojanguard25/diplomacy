using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Polarsoft.Diplomacy.Daide;

namespace Polarsoft.Diplomacy.AI
{
    public partial class Main : Form
    {
        markos bot1;

        public Main(string host, int port)
        {
            InitializeComponent();
            this.button1.Enabled = false;
            this.bot1 = new markos(host, port, this);
        }

        public void UpdateGameTime(string str)
        {
            this.label3.Text = str;
        }

        public void UpdatePowersListBox(List<string> powers)
        {
            if (this.comboBox1.Items.Count > 0)
            {
                this.comboBox1.Items.Clear();
            }

            foreach (string power in powers)
            {
                this.comboBox1.Items.Add(power.ToString());
            }
        }

        public void SetReady()
        {
            this.button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
            this.bot1.SendOrders();
        }
    }
}
