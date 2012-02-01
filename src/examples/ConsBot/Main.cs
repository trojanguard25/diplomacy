using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Polarsoft.Diplomacy.Daide;

namespace Polarsoft.Diplomacy.AI
{
    public partial class Main : Form
    {
        BaseBot bot;

        public Main(string host, int port)
        {
            InitializeComponent();
            this.bot = new ConsBot(host, port, this);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}