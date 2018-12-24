namespace TSPsolver
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.ResultArea = new System.Windows.Forms.PictureBox();
            this.StartButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ResultArea)).BeginInit();
            this.SuspendLayout();
            // 
            // ResultArea
            // 
            this.ResultArea.Location = new System.Drawing.Point(2, 2);
            this.ResultArea.Name = "ResultArea";
            this.ResultArea.Size = new System.Drawing.Size(360, 360);
            this.ResultArea.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ResultArea.TabIndex = 0;
            this.ResultArea.TabStop = false;
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(390, 12);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(130, 40);
            this.StartButton.TabIndex = 2;
            this.StartButton.Text = "开始";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.Start);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.StartButton);
            this.Name = "Form1";
            this.Text = "TSP solver";
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form_Click);
            ((System.ComponentModel.ISupportInitialize)(this.ResultArea)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox ResultArea;
        private System.Windows.Forms.Button StartButton;
    }
}

