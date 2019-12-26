namespace Client
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.listView1 = new System.Windows.Forms.ListView();
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbXTEA = new System.Windows.Forms.RadioButton();
            this.rbKS = new System.Windows.Forms.RadioButton();
            this.rbDT = new System.Windows.Forms.RadioButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.cbOFB = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.listView1.Location = new System.Drawing.Point(12, 12);
            this.listView1.Name = "listView1";
            this.listView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.listView1.Size = new System.Drawing.Size(457, 326);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            // 
            // btnUpload
            // 
            this.btnUpload.Location = new System.Drawing.Point(479, 147);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(140, 41);
            this.btnUpload.TabIndex = 1;
            this.btnUpload.Text = "Upload";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(478, 194);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(141, 44);
            this.btnDownload.TabIndex = 2;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbXTEA);
            this.groupBox1.Controls.Add(this.rbKS);
            this.groupBox1.Controls.Add(this.rbDT);
            this.groupBox1.Location = new System.Drawing.Point(475, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(141, 100);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // rbXTEA
            // 
            this.rbXTEA.AutoSize = true;
            this.rbXTEA.Location = new System.Drawing.Point(7, 68);
            this.rbXTEA.Name = "rbXTEA";
            this.rbXTEA.Size = new System.Drawing.Size(53, 17);
            this.rbXTEA.TabIndex = 2;
            this.rbXTEA.Text = "XTEA";
            this.rbXTEA.UseVisualStyleBackColor = true;
            this.rbXTEA.CheckedChanged += new System.EventHandler(this.rbXTEA_CheckedChanged);
            // 
            // rbKS
            // 
            this.rbKS.AutoSize = true;
            this.rbKS.Location = new System.Drawing.Point(7, 44);
            this.rbKS.Name = "rbKS";
            this.rbKS.Size = new System.Drawing.Size(73, 17);
            this.rbKS.TabIndex = 1;
            this.rbKS.Text = "Knapsack";
            this.rbKS.UseVisualStyleBackColor = true;
            // 
            // rbDT
            // 
            this.rbDT.AutoSize = true;
            this.rbDT.Checked = true;
            this.rbDT.Location = new System.Drawing.Point(7, 20);
            this.rbDT.Name = "rbDT";
            this.rbDT.Size = new System.Drawing.Size(125, 17);
            this.rbDT.TabIndex = 0;
            this.rbDT.TabStop = true;
            this.rbDT.Text = "Double Transposition";
            this.rbDT.UseVisualStyleBackColor = true;
            // 
            // cbOFB
            // 
            this.cbOFB.AutoSize = true;
            this.cbOFB.Location = new System.Drawing.Point(482, 118);
            this.cbOFB.Name = "cbOFB";
            this.cbOFB.Size = new System.Drawing.Size(76, 17);
            this.cbOFB.TabIndex = 4;
            this.cbOFB.Text = "OFB mode";
            this.cbOFB.UseVisualStyleBackColor = true;
            this.cbOFB.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 349);
            this.Controls.Add(this.cbOFB);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.listView1);
            this.Name = "Form1";
            this.Text = "Cloud";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load_1);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbXTEA;
        private System.Windows.Forms.RadioButton rbKS;
        private System.Windows.Forms.RadioButton rbDT;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox cbOFB;
    }
}

