﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SS_OpenCV
{
    public partial class InputBox : Form
    {
        public InputBox()
        {
            InitializeComponent();
        }

        public InputBox(string _title)
        {
            InitializeComponent();

            this.Text = _title;

        }

        private void InputBox_Load(object sender, EventArgs e)
        {

        }

        private void ValueTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
        }
    }
}