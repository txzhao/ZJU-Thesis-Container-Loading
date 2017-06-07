namespace BoxPacking
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CloseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.运行ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StartComputeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DisplayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ContinueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ResultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.导出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.数据库连接ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.运行ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(326, 25);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenToolStripMenuItem,
            this.SaveToolStripMenuItem,
            this.数据库连接ToolStripMenuItem,
            this.导出ToolStripMenuItem,
            this.CloseToolStripMenuItem});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.OpenToolStripMenuItem.Text = "打开";
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // SaveToolStripMenuItem
            // 
            this.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            this.SaveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.SaveToolStripMenuItem.Text = "另存为";
            this.SaveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // CloseToolStripMenuItem
            // 
            this.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem";
            this.CloseToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.CloseToolStripMenuItem.Text = "关闭";
            this.CloseToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // 运行ToolStripMenuItem
            // 
            this.运行ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StartComputeToolStripMenuItem,
            this.DisplayToolStripMenuItem,
            this.StopToolStripMenuItem,
            this.ContinueToolStripMenuItem,
            this.ResultToolStripMenuItem});
            this.运行ToolStripMenuItem.Name = "运行ToolStripMenuItem";
            this.运行ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.运行ToolStripMenuItem.Text = "运行";
            // 
            // StartComputeToolStripMenuItem
            // 
            this.StartComputeToolStripMenuItem.Name = "StartComputeToolStripMenuItem";
            this.StartComputeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.StartComputeToolStripMenuItem.Text = "开始计算";
            this.StartComputeToolStripMenuItem.Click += new System.EventHandler(this.StartComputeToolStripMenuItem_Click);
            // 
            // DisplayToolStripMenuItem
            // 
            this.DisplayToolStripMenuItem.Name = "DisplayToolStripMenuItem";
            this.DisplayToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.DisplayToolStripMenuItem.Text = "动态显示";
            this.DisplayToolStripMenuItem.Click += new System.EventHandler(this.DisplayToolStripMenuItem_Click);
            // 
            // StopToolStripMenuItem
            // 
            this.StopToolStripMenuItem.Name = "StopToolStripMenuItem";
            this.StopToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.StopToolStripMenuItem.Text = "暂停";
            this.StopToolStripMenuItem.Click += new System.EventHandler(this.StopToolStripMenuItem_Click);
            // 
            // ContinueToolStripMenuItem
            // 
            this.ContinueToolStripMenuItem.Name = "ContinueToolStripMenuItem";
            this.ContinueToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ContinueToolStripMenuItem.Text = "继续";
            this.ContinueToolStripMenuItem.Click += new System.EventHandler(this.ContinueToolStripMenuItem_Click);
            // 
            // ResultToolStripMenuItem
            // 
            this.ResultToolStripMenuItem.Name = "ResultToolStripMenuItem";
            this.ResultToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ResultToolStripMenuItem.Text = "运行结果";
            this.ResultToolStripMenuItem.Click += new System.EventHandler(this.ResultToolStripMenuItem_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(172, 150);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(100, 23);
            this.progressBar1.TabIndex = 4;
            this.progressBar1.Visible = false;
            // 
            // 导出ToolStripMenuItem
            // 
            this.导出ToolStripMenuItem.Name = "导出ToolStripMenuItem";
            this.导出ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.导出ToolStripMenuItem.Text = "导出";
            this.导出ToolStripMenuItem.Click += new System.EventHandler(this.导出ToolStripMenuItem_Click);
            // 
            // 数据库连接ToolStripMenuItem
            // 
            this.数据库连接ToolStripMenuItem.Name = "数据库连接ToolStripMenuItem";
            this.数据库连接ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.数据库连接ToolStripMenuItem.Text = "数据库连接";
            this.数据库连接ToolStripMenuItem.Click += new System.EventHandler(this.数据库连接ToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(326, 377);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 运行ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CloseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem StartComputeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DisplayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem StopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ContinueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ResultToolStripMenuItem;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ToolStripMenuItem 数据库连接ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导出ToolStripMenuItem;
    }
}

