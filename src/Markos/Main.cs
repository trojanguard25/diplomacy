﻿using System;
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
        BaseBot bot;

        public Main(string host, int port)
        {
            InitializeComponent();
            this.bot = new markos(host, port, this);
        }
    }
}
