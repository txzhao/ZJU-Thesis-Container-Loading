namespace BoxPacking
{
    partial class SizeInfo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Lable1 = new System.Windows.Forms.Label();
            this.Lable2 = new System.Windows.Forms.Label();
            this.Button = new System.Windows.Forms.Button();
            this.WidthOfContainer = new System.Windows.Forms.TextBox();
            this.HeightOfContainer = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Lable1
            // 
            this.Lable1.AutoSize = true;
            this.Lable1.Location = new System.Drawing.Point(31, 22);
            this.Lable1.Name = "Lable1";
            this.Lable1.Size = new System.Drawing.Size(41, 12);
            this.Lable1.TabIndex = 0;
            this.Lable1.Text = "Width:";
            // 
            // Lable2
            // 
            this.Lable2.AutoSize = true;
            this.Lable2.Location = new System.Drawing.Point(31, 59);
            this.Lable2.Name = "Lable2";
            this.Lable2.Size = new System.Drawing.Size(47, 12);
            this.Lable2.TabIndex = 1;
            this.Lable2.Text = "Height:";
            // 
            // Button
            // 
            this.Button.Location = new System.Drawing.Point(165, 54);
            this.Button.Name = "Button";
            this.Button.Size = new System.Drawing.Size(75, 23);
            this.Button.TabIndex = 2;
            this.Button.Text = "确定";
            this.Button.UseVisualStyleBackColor = true;
            this.Button.Click += new System.EventHandler(this.Button_Click);
            // 
            // WidthOfContainer
            // 
            this.WidthOfContainer.Location = new System.Drawing.Point(78, 19);
            this.WidthOfContainer.Name = "WidthOfContainer";
            this.WidthOfContainer.Size = new System.Drawing.Size(74, 21);
            this.WidthOfContainer.TabIndex = 3;
            // 
            // HeightOfContainer
            // 
            this.HeightOfContainer.Location = new System.Drawing.Point(79, 55);
            this.HeightOfContainer.Name = "HeightOfContainer";
            this.HeightOfContainer.Size = new System.Drawing.Size(73, 21);
            this.HeightOfContainer.TabIndex = 4;
            // 
            // SizeInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(252, 98);
            this.Controls.Add(this.HeightOfContainer);
            this.Controls.Add(this.WidthOfContainer);
            this.Controls.Add(this.Button);
            this.Controls.Add(this.Lable2);
            this.Controls.Add(this.Lable1);
            this.Name = "SizeInfo";
            this.Text = "SizeInfo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Lable1;
        private System.Windows.Forms.Label Lable2;
        private System.Windows.Forms.Button Button;
        private System.Windows.Forms.TextBox WidthOfContainer;
        private System.Windows.Forms.TextBox HeightOfContainer;
    }
}