using Deduce.DMIP.Business.Components;
namespace Deduce.DMIP.ResourceSync
{
    partial class frmSBGGModify
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSBGGModify));
            Deduce.DMIP.Business.Components.EditTrigger editTrigger1 = new Deduce.DMIP.Business.Components.EditTrigger();
            Deduce.DMIP.Business.Components.SelfProperty selfProperty1 = new Deduce.DMIP.Business.Components.SelfProperty();
            this.opFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnFirst = new DevExpress.XtraEditors.SimpleButton();
            this.txtGGCount = new System.Windows.Forms.TextBox();
            this.btnLast = new DevExpress.XtraEditors.SimpleButton();
            this.btnNext = new DevExpress.XtraEditors.SimpleButton();
            this.btnPre = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnExchangeFile = new DevExpress.XtraEditors.SimpleButton();
            this.btnSeeFile = new DevExpress.XtraEditors.SimpleButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSelectedCode = new System.Windows.Forms.TextBox();
            this.txtGGRQ = new Deduce.DMIP.Business.Components.EditDateBox();
            this.btnGGLB = new System.Windows.Forms.Button();
            this.txtGGBT = new System.Windows.Forms.RichTextBox();
            this.txtGPDM = new Deduce.DMIP.Business.Components.ExtendTextBox(this.components);
            this.boxGKBZ = new System.Windows.Forms.ComboBox();
            this.txtGGLY = new Deduce.DMIP.Business.Components.ExtendTextBox(this.components);
            this.label11 = new System.Windows.Forms.Label();
            this.txtGGLB = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pnlExistFile = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnSeeExistFile = new DevExpress.XtraEditors.SimpleButton();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txtXGRY_OSS = new System.Windows.Forms.TextBox();
            this.txtGKBZ_OSS = new System.Windows.Forms.TextBox();
            this.txtGGRQ_OSS = new System.Windows.Forms.TextBox();
            this.txtGGBT_OSS = new System.Windows.Forms.RichTextBox();
            this.txtXGRQ_OSS = new System.Windows.Forms.TextBox();
            this.txtGGLB_OSS = new System.Windows.Forms.TextBox();
            this.txtZQDM_OSS = new System.Windows.Forms.TextBox();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.pnlExistFile.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // opFileDialog
            // 
            this.opFileDialog.FileName = "openFileDialog1";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel2.Controls.Add(this.btnFirst);
            this.panel2.Controls.Add(this.txtGGCount);
            this.panel2.Controls.Add(this.btnLast);
            this.panel2.Controls.Add(this.btnNext);
            this.panel2.Controls.Add(this.btnPre);
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Controls.Add(this.btnExchangeFile);
            this.panel2.Controls.Add(this.btnSeeFile);
            this.panel2.Location = new System.Drawing.Point(0, 234);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(383, 63);
            this.panel2.TabIndex = 51;
            // 
            // btnFirst
            // 
            this.btnFirst.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btnFirst.Image = ((System.Drawing.Image)(resources.GetObject("btnFirst.Image")));
            this.btnFirst.Location = new System.Drawing.Point(113, 41);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(27, 21);
            this.btnFirst.TabIndex = 19;
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            // 
            // txtGGCount
            // 
            this.txtGGCount.Location = new System.Drawing.Point(167, 40);
            this.txtGGCount.Name = "txtGGCount";
            this.txtGGCount.Size = new System.Drawing.Size(62, 21);
            this.txtGGCount.TabIndex = 18;
            this.txtGGCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtGGCount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGGCount_KeyDown);
            // 
            // btnLast
            // 
            this.btnLast.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnLast.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.btnLast.Image = ((System.Drawing.Image)(resources.GetObject("btnLast.Image")));
            this.btnLast.Location = new System.Drawing.Point(256, 41);
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(27, 21);
            this.btnLast.TabIndex = 12;
            this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
            // 
            // btnNext
            // 
            this.btnNext.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.btnNext.Image = ((System.Drawing.Image)(resources.GetObject("btnNext.Image")));
            this.btnNext.Location = new System.Drawing.Point(229, 41);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(27, 21);
            this.btnNext.TabIndex = 12;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPre
            // 
            this.btnPre.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btnPre.Image = ((System.Drawing.Image)(resources.GetObject("btnPre.Image")));
            this.btnPre.Location = new System.Drawing.Point(140, 41);
            this.btnPre.Name = "btnPre";
            this.btnPre.Size = new System.Drawing.Size(27, 21);
            this.btnPre.TabIndex = 11;
            this.btnPre.Click += new System.EventHandler(this.btnPre_Click);
            // 
            // btnSave
            // 
            this.btnSave.Appearance.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSave.Appearance.Options.UseFont = true;
            this.btnSave.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.Location = new System.Drawing.Point(289, 6);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(85, 29);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "保   存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExchangeFile
            // 
            this.btnExchangeFile.Appearance.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnExchangeFile.Appearance.Options.UseFont = true;
            this.btnExchangeFile.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
            this.btnExchangeFile.Image = ((System.Drawing.Image)(resources.GetObject("btnExchangeFile.Image")));
            this.btnExchangeFile.Location = new System.Drawing.Point(149, 6);
            this.btnExchangeFile.Name = "btnExchangeFile";
            this.btnExchangeFile.Size = new System.Drawing.Size(85, 29);
            this.btnExchangeFile.TabIndex = 9;
            this.btnExchangeFile.Text = "替换文件";
            this.btnExchangeFile.Click += new System.EventHandler(this.btnExchangeFile_Click);
            // 
            // btnSeeFile
            // 
            this.btnSeeFile.Appearance.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSeeFile.Appearance.Options.UseFont = true;
            this.btnSeeFile.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
            this.btnSeeFile.Image = ((System.Drawing.Image)(resources.GetObject("btnSeeFile.Image")));
            this.btnSeeFile.Location = new System.Drawing.Point(17, 6);
            this.btnSeeFile.Name = "btnSeeFile";
            this.btnSeeFile.Size = new System.Drawing.Size(85, 29);
            this.btnSeeFile.TabIndex = 8;
            this.btnSeeFile.Text = "查看文件";
            this.btnSeeFile.Click += new System.EventHandler(this.btnSeeFile_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(383, 218);
            this.panel1.TabIndex = 50;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Menu;
            this.groupBox1.Controls.Add(this.txtSelectedCode);
            this.groupBox1.Controls.Add(this.txtGGRQ);
            this.groupBox1.Controls.Add(this.btnGGLB);
            this.groupBox1.Controls.Add(this.txtGGBT);
            this.groupBox1.Controls.Add(this.txtGPDM);
            this.groupBox1.Controls.Add(this.boxGKBZ);
            this.groupBox1.Controls.Add(this.txtGGLY);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.txtGGLB);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(392, 234);
            this.groupBox1.TabIndex = 52;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "修改字段";
            // 
            // txtSelectedCode
            // 
            this.txtSelectedCode.BackColor = System.Drawing.SystemColors.Window;
            this.txtSelectedCode.Location = new System.Drawing.Point(266, 22);
            this.txtSelectedCode.Name = "txtSelectedCode";
            this.txtSelectedCode.ReadOnly = true;
            this.txtSelectedCode.Size = new System.Drawing.Size(117, 21);
            this.txtSelectedCode.TabIndex = 59;
            // 
            // txtGGRQ
            // 
            this.txtGGRQ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(237)))), ((int)(((byte)(215)))));
            this.txtGGRQ.BoxDisplay = Deduce.DMIP.Sys.SysData.BoxDisplay.Both;
            this.txtGGRQ.DataSource = null;
            this.txtGGRQ.DataValue = "";
            this.txtGGRQ.DisplayField = null;
            this.txtGGRQ.DisplayMember = "";
            this.txtGGRQ.EditBackColor = System.Drawing.SystemColors.Info;
            this.txtGGRQ.EditReadOnly = false;
            this.txtGGRQ.EnterIsTab = true;
            this.txtGGRQ.FieldName = "";
            this.txtGGRQ.Formula = null;
            this.txtGGRQ.FrameMoving = null;
            this.txtGGRQ.FrameStay = null;
            this.txtGGRQ.ImageByte = null;
            this.txtGGRQ.IsBuildCode = true;
            this.txtGGRQ.IsEscClear = true;
            this.txtGGRQ.IsKeyDownF3 = false;
            this.txtGGRQ.IsQuery = false;
            this.txtGGRQ.IsQueryDataTable = false;
            this.txtGGRQ.IsSplitterFixed = true;
            this.txtGGRQ.LabelColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.txtGGRQ.LabelFont = new System.Drawing.Font("宋体", 9F);
            this.txtGGRQ.LabelText = "";
            this.txtGGRQ.Location = new System.Drawing.Point(69, 51);
            this.txtGGRQ.MenuID = "";
            this.txtGGRQ.MultiValue = "";
            this.txtGGRQ.Name = "txtGGRQ";
            this.txtGGRQ.PreviousData = null;
            this.txtGGRQ.PropertyData = ((Deduce.DMIP.Business.Components.ControlValue)(resources.GetObject("txtGGRQ.PropertyData")));
            this.txtGGRQ.ReadOnly = true;
            this.txtGGRQ.RelatedInputValue = null;
            this.txtGGRQ.RelationTable = null;
            this.txtGGRQ.Script = "";
            this.txtGGRQ.SearchBox = null;
            this.txtGGRQ.SearchBoxPlus = null;
            this.txtGGRQ.SelectedIndex = -1;
            this.txtGGRQ.SelectedItem = null;
            this.txtGGRQ.SelectedValue = null;
            this.txtGGRQ.Size = new System.Drawing.Size(314, 20);
            this.txtGGRQ.SwitchFieldName = null;
            this.txtGGRQ.TabIndex = 2;
            this.txtGGRQ.TableName = "";
            this.txtGGRQ.TableNo = null;
            this.txtGGRQ.TagWidth = 0;
            editTrigger1.IsTriggerRebind = false;
            editTrigger1.RebindScript = "";
            editTrigger1.TriggerSelfProperty = selfProperty1;
            this.txtGGRQ.TriggerInfo = editTrigger1;
            this.txtGGRQ.ValueMember = null;
            this.txtGGRQ.WidthScale = 0;
            // 
            // btnGGLB
            // 
            this.btnGGLB.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGGLB.Location = new System.Drawing.Point(348, 157);
            this.btnGGLB.Name = "btnGGLB";
            this.btnGGLB.Size = new System.Drawing.Size(32, 23);
            this.btnGGLB.TabIndex = 6;
            this.btnGGLB.Text = "...";
            this.btnGGLB.UseVisualStyleBackColor = true;
            this.btnGGLB.Click += new System.EventHandler(this.btnGGLB_Click);
            this.btnGGLB.Leave += new System.EventHandler(this.btnGGLB_Leave);
            // 
            // txtGGBT
            // 
            this.txtGGBT.Location = new System.Drawing.Point(69, 77);
            this.txtGGBT.MaxLength = 400;
            this.txtGGBT.Name = "txtGGBT";
            this.txtGGBT.Size = new System.Drawing.Size(314, 45);
            this.txtGGBT.TabIndex = 3;
            this.txtGGBT.Text = "";
            this.txtGGBT.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGGBT_KeyDown);
            // 
            // txtGPDM
            // 
            this.txtGPDM.DataSource = null;
            this.txtGPDM.DefaultSelected = true;
            this.txtGPDM.DisplayMember = "";
            this.txtGPDM.DisplayValue = "";
            this.txtGPDM.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtGPDM.HideSystemMenu = false;
            this.txtGPDM.IsMulitValue = false;
            this.txtGPDM.IsUsedCache = false;
            this.txtGPDM.IsUsedCustomIME = false;
            this.txtGPDM.Location = new System.Drawing.Point(70, 22);
            this.txtGPDM.MaxLength = 100;
            this.txtGPDM.Name = "txtGPDM";
            this.txtGPDM.QueryValue = "";
            this.txtGPDM.Script = "";
            this.txtGPDM.ShortcutFind = true;
            this.txtGPDM.ShowDropBox = true;
            this.txtGPDM.Size = new System.Drawing.Size(190, 21);
            this.txtGPDM.TabIndex = 1;
            this.txtGPDM.ValueMember = "";
            this.txtGPDM.Watermark = "请输入证券代码或名称(拼音)";
            this.txtGPDM.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGPDM_KeyDown);
            // 
            // boxGKBZ
            // 
            this.boxGKBZ.FormattingEnabled = true;
            this.boxGKBZ.Items.AddRange(new object[] {
            "1 未公开",
            "3 公开"});
            this.boxGKBZ.Location = new System.Drawing.Point(70, 197);
            this.boxGKBZ.Name = "boxGKBZ";
            this.boxGKBZ.Size = new System.Drawing.Size(310, 20);
            this.boxGKBZ.TabIndex = 7;
            // 
            // txtGGLY
            // 
            this.txtGGLY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGGLY.DataSource = null;
            this.txtGGLY.DefaultSelected = true;
            this.txtGGLY.DisplayMember = "";
            this.txtGGLY.DisplayValue = "";
            this.txtGGLY.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtGGLY.HideSystemMenu = false;
            this.txtGGLY.IsMulitValue = false;
            this.txtGGLY.IsUsedCache = false;
            this.txtGGLY.IsUsedCustomIME = false;
            this.txtGGLY.Location = new System.Drawing.Point(71, 129);
            this.txtGGLY.MaxLength = 100;
            this.txtGGLY.Name = "txtGGLY";
            this.txtGGLY.QueryValue = "";
            this.txtGGLY.Script = "";
            this.txtGGLY.ShortcutFind = true;
            this.txtGGLY.ShowDropBox = true;
            this.txtGGLY.Size = new System.Drawing.Size(309, 21);
            this.txtGGLY.TabIndex = 4;
            this.txtGGLY.ValueMember = "";
            this.txtGGLY.Watermark = "请输入公告来源代码或名称";
            this.txtGGLY.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGGLY_KeyDown);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 133);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 0;
            this.label11.Text = "公告来源";
            // 
            // txtGGLB
            // 
            this.txtGGLB.Location = new System.Drawing.Point(70, 158);
            this.txtGGLB.Name = "txtGGLB";
            this.txtGGLB.Size = new System.Drawing.Size(280, 21);
            this.txtGGLB.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "证券代码";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "公告日期";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 199);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "公开标识";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "公告标题";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 161);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "公告类别";
            // 
            // pnlExistFile
            // 
            this.pnlExistFile.Controls.Add(this.groupBox2);
            this.pnlExistFile.Location = new System.Drawing.Point(389, 0);
            this.pnlExistFile.Name = "pnlExistFile";
            this.pnlExistFile.Size = new System.Drawing.Size(352, 297);
            this.pnlExistFile.TabIndex = 52;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panel4);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.txtXGRY_OSS);
            this.groupBox2.Controls.Add(this.txtGKBZ_OSS);
            this.groupBox2.Controls.Add(this.txtGGRQ_OSS);
            this.groupBox2.Controls.Add(this.txtGGBT_OSS);
            this.groupBox2.Controls.Add(this.txtXGRQ_OSS);
            this.groupBox2.Controls.Add(this.txtGGLB_OSS);
            this.groupBox2.Controls.Add(this.txtZQDM_OSS);
            this.groupBox2.Location = new System.Drawing.Point(2, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(350, 293);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "OSS端已存在的公告信息（只读）";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel4.Controls.Add(this.btnSeeExistFile);
            this.panel4.Location = new System.Drawing.Point(3, 261);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(347, 33);
            this.panel4.TabIndex = 17;
            // 
            // btnSeeExistFile
            // 
            this.btnSeeExistFile.Appearance.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSeeExistFile.Appearance.Options.UseFont = true;
            this.btnSeeExistFile.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
            this.btnSeeExistFile.Image = ((System.Drawing.Image)(resources.GetObject("btnSeeExistFile.Image")));
            this.btnSeeExistFile.Location = new System.Drawing.Point(117, 2);
            this.btnSeeExistFile.Name = "btnSeeExistFile";
            this.btnSeeExistFile.Size = new System.Drawing.Size(105, 29);
            this.btnSeeExistFile.TabIndex = 10;
            this.btnSeeExistFile.Text = "查看OSS文件";
            this.btnSeeExistFile.Click += new System.EventHandler(this.btnSeeExistFile_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 237);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 10;
            this.label7.Text = "修改人员";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 206);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "修改日期";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 173);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "公开标识";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 141);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 13;
            this.label9.Text = "公告类别";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 94);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 12);
            this.label10.TabIndex = 14;
            this.label10.Text = "公告标题";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 47);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 12);
            this.label12.TabIndex = 15;
            this.label12.Text = "公告日期";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(7, 19);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(53, 12);
            this.label13.TabIndex = 16;
            this.label13.Text = "证券代码";
            // 
            // txtXGRY_OSS
            // 
            this.txtXGRY_OSS.BackColor = System.Drawing.SystemColors.Window;
            this.txtXGRY_OSS.Location = new System.Drawing.Point(68, 234);
            this.txtXGRY_OSS.Name = "txtXGRY_OSS";
            this.txtXGRY_OSS.ReadOnly = true;
            this.txtXGRY_OSS.Size = new System.Drawing.Size(274, 21);
            this.txtXGRY_OSS.TabIndex = 3;
            // 
            // txtGKBZ_OSS
            // 
            this.txtGKBZ_OSS.BackColor = System.Drawing.SystemColors.Window;
            this.txtGKBZ_OSS.Location = new System.Drawing.Point(68, 171);
            this.txtGKBZ_OSS.Name = "txtGKBZ_OSS";
            this.txtGKBZ_OSS.ReadOnly = true;
            this.txtGKBZ_OSS.Size = new System.Drawing.Size(274, 21);
            this.txtGKBZ_OSS.TabIndex = 3;
            // 
            // txtGGRQ_OSS
            // 
            this.txtGGRQ_OSS.BackColor = System.Drawing.SystemColors.Window;
            this.txtGGRQ_OSS.Location = new System.Drawing.Point(68, 44);
            this.txtGGRQ_OSS.Name = "txtGGRQ_OSS";
            this.txtGGRQ_OSS.ReadOnly = true;
            this.txtGGRQ_OSS.Size = new System.Drawing.Size(274, 21);
            this.txtGGRQ_OSS.TabIndex = 3;
            // 
            // txtGGBT_OSS
            // 
            this.txtGGBT_OSS.BackColor = System.Drawing.SystemColors.Window;
            this.txtGGBT_OSS.Location = new System.Drawing.Point(67, 74);
            this.txtGGBT_OSS.Name = "txtGGBT_OSS";
            this.txtGGBT_OSS.ReadOnly = true;
            this.txtGGBT_OSS.Size = new System.Drawing.Size(275, 53);
            this.txtGGBT_OSS.TabIndex = 2;
            this.txtGGBT_OSS.Text = "";
            // 
            // txtXGRQ_OSS
            // 
            this.txtXGRQ_OSS.BackColor = System.Drawing.SystemColors.Window;
            this.txtXGRQ_OSS.Location = new System.Drawing.Point(68, 203);
            this.txtXGRQ_OSS.Name = "txtXGRQ_OSS";
            this.txtXGRQ_OSS.ReadOnly = true;
            this.txtXGRQ_OSS.Size = new System.Drawing.Size(274, 21);
            this.txtXGRQ_OSS.TabIndex = 1;
            // 
            // txtGGLB_OSS
            // 
            this.txtGGLB_OSS.BackColor = System.Drawing.SystemColors.Window;
            this.txtGGLB_OSS.Location = new System.Drawing.Point(68, 137);
            this.txtGGLB_OSS.Name = "txtGGLB_OSS";
            this.txtGGLB_OSS.ReadOnly = true;
            this.txtGGLB_OSS.Size = new System.Drawing.Size(274, 21);
            this.txtGGLB_OSS.TabIndex = 1;
            // 
            // txtZQDM_OSS
            // 
            this.txtZQDM_OSS.BackColor = System.Drawing.SystemColors.Window;
            this.txtZQDM_OSS.Location = new System.Drawing.Point(68, 16);
            this.txtZQDM_OSS.Name = "txtZQDM_OSS";
            this.txtZQDM_OSS.ReadOnly = true;
            this.txtZQDM_OSS.Size = new System.Drawing.Size(274, 21);
            this.txtZQDM_OSS.TabIndex = 1;
            // 
            // frmSBGGModify
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(740, 297);
            this.Controls.Add(this.pnlExistFile);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSBGGModify";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "三板公告修改";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSBGGModify_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmSBGGModify_KeyDown);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnlExistFile.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog opFileDialog;
        private System.Windows.Forms.Panel panel2;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnExchangeFile;
        private DevExpress.XtraEditors.SimpleButton btnSeeFile;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.SimpleButton btnPre;
        private DevExpress.XtraEditors.SimpleButton btnNext;
        private System.Windows.Forms.TextBox txtGGCount;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnGGLB;
        private System.Windows.Forms.RichTextBox txtGGBT;
        private Business.Components.ExtendTextBox txtGPDM;
        private System.Windows.Forms.ComboBox boxGKBZ;
        private Business.Components.ExtendTextBox txtGGLY;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtGGLB;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private Business.Components.EditDateBox txtGGRQ;
        private System.Windows.Forms.TextBox txtSelectedCode;
        private DevExpress.XtraEditors.SimpleButton btnLast;
        private DevExpress.XtraEditors.SimpleButton btnFirst;
        private System.Windows.Forms.Panel pnlExistFile;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel4;
        private DevExpress.XtraEditors.SimpleButton btnSeeExistFile;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtXGRY_OSS;
        private System.Windows.Forms.TextBox txtGKBZ_OSS;
        private System.Windows.Forms.TextBox txtGGRQ_OSS;
        private System.Windows.Forms.RichTextBox txtGGBT_OSS;
        private System.Windows.Forms.TextBox txtXGRQ_OSS;
        private System.Windows.Forms.TextBox txtGGLB_OSS;
        private System.Windows.Forms.TextBox txtZQDM_OSS;

    }
}