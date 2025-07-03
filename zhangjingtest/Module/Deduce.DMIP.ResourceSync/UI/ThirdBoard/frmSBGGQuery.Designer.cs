using Deduce.DMIP.Business.Components;
namespace Deduce.DMIP.ResourceSync
{
    partial class frmSBGGQuery
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSBGGQuery));
            Deduce.DMIP.Business.Components.EditTrigger editTrigger4 = new Deduce.DMIP.Business.Components.EditTrigger();
            Deduce.DMIP.Business.Components.SelfProperty selfProperty4 = new Deduce.DMIP.Business.Components.SelfProperty();
            Deduce.DMIP.Business.Components.EditTrigger editTrigger5 = new Deduce.DMIP.Business.Components.EditTrigger();
            Deduce.DMIP.Business.Components.SelfProperty selfProperty5 = new Deduce.DMIP.Business.Components.SelfProperty();
            Deduce.DMIP.Business.Components.EditTrigger editTrigger1 = new Deduce.DMIP.Business.Components.EditTrigger();
            Deduce.DMIP.Business.Components.SelfProperty selfProperty1 = new Deduce.DMIP.Business.Components.SelfProperty();
            Deduce.DMIP.Business.Components.EditTrigger editTrigger2 = new Deduce.DMIP.Business.Components.EditTrigger();
            Deduce.DMIP.Business.Components.SelfProperty selfProperty2 = new Deduce.DMIP.Business.Components.SelfProperty();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.gridSBGG = new System.Windows.Forms.DataGridView();
            this.pageQuery = new Deduce.DMIP.Business.Components.ucPageQuery();
            this.gpQueryCondition = new System.Windows.Forms.GroupBox();
            this.btnSeeSQL = new DevExpress.XtraEditors.SimpleButton();
            this.statusSelect = new System.Windows.Forms.CheckedListBox();
            this.btnClearRule = new DevExpress.XtraEditors.SimpleButton();
            this.txtXGLJ = new System.Windows.Forms.ComboBox();
            this.btnQuery = new DevExpress.XtraEditors.SimpleButton();
            this.txtXGRQEnd = new Deduce.DMIP.Business.Components.EditDateBox();
            this.txtGGRQEnd = new Deduce.DMIP.Business.Components.EditDateBox();
            this.txtXGRQStart = new Deduce.DMIP.Business.Components.EditDateBox();
            this.txtGGRQStart = new Deduce.DMIP.Business.Components.EditDateBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.boxUnpublic = new System.Windows.Forms.CheckBox();
            this.boxPublic = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtGPDM = new Deduce.DMIP.Business.Components.ExtendTextBox(this.components);
            this.txtGGLY = new Deduce.DMIP.Business.Components.ExtendTextBox(this.components);
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.gridSelectedXGLJ = new System.Windows.Forms.DataGridView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.gridSelectedGPDM = new System.Windows.Forms.DataGridView();
            this.label11 = new System.Windows.Forms.Label();
            this.btnGGLB = new System.Windows.Forms.Button();
            this.txtGGLB = new System.Windows.Forms.TextBox();
            this.txtXGRY = new Deduce.DMIP.Business.Components.ExtendTextBox(this.components);
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.gridSelectedXGRY = new System.Windows.Forms.DataGridView();
            this.groupBoxDatas = new System.Windows.Forms.GroupBox();
            this.gridSelectedGGLY = new System.Windows.Forms.DataGridView();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtGGBT = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridSBGG)).BeginInit();
            this.gpQueryCondition.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridSelectedXGLJ)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridSelectedGPDM)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridSelectedXGRY)).BeginInit();
            this.groupBoxDatas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridSelectedGGLY)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.gridSBGG);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 191);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1084, 84);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "公告列表";
            // 
            // gridSBGG
            // 
            this.gridSBGG.AllowUserToDeleteRows = false;
            this.gridSBGG.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.gridSBGG.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSBGG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridSBGG.Location = new System.Drawing.Point(3, 17);
            this.gridSBGG.Name = "gridSBGG";
            this.gridSBGG.ReadOnly = true;
            this.gridSBGG.RowTemplate.Height = 23;
            this.gridSBGG.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridSBGG.Size = new System.Drawing.Size(1078, 64);
            this.gridSBGG.TabIndex = 1;
            this.gridSBGG.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridSBGG_CellDoubleClick);
            this.gridSBGG.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.gridSBGG_RowPostPaint);
            this.gridSBGG.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridSBGG_KeyDown);
            // 
            // pageQuery
            // 
            this.pageQuery.CurrentPageIndex = "1";
            this.pageQuery.Location = new System.Drawing.Point(5, 163);
            this.pageQuery.MenuID = "";
            this.pageQuery.Name = "pageQuery";
            this.pageQuery.PerPageRecordCount = "25";
            this.pageQuery.Size = new System.Drawing.Size(790, 25);
            this.pageQuery.TabIndex = 1;
            this.pageQuery.TotalPages = "0";
            this.pageQuery.TotalRecords = "0";
            // 
            // gpQueryCondition
            // 
            this.gpQueryCondition.Controls.Add(this.pageQuery);
            this.gpQueryCondition.Controls.Add(this.btnSeeSQL);
            this.gpQueryCondition.Controls.Add(this.statusSelect);
            this.gpQueryCondition.Controls.Add(this.btnClearRule);
            this.gpQueryCondition.Controls.Add(this.txtXGLJ);
            this.gpQueryCondition.Controls.Add(this.btnQuery);
            this.gpQueryCondition.Controls.Add(this.txtXGRQEnd);
            this.gpQueryCondition.Controls.Add(this.txtGGRQEnd);
            this.gpQueryCondition.Controls.Add(this.txtXGRQStart);
            this.gpQueryCondition.Controls.Add(this.txtGGRQStart);
            this.gpQueryCondition.Controls.Add(this.panel1);
            this.gpQueryCondition.Controls.Add(this.label4);
            this.gpQueryCondition.Controls.Add(this.txtGPDM);
            this.gpQueryCondition.Controls.Add(this.txtGGLY);
            this.gpQueryCondition.Controls.Add(this.groupBox5);
            this.gpQueryCondition.Controls.Add(this.groupBox3);
            this.gpQueryCondition.Controls.Add(this.label11);
            this.gpQueryCondition.Controls.Add(this.btnGGLB);
            this.gpQueryCondition.Controls.Add(this.txtGGLB);
            this.gpQueryCondition.Controls.Add(this.txtXGRY);
            this.gpQueryCondition.Controls.Add(this.groupBox4);
            this.gpQueryCondition.Controls.Add(this.groupBoxDatas);
            this.gpQueryCondition.Controls.Add(this.label10);
            this.gpQueryCondition.Controls.Add(this.label9);
            this.gpQueryCondition.Controls.Add(this.label8);
            this.gpQueryCondition.Controls.Add(this.label6);
            this.gpQueryCondition.Controls.Add(this.label7);
            this.gpQueryCondition.Controls.Add(this.label5);
            this.gpQueryCondition.Controls.Add(this.txtGGBT);
            this.gpQueryCondition.Controls.Add(this.label3);
            this.gpQueryCondition.Controls.Add(this.label2);
            this.gpQueryCondition.Controls.Add(this.label1);
            this.gpQueryCondition.Dock = System.Windows.Forms.DockStyle.Top;
            this.gpQueryCondition.Location = new System.Drawing.Point(0, 0);
            this.gpQueryCondition.Name = "gpQueryCondition";
            this.gpQueryCondition.Padding = new System.Windows.Forms.Padding(2);
            this.gpQueryCondition.Size = new System.Drawing.Size(1084, 191);
            this.gpQueryCondition.TabIndex = 0;
            this.gpQueryCondition.TabStop = false;
            this.gpQueryCondition.Text = "查询条件";
            // 
            // btnSeeSQL
            // 
            this.btnSeeSQL.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btnSeeSQL.Image = ((System.Drawing.Image)(resources.GetObject("btnSeeSQL.Image")));
            this.btnSeeSQL.Location = new System.Drawing.Point(954, 163);
            this.btnSeeSQL.Name = "btnSeeSQL";
            this.btnSeeSQL.Size = new System.Drawing.Size(80, 21);
            this.btnSeeSQL.TabIndex = 16;
            this.btnSeeSQL.Text = " SQL脚本";
            this.btnSeeSQL.Click += new System.EventHandler(this.btnSeeSQL_Click);
            // 
            // statusSelect
            // 
            this.statusSelect.FormattingEnabled = true;
            this.statusSelect.Items.AddRange(new object[] {
            "0 初始状态",
            "1 同步中...",
            "2 同步成功",
            "3 代码为空",
            "4 Md5存在"});
            this.statusSelect.Location = new System.Drawing.Point(754, 86);
            this.statusSelect.Name = "statusSelect";
            this.statusSelect.Size = new System.Drawing.Size(128, 68);
            this.statusSelect.TabIndex = 105;
            this.statusSelect.TabStop = false;
            this.statusSelect.Tag = "";
            this.statusSelect.Leave += new System.EventHandler(this.statusSelect_Leave);
            // 
            // btnClearRule
            // 
            this.btnClearRule.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btnClearRule.Image = ((System.Drawing.Image)(resources.GetObject("btnClearRule.Image")));
            this.btnClearRule.Location = new System.Drawing.Point(799, 163);
            this.btnClearRule.Name = "btnClearRule";
            this.btnClearRule.Size = new System.Drawing.Size(57, 21);
            this.btnClearRule.TabIndex = 15;
            this.btnClearRule.Text = " 清除";
            this.btnClearRule.Click += new System.EventHandler(this.btnClearRule_Click);
            // 
            // txtXGLJ
            // 
            this.txtXGLJ.FormattingEnabled = true;
            this.txtXGLJ.Items.AddRange(new object[] {
            "2 手动录入"});
            this.txtXGLJ.Location = new System.Drawing.Point(608, 133);
            this.txtXGLJ.Name = "txtXGLJ";
            this.txtXGLJ.Size = new System.Drawing.Size(125, 20);
            this.txtXGLJ.TabIndex = 13;
            this.txtXGLJ.SelectedIndexChanged += new System.EventHandler(this.txtXGLJ_SelectedIndexChanged);
            // 
            // btnQuery
            // 
            this.btnQuery.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btnQuery.Image = ((System.Drawing.Image)(resources.GetObject("btnQuery.Image")));
            this.btnQuery.Location = new System.Drawing.Point(881, 163);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(57, 21);
            this.btnQuery.TabIndex = 14;
            this.btnQuery.Text = "查询";
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // txtXGRQEnd
            // 
            this.txtXGRQEnd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.txtXGRQEnd.BoxDisplay = Deduce.DMIP.Sys.SysData.BoxDisplay.Both;
            this.txtXGRQEnd.DataSource = null;
            this.txtXGRQEnd.DataValue = "";
            this.txtXGRQEnd.DisplayField = null;
            this.txtXGRQEnd.DisplayMember = "";
            this.txtXGRQEnd.EditBackColor = System.Drawing.SystemColors.Info;
            this.txtXGRQEnd.EditReadOnly = false;
            this.txtXGRQEnd.EnterIsTab = true;
            this.txtXGRQEnd.FieldName = "";
            this.txtXGRQEnd.Formula = null;
            this.txtXGRQEnd.FrameMoving = null;
            this.txtXGRQEnd.FrameStay = null;
            this.txtXGRQEnd.ImageByte = null;
            this.txtXGRQEnd.IsBuildCode = true;
            this.txtXGRQEnd.IsEscClear = true;
            this.txtXGRQEnd.IsKeyDownF3 = false;
            this.txtXGRQEnd.IsQuery = false;
            this.txtXGRQEnd.IsQueryDataTable = false;
            this.txtXGRQEnd.IsSplitterFixed = true;
            this.txtXGRQEnd.LabelColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.txtXGRQEnd.LabelFont = new System.Drawing.Font("宋体", 9F);
            this.txtXGRQEnd.LabelText = "";
            this.txtXGRQEnd.Location = new System.Drawing.Point(754, 47);
            this.txtXGRQEnd.MenuID = "";
            this.txtXGRQEnd.MultiValue = "";
            this.txtXGRQEnd.Name = "txtXGRQEnd";
            this.txtXGRQEnd.PreviousData = null;
            this.txtXGRQEnd.PropertyData = ((Deduce.DMIP.Business.Components.ControlValue)(resources.GetObject("txtXGRQEnd.PropertyData")));
            this.txtXGRQEnd.ReadOnly = false;
            this.txtXGRQEnd.RelatedInputValue = null;
            this.txtXGRQEnd.RelationTable = null;
            this.txtXGRQEnd.Script = "";
            this.txtXGRQEnd.SearchBox = null;
            this.txtXGRQEnd.SearchBoxPlus = null;
            this.txtXGRQEnd.SelectedIndex = -1;
            this.txtXGRQEnd.SelectedItem = null;
            this.txtXGRQEnd.SelectedValue = null;
            this.txtXGRQEnd.Size = new System.Drawing.Size(128, 20);
            this.txtXGRQEnd.SwitchFieldName = null;
            this.txtXGRQEnd.TabIndex = 10;
            this.txtXGRQEnd.TableName = "";
            this.txtXGRQEnd.TableNo = null;
            this.txtXGRQEnd.TagWidth = 0;
            editTrigger4.IsTriggerRebind = false;
            editTrigger4.RebindScript = "";
            editTrigger4.TriggerSelfProperty = selfProperty4;
            this.txtXGRQEnd.TriggerInfo = editTrigger4;
            this.txtXGRQEnd.ValueMember = null;
            this.txtXGRQEnd.WidthScale = 0;
            // 
            // txtGGRQEnd
            // 
            this.txtGGRQEnd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.txtGGRQEnd.BoxDisplay = Deduce.DMIP.Sys.SysData.BoxDisplay.Both;
            this.txtGGRQEnd.DataSource = null;
            this.txtGGRQEnd.DataValue = "";
            this.txtGGRQEnd.DisplayField = null;
            this.txtGGRQEnd.DisplayMember = "";
            this.txtGGRQEnd.EditBackColor = System.Drawing.SystemColors.Info;
            this.txtGGRQEnd.EditReadOnly = false;
            this.txtGGRQEnd.EnterIsTab = true;
            this.txtGGRQEnd.FieldName = "";
            this.txtGGRQEnd.Formula = null;
            this.txtGGRQEnd.FrameMoving = null;
            this.txtGGRQEnd.FrameStay = null;
            this.txtGGRQEnd.ImageByte = null;
            this.txtGGRQEnd.IsBuildCode = true;
            this.txtGGRQEnd.IsEscClear = true;
            this.txtGGRQEnd.IsKeyDownF3 = false;
            this.txtGGRQEnd.IsQuery = false;
            this.txtGGRQEnd.IsQueryDataTable = false;
            this.txtGGRQEnd.IsSplitterFixed = true;
            this.txtGGRQEnd.LabelColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.txtGGRQEnd.LabelFont = new System.Drawing.Font("宋体", 9F);
            this.txtGGRQEnd.LabelText = "";
            this.txtGGRQEnd.Location = new System.Drawing.Point(211, 12);
            this.txtGGRQEnd.MenuID = "";
            this.txtGGRQEnd.MultiValue = "";
            this.txtGGRQEnd.Name = "txtGGRQEnd";
            this.txtGGRQEnd.PreviousData = null;
            this.txtGGRQEnd.PropertyData = ((Deduce.DMIP.Business.Components.ControlValue)(resources.GetObject("txtGGRQEnd.PropertyData")));
            this.txtGGRQEnd.ReadOnly = false;
            this.txtGGRQEnd.RelatedInputValue = null;
            this.txtGGRQEnd.RelationTable = null;
            this.txtGGRQEnd.Script = "";
            this.txtGGRQEnd.SearchBox = null;
            this.txtGGRQEnd.SearchBoxPlus = null;
            this.txtGGRQEnd.SelectedIndex = -1;
            this.txtGGRQEnd.SelectedItem = null;
            this.txtGGRQEnd.SelectedValue = null;
            this.txtGGRQEnd.Size = new System.Drawing.Size(128, 20);
            this.txtGGRQEnd.SwitchFieldName = null;
            this.txtGGRQEnd.TabIndex = 2;
            this.txtGGRQEnd.TableName = "";
            this.txtGGRQEnd.TableNo = null;
            this.txtGGRQEnd.TagWidth = 0;
            editTrigger5.IsTriggerRebind = false;
            editTrigger5.RebindScript = "";
            editTrigger5.TriggerSelfProperty = selfProperty5;
            this.txtGGRQEnd.TriggerInfo = editTrigger5;
            this.txtGGRQEnd.ValueMember = null;
            this.txtGGRQEnd.WidthScale = 0;
            // 
            // txtXGRQStart
            // 
            this.txtXGRQStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.txtXGRQStart.BoxDisplay = Deduce.DMIP.Sys.SysData.BoxDisplay.Both;
            this.txtXGRQStart.DataSource = null;
            this.txtXGRQStart.DataValue = "";
            this.txtXGRQStart.DisplayField = null;
            this.txtXGRQStart.DisplayMember = "";
            this.txtXGRQStart.EditBackColor = System.Drawing.SystemColors.Info;
            this.txtXGRQStart.EditReadOnly = false;
            this.txtXGRQStart.EnterIsTab = true;
            this.txtXGRQStart.FieldName = "";
            this.txtXGRQStart.Formula = null;
            this.txtXGRQStart.FrameMoving = null;
            this.txtXGRQStart.FrameStay = null;
            this.txtXGRQStart.ImageByte = null;
            this.txtXGRQStart.IsBuildCode = true;
            this.txtXGRQStart.IsEscClear = true;
            this.txtXGRQStart.IsKeyDownF3 = false;
            this.txtXGRQStart.IsQuery = false;
            this.txtXGRQStart.IsQueryDataTable = false;
            this.txtXGRQStart.IsSplitterFixed = true;
            this.txtXGRQStart.LabelColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.txtXGRQStart.LabelFont = new System.Drawing.Font("宋体", 9F);
            this.txtXGRQStart.LabelText = "";
            this.txtXGRQStart.Location = new System.Drawing.Point(605, 47);
            this.txtXGRQStart.MenuID = "";
            this.txtXGRQStart.MultiValue = "";
            this.txtXGRQStart.Name = "txtXGRQStart";
            this.txtXGRQStart.PreviousData = null;
            this.txtXGRQStart.PropertyData = ((Deduce.DMIP.Business.Components.ControlValue)(resources.GetObject("txtXGRQStart.PropertyData")));
            this.txtXGRQStart.ReadOnly = false;
            this.txtXGRQStart.RelatedInputValue = null;
            this.txtXGRQStart.RelationTable = null;
            this.txtXGRQStart.Script = "";
            this.txtXGRQStart.SearchBox = null;
            this.txtXGRQStart.SearchBoxPlus = null;
            this.txtXGRQStart.SelectedIndex = -1;
            this.txtXGRQStart.SelectedItem = null;
            this.txtXGRQStart.SelectedValue = null;
            this.txtXGRQStart.Size = new System.Drawing.Size(128, 20);
            this.txtXGRQStart.SwitchFieldName = null;
            this.txtXGRQStart.TabIndex = 9;
            this.txtXGRQStart.TableName = "";
            this.txtXGRQStart.TableNo = null;
            this.txtXGRQStart.TagWidth = 0;
            editTrigger1.IsTriggerRebind = false;
            editTrigger1.RebindScript = "";
            editTrigger1.TriggerSelfProperty = selfProperty1;
            this.txtXGRQStart.TriggerInfo = editTrigger1;
            this.txtXGRQStart.ValueMember = null;
            this.txtXGRQStart.WidthScale = 0;
            // 
            // txtGGRQStart
            // 
            this.txtGGRQStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.txtGGRQStart.BoxDisplay = Deduce.DMIP.Sys.SysData.BoxDisplay.Both;
            this.txtGGRQStart.DataSource = null;
            this.txtGGRQStart.DataValue = "";
            this.txtGGRQStart.DisplayField = null;
            this.txtGGRQStart.DisplayMember = "";
            this.txtGGRQStart.EditBackColor = System.Drawing.SystemColors.Info;
            this.txtGGRQStart.EditReadOnly = false;
            this.txtGGRQStart.EnterIsTab = true;
            this.txtGGRQStart.FieldName = "";
            this.txtGGRQStart.Formula = null;
            this.txtGGRQStart.FrameMoving = null;
            this.txtGGRQStart.FrameStay = null;
            this.txtGGRQStart.ImageByte = null;
            this.txtGGRQStart.IsBuildCode = true;
            this.txtGGRQStart.IsEscClear = true;
            this.txtGGRQStart.IsKeyDownF3 = false;
            this.txtGGRQStart.IsQuery = false;
            this.txtGGRQStart.IsQueryDataTable = false;
            this.txtGGRQStart.IsSplitterFixed = true;
            this.txtGGRQStart.LabelColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.txtGGRQStart.LabelFont = new System.Drawing.Font("宋体", 9F);
            this.txtGGRQStart.LabelText = "";
            this.txtGGRQStart.Location = new System.Drawing.Point(62, 12);
            this.txtGGRQStart.MenuID = "";
            this.txtGGRQStart.MultiValue = "";
            this.txtGGRQStart.Name = "txtGGRQStart";
            this.txtGGRQStart.PreviousData = null;
            this.txtGGRQStart.PropertyData = ((Deduce.DMIP.Business.Components.ControlValue)(resources.GetObject("txtGGRQStart.PropertyData")));
            this.txtGGRQStart.ReadOnly = false;
            this.txtGGRQStart.RelatedInputValue = null;
            this.txtGGRQStart.RelationTable = null;
            this.txtGGRQStart.Script = "";
            this.txtGGRQStart.SearchBox = null;
            this.txtGGRQStart.SearchBoxPlus = null;
            this.txtGGRQStart.SelectedIndex = -1;
            this.txtGGRQStart.SelectedItem = null;
            this.txtGGRQStart.SelectedValue = null;
            this.txtGGRQStart.Size = new System.Drawing.Size(128, 20);
            this.txtGGRQStart.SwitchFieldName = null;
            this.txtGGRQStart.TabIndex = 1;
            this.txtGGRQStart.TableName = "";
            this.txtGGRQStart.TableNo = null;
            this.txtGGRQStart.TagWidth = 0;
            editTrigger2.IsTriggerRebind = false;
            editTrigger2.RebindScript = "";
            editTrigger2.TriggerSelfProperty = selfProperty2;
            this.txtGGRQStart.TriggerInfo = editTrigger2;
            this.txtGGRQStart.ValueMember = null;
            this.txtGGRQStart.WidthScale = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.boxUnpublic);
            this.panel1.Controls.Add(this.boxPublic);
            this.panel1.Location = new System.Drawing.Point(608, 85);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(125, 26);
            this.panel1.TabIndex = 104;
            // 
            // boxUnpublic
            // 
            this.boxUnpublic.AutoSize = true;
            this.boxUnpublic.Location = new System.Drawing.Point(61, 7);
            this.boxUnpublic.Name = "boxUnpublic";
            this.boxUnpublic.Size = new System.Drawing.Size(60, 16);
            this.boxUnpublic.TabIndex = 12;
            this.boxUnpublic.Text = "未公开";
            this.boxUnpublic.UseVisualStyleBackColor = true;
            // 
            // boxPublic
            // 
            this.boxPublic.AutoSize = true;
            this.boxPublic.Location = new System.Drawing.Point(7, 6);
            this.boxPublic.Name = "boxPublic";
            this.boxPublic.Size = new System.Drawing.Size(48, 16);
            this.boxPublic.TabIndex = 11;
            this.boxPublic.Text = "公开";
            this.boxPublic.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(548, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "公开标识";
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
            this.txtGPDM.Location = new System.Drawing.Point(62, 65);
            this.txtGPDM.MaxLength = 100;
            this.txtGPDM.Name = "txtGPDM";
            this.txtGPDM.QueryValue = "";
            this.txtGPDM.Script = "";
            this.txtGPDM.ShortcutFind = true;
            this.txtGPDM.ShowDropBox = true;
            this.txtGPDM.Size = new System.Drawing.Size(277, 21);
            this.txtGPDM.TabIndex = 4;
            this.txtGPDM.ValueMember = "";
            this.txtGPDM.Watermark = "请输入证券代码或名称（拼音）";
            this.txtGPDM.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGPDM_KeyDown);
            // 
            // txtGGLY
            // 
            this.txtGGLY.DataSource = null;
            this.txtGGLY.DefaultSelected = true;
            this.txtGGLY.DisplayMember = "";
            this.txtGGLY.DisplayValue = "";
            this.txtGGLY.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtGGLY.HideSystemMenu = false;
            this.txtGGLY.IsMulitValue = false;
            this.txtGGLY.IsUsedCache = false;
            this.txtGGLY.IsUsedCustomIME = false;
            this.txtGGLY.Location = new System.Drawing.Point(62, 38);
            this.txtGGLY.MaxLength = 100;
            this.txtGGLY.Name = "txtGGLY";
            this.txtGGLY.QueryValue = "";
            this.txtGGLY.Script = "";
            this.txtGGLY.ShortcutFind = true;
            this.txtGGLY.ShowDropBox = true;
            this.txtGGLY.Size = new System.Drawing.Size(277, 21);
            this.txtGGLY.TabIndex = 3;
            this.txtGGLY.ValueMember = "";
            this.txtGGLY.Watermark = "请输入公告来源代码或名称";
            this.txtGGLY.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGGLY_KeyDown);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.gridSelectedXGLJ);
            this.groupBox5.Location = new System.Drawing.Point(884, 82);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(1);
            this.groupBox5.Size = new System.Drawing.Size(196, 72);
            this.groupBox5.TabIndex = 49;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "已选修改路径";
            // 
            // gridSelectedXGLJ
            // 
            this.gridSelectedXGLJ.AllowUserToAddRows = false;
            this.gridSelectedXGLJ.AllowUserToDeleteRows = false;
            this.gridSelectedXGLJ.AllowUserToResizeColumns = false;
            this.gridSelectedXGLJ.AllowUserToResizeRows = false;
            this.gridSelectedXGLJ.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridSelectedXGLJ.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridSelectedXGLJ.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSelectedXGLJ.ColumnHeadersVisible = false;
            this.gridSelectedXGLJ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridSelectedXGLJ.GridColor = System.Drawing.SystemColors.ScrollBar;
            this.gridSelectedXGLJ.Location = new System.Drawing.Point(1, 15);
            this.gridSelectedXGLJ.Name = "gridSelectedXGLJ";
            this.gridSelectedXGLJ.ReadOnly = true;
            this.gridSelectedXGLJ.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gridSelectedXGLJ.RowHeadersVisible = false;
            this.gridSelectedXGLJ.RowTemplate.Height = 23;
            this.gridSelectedXGLJ.Size = new System.Drawing.Size(194, 56);
            this.gridSelectedXGLJ.TabIndex = 0;
            this.gridSelectedXGLJ.DoubleClick += new System.EventHandler(this.gridSelectedXGLJ_DoubleClick);
            this.gridSelectedXGLJ.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gridClearAllSelectedXGLJ);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.gridSelectedGPDM);
            this.groupBox3.Location = new System.Drawing.Point(342, 88);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(1);
            this.groupBox3.Size = new System.Drawing.Size(196, 71);
            this.groupBox3.TabIndex = 47;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "已选证券代码";
            // 
            // gridSelectedGPDM
            // 
            this.gridSelectedGPDM.AllowUserToAddRows = false;
            this.gridSelectedGPDM.AllowUserToDeleteRows = false;
            this.gridSelectedGPDM.AllowUserToResizeColumns = false;
            this.gridSelectedGPDM.AllowUserToResizeRows = false;
            this.gridSelectedGPDM.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridSelectedGPDM.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridSelectedGPDM.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSelectedGPDM.ColumnHeadersVisible = false;
            this.gridSelectedGPDM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridSelectedGPDM.GridColor = System.Drawing.SystemColors.ScrollBar;
            this.gridSelectedGPDM.Location = new System.Drawing.Point(1, 15);
            this.gridSelectedGPDM.Name = "gridSelectedGPDM";
            this.gridSelectedGPDM.ReadOnly = true;
            this.gridSelectedGPDM.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gridSelectedGPDM.RowHeadersVisible = false;
            this.gridSelectedGPDM.RowTemplate.Height = 23;
            this.gridSelectedGPDM.Size = new System.Drawing.Size(194, 55);
            this.gridSelectedGPDM.TabIndex = 0;
            this.gridSelectedGPDM.DoubleClick += new System.EventHandler(this.gridSelectedGPDM_DoubleClick);
            this.gridSelectedGPDM.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gridClearAllSelectedZQDM);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 40);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 0;
            this.label11.Text = "公告来源";
            // 
            // btnGGLB
            // 
            this.btnGGLB.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGGLB.Location = new System.Drawing.Point(298, 114);
            this.btnGGLB.Name = "btnGGLB";
            this.btnGGLB.Size = new System.Drawing.Size(38, 23);
            this.btnGGLB.TabIndex = 7;
            this.btnGGLB.Text = "...";
            this.btnGGLB.UseVisualStyleBackColor = true;
            this.btnGGLB.Click += new System.EventHandler(this.btnGGLB_Click);
            // 
            // txtGGLB
            // 
            this.txtGGLB.Location = new System.Drawing.Point(62, 115);
            this.txtGGLB.Name = "txtGGLB";
            this.txtGGLB.Size = new System.Drawing.Size(238, 21);
            this.txtGGLB.TabIndex = 6;
            this.txtGGLB.Leave += new System.EventHandler(this.txtGGLB_Leave);
            // 
            // txtXGRY
            // 
            this.txtXGRY.DataSource = null;
            this.txtXGRY.DefaultSelected = true;
            this.txtXGRY.DisplayMember = "";
            this.txtXGRY.DisplayValue = "";
            this.txtXGRY.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtXGRY.HideSystemMenu = false;
            this.txtXGRY.HintMsg = null;
            this.txtXGRY.IsMulitValue = false;
            this.txtXGRY.IsUsedCache = false;
            this.txtXGRY.IsUsedCustomIME = false;
            this.txtXGRY.Location = new System.Drawing.Point(608, 11);
            this.txtXGRY.MaxLength = 100;
            this.txtXGRY.Name = "txtXGRY";
            this.txtXGRY.QueryValue = "";
            this.txtXGRY.Script = "";
            this.txtXGRY.ShortcutFind = true;
            this.txtXGRY.ShowDropBox = true;
            this.txtXGRY.Size = new System.Drawing.Size(274, 21);
            this.txtXGRY.TabIndex = 8;
            this.txtXGRY.ValueMember = "";
            this.txtXGRY.Watermark = "请输入修改人员编码或姓名(拼音)";
            this.txtXGRY.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtXGRY_KeyDown);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.gridSelectedXGRY);
            this.groupBox4.Location = new System.Drawing.Point(885, -1);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(1);
            this.groupBox4.Size = new System.Drawing.Size(196, 82);
            this.groupBox4.TabIndex = 36;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "已选修改人员";
            // 
            // gridSelectedXGRY
            // 
            this.gridSelectedXGRY.AllowUserToAddRows = false;
            this.gridSelectedXGRY.AllowUserToDeleteRows = false;
            this.gridSelectedXGRY.AllowUserToResizeColumns = false;
            this.gridSelectedXGRY.AllowUserToResizeRows = false;
            this.gridSelectedXGRY.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridSelectedXGRY.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridSelectedXGRY.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSelectedXGRY.ColumnHeadersVisible = false;
            this.gridSelectedXGRY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridSelectedXGRY.GridColor = System.Drawing.SystemColors.ScrollBar;
            this.gridSelectedXGRY.Location = new System.Drawing.Point(1, 15);
            this.gridSelectedXGRY.Name = "gridSelectedXGRY";
            this.gridSelectedXGRY.ReadOnly = true;
            this.gridSelectedXGRY.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gridSelectedXGRY.RowHeadersVisible = false;
            this.gridSelectedXGRY.RowTemplate.Height = 23;
            this.gridSelectedXGRY.Size = new System.Drawing.Size(194, 66);
            this.gridSelectedXGRY.TabIndex = 0;
            this.gridSelectedXGRY.DoubleClick += new System.EventHandler(this.gridSelectedXGRY_DoubleClick);
            this.gridSelectedXGRY.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gridClearAllSelectedXGRY);
            // 
            // groupBoxDatas
            // 
            this.groupBoxDatas.Controls.Add(this.gridSelectedGGLY);
            this.groupBoxDatas.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBoxDatas.Location = new System.Drawing.Point(341, 4);
            this.groupBoxDatas.Name = "groupBoxDatas";
            this.groupBoxDatas.Padding = new System.Windows.Forms.Padding(1);
            this.groupBoxDatas.Size = new System.Drawing.Size(196, 82);
            this.groupBoxDatas.TabIndex = 36;
            this.groupBoxDatas.TabStop = false;
            this.groupBoxDatas.Text = "已选公告来源";
            // 
            // gridSelectedGGLY
            // 
            this.gridSelectedGGLY.AllowUserToAddRows = false;
            this.gridSelectedGGLY.AllowUserToDeleteRows = false;
            this.gridSelectedGGLY.AllowUserToResizeColumns = false;
            this.gridSelectedGGLY.AllowUserToResizeRows = false;
            this.gridSelectedGGLY.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridSelectedGGLY.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridSelectedGGLY.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSelectedGGLY.ColumnHeadersVisible = false;
            this.gridSelectedGGLY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridSelectedGGLY.GridColor = System.Drawing.SystemColors.ScrollBar;
            this.gridSelectedGGLY.Location = new System.Drawing.Point(1, 15);
            this.gridSelectedGGLY.Name = "gridSelectedGGLY";
            this.gridSelectedGGLY.ReadOnly = true;
            this.gridSelectedGGLY.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gridSelectedGGLY.RowHeadersVisible = false;
            this.gridSelectedGGLY.RowTemplate.Height = 23;
            this.gridSelectedGGLY.Size = new System.Drawing.Size(194, 66);
            this.gridSelectedGGLY.TabIndex = 0;
            this.gridSelectedGGLY.DoubleClick += new System.EventHandler(this.gridSelectedGGLY_DoubleClick);
            this.gridSelectedGGLY.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gridClearAllSelectedGGLY);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 118);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 12);
            this.label10.TabIndex = 0;
            this.label10.Text = "公告类别";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(548, 15);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "修改人员";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 69);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "证券代码";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(739, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "至";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(548, 52);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "修改日期";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(548, 137);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "修改路径";
            // 
            // txtGGBT
            // 
            this.txtGGBT.Location = new System.Drawing.Point(62, 90);
            this.txtGGBT.MaxLength = 400;
            this.txtGGBT.Name = "txtGGBT";
            this.txtGGBT.Size = new System.Drawing.Size(274, 21);
            this.txtGGBT.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "公告标题";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(196, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "至";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "公告日期";
            // 
            // frmSBGGQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 275);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.gpQueryCondition);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "frmSBGGQuery";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "三板公告查询";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmSBGGQuery_KeyDown);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridSBGG)).EndInit();
            this.gpQueryCondition.ResumeLayout(false);
            this.gpQueryCondition.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridSelectedXGLJ)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridSelectedGPDM)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridSelectedXGRY)).EndInit();
            this.groupBoxDatas.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridSelectedGGLY)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView gridSBGG;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtGGBT;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBoxDatas;
        private System.Windows.Forms.DataGridView gridSelectedGGLY;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.DataGridView gridSelectedXGRY;
        private Business.Components.ExtendTextBox txtXGRY;
        private System.Windows.Forms.TextBox txtGGLB;
        private System.Windows.Forms.Button btnGGLB;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridView gridSelectedGPDM;
        private DevExpress.XtraEditors.SimpleButton btnSeeSQL;
        private DevExpress.XtraEditors.SimpleButton btnClearRule;
        private DevExpress.XtraEditors.SimpleButton btnQuery;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.DataGridView gridSelectedXGLJ;
        private System.Windows.Forms.GroupBox gpQueryCondition;
        private Business.Components.ExtendTextBox txtGGLY;
        private Business.Components.ExtendTextBox txtGPDM;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox boxUnpublic;
        private System.Windows.Forms.CheckBox boxPublic;
        private System.Windows.Forms.Label label4;
        private Business.Components.EditDateBox txtGGRQStart;
        private Business.Components.EditDateBox txtGGRQEnd;
        private Business.Components.EditDateBox txtXGRQEnd;
        private Business.Components.EditDateBox txtXGRQStart;
        private System.Windows.Forms.ComboBox txtXGLJ;
        private System.Windows.Forms.CheckedListBox statusSelect;
        private Business.Components.ucPageQuery pageQuery;     

    }
}