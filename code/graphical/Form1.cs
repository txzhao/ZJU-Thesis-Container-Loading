/*===================== Form1.cs =====================*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BoxPacking;

namespace BoxPacking
{
    public partial class Form1 : Form
    {
        Container container = new Container();
        PackingInfomation information = new PackingInfomation();

        public Form1()
        {
            InitializeComponent();
            container.BackColor = Color.Gray;
            container.PackingInformation = information;
            container.Timer.Enabled = false;
            container.Timer.Interval = 10;
            container.Timer.Tick += new EventHandler(container.TimerTick);
            container.ProgressBar = this.progressBar1;
            container.Proportion = 1;
            DrawSize();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            container.OpenFile();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            container.SaveFile();
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void StartComputeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            container.StartCompute();
        }

        private void DisplayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            container.Display();
        }

        private void DrawSize()
        {
            int[] size = container.SetSize();
            this.Width = size[0] * container.Proportion + 100 + 20 + 16;
            container.Left = 10;
            this.Height = size[1] * container.Proportion + this.menuStrip1.Bottom + 20 + 40 + container.Proportion * 60;
            container.Top = this.menuStrip1.Bottom + 10;
            container.Parent = this;
            container.Bitmap = new Bitmap(container.Width, container.Height);
            Rectangle rect = new Rectangle(container.Right + 20, 200, 140, 130);
            this.Width += rect.Width + 40;
            information.Size = new Size(rect.Width, rect.Height);
            information.Left = rect.Location.X;
            information.Top = rect.Location.Y;
            information.Parent = this;
            progressBar1.Width = size[0] * container.Proportion * 2;
            progressBar1.Top = container.Bottom - progressBar1.Height;
            progressBar1.Left = container.Left + size[0] * container.Proportion + container.Proportion * 2;
            information.DrawRect(size);
            container.DrawFram(this.BackColor);
            container.DrawBoxInfo();
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            container.Stop();
        }

        private void ContinueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            container.Continue();
        }

        private void ResultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            container.Result();
        }

        private void 数据库连接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            container.Login();
        }

        private void 导出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            container.AddData();
        }

    }
}
