using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace BoxPacking
{
    public partial class SizeInfo : Form
    {
        private int x = 0;

        public int ContainerWidth
        {
            get
            {
                if (IsInteger(this.WidthOfContainer.Text) && this.WidthOfContainer.Text != "")
                {
                    if (Convert.ToInt32(this.WidthOfContainer.Text) > 0)
                    {
                        x = 1;
                        return Convert.ToInt32(this.WidthOfContainer.Text);
                    }
                    else
                    {
                        x = 0;
                        MessageBox.Show("Please input a valid number", "Error");
                        return -1;
                    }
                }
                else
                {
                    x = 0;
                    MessageBox.Show("Please input a valid number", "Error");
                    return -1;
                }
            }
        }

        public int ContainerHeight
        {
            get
            {
                if (IsInteger(this.HeightOfContainer.Text) && this.HeightOfContainer.Text != "")
                {
                    if (Convert.ToInt32(this.HeightOfContainer.Text) > 0)
                    {
                        return Convert.ToInt32(this.HeightOfContainer.Text);
                    }
                    else
                    {
                        if (x == 1)
                        {
                            MessageBox.Show("Please input a valid number", "Error");
                        }
                        return -1;
                    }
                }
                else
                {
                    if (x == 1)
                    {
                        MessageBox.Show("Please input a valid number", "Error");
                    }
                    return -1;
                }
            }
        }

        public SizeInfo()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public bool IsInteger(string value)
        {
            Regex r = new Regex(@"^\d*$");
            return r.IsMatch(value);
        }
 
    }
}
