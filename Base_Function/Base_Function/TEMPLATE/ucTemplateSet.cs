using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Bifrost;

namespace Base_Function.TEMPLATE
{
    public partial class ucTemplateSet : UserControl
    {
        public ucTemplateSet()
        {
            InitializeComponent();
            App.UsControlStyle(this);
        }
    }
}