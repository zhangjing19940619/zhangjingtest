namespace Deduce.DMIP.ResourceSync
{
    partial class frmSBFLRuleDesign
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
                components.Dispose ();
            }
            base.Dispose ( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSBFLRuleDesign));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtFormula = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDesign = new DevExpress.XtraEditors.SimpleButton();
            this.btnClearRule = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.groupBox3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtFormula);
            this.groupBox3.Controls.Add(this.panel1);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(513, 280);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "资源同步规则";
            // 
            // txtFormula
            // 
            this.txtFormula.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFormula.Location = new System.Drawing.Point(3, 45);
            this.txtFormula.Name = "txtFormula";
            this.txtFormula.ReadOnly = true;
            this.txtFormula.Size = new System.Drawing.Size(507, 232);
            this.txtFormula.TabIndex = 17;
            this.txtFormula.Text = "";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnDesign);
            this.panel1.Controls.Add(this.btnClearRule);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 17);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(507, 28);
            this.panel1.TabIndex = 18;
            // 
            // btnDesign
            // 
            this.btnDesign.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnDesign.Image = ((System.Drawing.Image)(resources.GetObject("btnDesign.Image")));
            this.btnDesign.Location = new System.Drawing.Point(4, 2);
            this.btnDesign.Name = "btnDesign";
            this.btnDesign.Size = new System.Drawing.Size(59, 21);
            this.btnDesign.TabIndex = 15;
            this.btnDesign.Text = "设 计";
            this.btnDesign.Click += new System.EventHandler(this.btnDesign_Click);
            // 
            // btnClearRule
            // 
            this.btnClearRule.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnClearRule.Image = ((System.Drawing.Image)(resources.GetObject("btnClearRule.Image")));
            this.btnClearRule.Location = new System.Drawing.Point(69, 2);
            this.btnClearRule.Name = "btnClearRule";
            this.btnClearRule.Size = new System.Drawing.Size(59, 21);
            this.btnClearRule.TabIndex = 15;
            this.btnClearRule.Text = "清 除";
            this.btnClearRule.Click += new System.EventHandler(this.btnClearRule_Click);
            // 
            // btnSave
            // 
            this.btnSave.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.Location = new System.Drawing.Point(134, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(59, 21);
            this.btnSave.TabIndex = 15;
            this.btnSave.Text = "保存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // frmSBFLRuleDesign
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(513, 280);
            this.Controls.Add(this.groupBox3);
            this.MaximizeBox = false;
            this.Name = "frmSBFLRuleDesign";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "三板公告分类 设计时";
            this.groupBox3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RichTextBox txtFormula;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.SimpleButton btnDesign;
        private DevExpress.XtraEditors.SimpleButton btnClearRule;
        private DevExpress.XtraEditors.SimpleButton btnSave;
    }
}