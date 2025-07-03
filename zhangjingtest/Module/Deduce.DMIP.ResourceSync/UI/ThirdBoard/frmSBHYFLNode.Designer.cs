namespace Deduce.DMIP.ResourceSync
{
    partial class frmSBHYFLNode
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
            this.btnSave = new System.Windows.Forms.Button();
            this.txtExplain = new System.Windows.Forms.RichTextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.labName = new System.Windows.Forms.Label();
            this.labDate1 = new System.Windows.Forms.Label();
            this.dtpAfficfe = new System.Windows.Forms.DateTimePicker();
            this.labdate2 = new System.Windows.Forms.Label();
            this.dtpEffect = new System.Windows.Forms.DateTimePicker();
            this.labMark = new System.Windows.Forms.Label();
            this.cbxFlag = new System.Windows.Forms.ComboBox();
            this.labFBJG = new System.Windows.Forms.Label();
            this.cbxFBJG = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(223, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtExplain
            // 
            this.txtExplain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtExplain.Location = new System.Drawing.Point(3, 17);
            this.txtExplain.Name = "txtExplain";
            this.txtExplain.Size = new System.Drawing.Size(579, 140);
            this.txtExplain.TabIndex = 8;
            this.txtExplain.Text = "";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(73, 5);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(158, 21);
            this.txtName.TabIndex = 6;
            // 
            // labName
            // 
            this.labName.AutoSize = true;
            this.labName.Location = new System.Drawing.Point(15, 9);
            this.labName.Name = "labName";
            this.labName.Size = new System.Drawing.Size(53, 12);
            this.labName.TabIndex = 5;
            this.labName.Text = "行业名称";
            // 
            // labDate1
            // 
            this.labDate1.AutoSize = true;
            this.labDate1.Location = new System.Drawing.Point(15, 38);
            this.labDate1.Name = "labDate1";
            this.labDate1.Size = new System.Drawing.Size(53, 12);
            this.labDate1.TabIndex = 10;
            this.labDate1.Text = "公告日期";
            // 
            // dtpAfficfe
            // 
            this.dtpAfficfe.Location = new System.Drawing.Point(73, 32);
            this.dtpAfficfe.Name = "dtpAfficfe";
            this.dtpAfficfe.Size = new System.Drawing.Size(158, 21);
            this.dtpAfficfe.TabIndex = 11;
            // 
            // labdate2
            // 
            this.labdate2.AutoSize = true;
            this.labdate2.Location = new System.Drawing.Point(318, 36);
            this.labdate2.Name = "labdate2";
            this.labdate2.Size = new System.Drawing.Size(53, 12);
            this.labdate2.TabIndex = 12;
            this.labdate2.Text = "生效日期";
            // 
            // dtpEffect
            // 
            this.dtpEffect.Location = new System.Drawing.Point(373, 32);
            this.dtpEffect.Name = "dtpEffect";
            this.dtpEffect.Size = new System.Drawing.Size(158, 21);
            this.dtpEffect.TabIndex = 13;
            // 
            // labMark
            // 
            this.labMark.AutoSize = true;
            this.labMark.Location = new System.Drawing.Point(15, 61);
            this.labMark.Name = "labMark";
            this.labMark.Size = new System.Drawing.Size(53, 12);
            this.labMark.TabIndex = 16;
            this.labMark.Text = "公开标志";
            // 
            // cbxFlag
            // 
            this.cbxFlag.FormattingEnabled = true;
            this.cbxFlag.Location = new System.Drawing.Point(73, 58);
            this.cbxFlag.Name = "cbxFlag";
            this.cbxFlag.Size = new System.Drawing.Size(158, 20);
            this.cbxFlag.TabIndex = 17;
            // 
            // labFBJG
            // 
            this.labFBJG.AutoSize = true;
            this.labFBJG.Location = new System.Drawing.Point(294, 61);
            this.labFBJG.Name = "labFBJG";
            this.labFBJG.Size = new System.Drawing.Size(77, 12);
            this.labFBJG.TabIndex = 18;
            this.labFBJG.Text = "发布机构代码";
            // 
            // cbxFBJG
            // 
            this.cbxFBJG.FormattingEnabled = true;
            this.cbxFBJG.Location = new System.Drawing.Point(373, 58);
            this.cbxFBJG.Name = "cbxFBJG";
            this.cbxFBJG.Size = new System.Drawing.Size(158, 20);
            this.cbxFBJG.TabIndex = 19;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cbxFBJG);
            this.panel1.Controls.Add(this.labName);
            this.panel1.Controls.Add(this.labFBJG);
            this.panel1.Controls.Add(this.txtName);
            this.panel1.Controls.Add(this.cbxFlag);
            this.panel1.Controls.Add(this.labDate1);
            this.panel1.Controls.Add(this.labMark);
            this.panel1.Controls.Add(this.dtpAfficfe);
            this.panel1.Controls.Add(this.dtpEffect);
            this.panel1.Controls.Add(this.labdate2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(585, 86);
            this.panel1.TabIndex = 20;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 246);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(585, 35);
            this.panel2.TabIndex = 21;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtExplain);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 86);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(585, 160);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "注释说明";
            // 
            // frmSBHYFLNode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 281);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "frmSBHYFLNode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "三板行业分类信息";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.RichTextBox txtExplain;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label labName;
        private System.Windows.Forms.Label labDate1;
        private System.Windows.Forms.DateTimePicker dtpAfficfe;
        private System.Windows.Forms.Label labdate2;
        private System.Windows.Forms.DateTimePicker dtpEffect;
        private System.Windows.Forms.Label labMark;
        private System.Windows.Forms.ComboBox cbxFlag;
        private System.Windows.Forms.Label labFBJG;
        private System.Windows.Forms.ComboBox cbxFBJG;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}