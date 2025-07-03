using Deduce.DMIP.Business.Components;
namespace Deduce.DMIP.ResourceSync
{
    partial class frmLeakQueryDelete
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLeakQueryDelete));
            Deduce.DMIP.Business.Components.EditTrigger editTrigger1 = new Deduce.DMIP.Business.Components.EditTrigger();
            Deduce.DMIP.Business.Components.SelfProperty selfProperty1 = new Deduce.DMIP.Business.Components.SelfProperty();
            Deduce.DMIP.Business.Components.EditTrigger editTrigger2 = new Deduce.DMIP.Business.Components.EditTrigger();
            Deduce.DMIP.Business.Components.SelfProperty selfProperty2 = new Deduce.DMIP.Business.Components.SelfProperty();
            this.btnQuery = new System.Windows.Forms.Button();
            this.grpFilter = new System.Windows.Forms.GroupBox();
            this.chkdefault = new System.Windows.Forms.CheckBox();
            this.titleTxt = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dateEnd = new Deduce.DMIP.Business.Components.EditDateBox();
            this.dateBegin = new Deduce.DMIP.Business.Components.EditDateBox();
            this.gridCtrl = new DevExpress.XtraGrid.GridControl();
            this.cmsResource = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.itmDeleteRow = new System.Windows.Forms.ToolStripMenuItem();
            this.itmDeleteAll = new System.Windows.Forms.ToolStripMenuItem();
            this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcOB_OBJECT_ID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcGGRQ = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcTitle = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcExtension = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcResourceURL = new DevExpress.XtraGrid.Columns.GridColumn();
            this.LinkField = new DevExpress.XtraEditors.Repository.RepositoryItemHyperLinkEdit();
            this.gcINBBM = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcIGSDM = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcPYSEARCH = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcUniqueFields = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcRecordCount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcIsAutoVerify = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcIsWantToCheck = new DevExpress.XtraGrid.Columns.GridColumn();
            this.itmSync = new System.Windows.Forms.ToolStripMenuItem();
            this.grpFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridCtrl)).BeginInit();
            this.cmsResource.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LinkField)).BeginInit();
            this.SuspendLayout();
            // 
            // btnQuery
            // 
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnQuery.Location = new System.Drawing.Point(807, 22);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(61, 23);
            this.btnQuery.TabIndex = 25;
            this.btnQuery.Text = "查询";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // grpFilter
            // 
            this.grpFilter.Controls.Add(this.chkdefault);
            this.grpFilter.Controls.Add(this.titleTxt);
            this.grpFilter.Controls.Add(this.label1);
            this.grpFilter.Controls.Add(this.label3);
            this.grpFilter.Controls.Add(this.label2);
            this.grpFilter.Controls.Add(this.dateEnd);
            this.grpFilter.Controls.Add(this.dateBegin);
            this.grpFilter.Controls.Add(this.btnQuery);
            this.grpFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpFilter.ForeColor = System.Drawing.Color.Blue;
            this.grpFilter.Location = new System.Drawing.Point(0, 0);
            this.grpFilter.Name = "grpFilter";
            this.grpFilter.Size = new System.Drawing.Size(890, 79);
            this.grpFilter.TabIndex = 7;
            this.grpFilter.TabStop = false;
            this.grpFilter.Text = "查询条件";
            // 
            // chkdefault
            // 
            this.chkdefault.AutoSize = true;
            this.chkdefault.Location = new System.Drawing.Point(473, 24);
            this.chkdefault.Name = "chkdefault";
            this.chkdefault.Size = new System.Drawing.Size(96, 16);
            this.chkdefault.TabIndex = 29;
            this.chkdefault.Text = "是否指定查询";
            this.chkdefault.UseVisualStyleBackColor = true;
            // 
            // titleTxt
            // 
            this.titleTxt.Location = new System.Drawing.Point(70, 52);
            this.titleTxt.Name = "titleTxt";
            this.titleTxt.Size = new System.Drawing.Size(348, 21);
            this.titleTxt.TabIndex = 28;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 27;
            this.label1.Text = "标题";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 26;
            this.label3.Text = "公告日期";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(232, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 25;
            this.label2.Text = "——";
            // 
            // dateEnd
            // 
            this.dateEnd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.dateEnd.BoxDisplay = Deduce.DMIP.Sys.SysData.BoxDisplay.Both;
            this.dateEnd.DataSource = null;
            this.dateEnd.DataValue = "";
            this.dateEnd.DisplayField = null;
            this.dateEnd.DisplayMember = "";
            this.dateEnd.EditBackColor = System.Drawing.SystemColors.Info;
            this.dateEnd.EditReadOnly = false;
            this.dateEnd.EnterIsTab = true;
            this.dateEnd.FieldName = "";
            this.dateEnd.Formula = null;
            this.dateEnd.FrameMoving = null;
            this.dateEnd.FrameStay = null;
            this.dateEnd.ImageByte = null;
            this.dateEnd.IsBuildCode = true;
            this.dateEnd.IsEscClear = true;
            this.dateEnd.IsKeyDownF3 = false;
            this.dateEnd.IsQuery = false;
            this.dateEnd.IsQueryDataTable = false;
            this.dateEnd.IsSplitterFixed = false;
            this.dateEnd.LabelColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.dateEnd.LabelFont = new System.Drawing.Font("宋体", 9F);
            this.dateEnd.LabelText = "";
            this.dateEnd.Location = new System.Drawing.Point(258, 21);
            this.dateEnd.MenuID = "";
            this.dateEnd.MultiValue = "";
            this.dateEnd.Name = "dateEnd";
            this.dateEnd.PreviousData = null;
            this.dateEnd.PropertyData = ((Deduce.DMIP.Business.Components.ControlValue)(resources.GetObject("dateEnd.PropertyData")));
            this.dateEnd.ReadOnly = false;
            this.dateEnd.RelatedInputValue = null;
            this.dateEnd.RelationTable = null;
            this.dateEnd.Script = "";
            this.dateEnd.SearchBox = null;
            this.dateEnd.SearchBoxPlus = null;
            this.dateEnd.SelectedIndex = -1;
            this.dateEnd.SelectedItem = null;
            this.dateEnd.SelectedValue = null;
            this.dateEnd.Size = new System.Drawing.Size(160, 20);
            this.dateEnd.SwitchFieldName = null;
            this.dateEnd.TabIndex = 23;
            this.dateEnd.TableName = "";
            this.dateEnd.TableNo = null;
            this.dateEnd.TagWidth = 0;
            editTrigger1.IsTriggerRebind = false;
            editTrigger1.RebindScript = "";
            editTrigger1.TriggerSelfProperty = selfProperty1;
            this.dateEnd.TriggerInfo = editTrigger1;
            this.dateEnd.ValueMember = null;
            this.dateEnd.WidthScale = 0;
            // 
            // dateBegin
            // 
            this.dateBegin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.dateBegin.BoxDisplay = Deduce.DMIP.Sys.SysData.BoxDisplay.Both;
            this.dateBegin.DataSource = null;
            this.dateBegin.DataValue = "";
            this.dateBegin.DisplayField = null;
            this.dateBegin.DisplayMember = "";
            this.dateBegin.EditBackColor = System.Drawing.SystemColors.Info;
            this.dateBegin.EditReadOnly = false;
            this.dateBegin.EnterIsTab = true;
            this.dateBegin.FieldName = "";
            this.dateBegin.Formula = null;
            this.dateBegin.FrameMoving = null;
            this.dateBegin.FrameStay = null;
            this.dateBegin.ImageByte = null;
            this.dateBegin.IsBuildCode = true;
            this.dateBegin.IsEscClear = true;
            this.dateBegin.IsKeyDownF3 = false;
            this.dateBegin.IsQuery = false;
            this.dateBegin.IsQueryDataTable = false;
            this.dateBegin.IsSplitterFixed = false;
            this.dateBegin.LabelColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.dateBegin.LabelFont = new System.Drawing.Font("宋体", 9F);
            this.dateBegin.LabelText = "";
            this.dateBegin.Location = new System.Drawing.Point(65, 21);
            this.dateBegin.MenuID = "";
            this.dateBegin.MultiValue = "";
            this.dateBegin.Name = "dateBegin";
            this.dateBegin.PreviousData = null;
            this.dateBegin.PropertyData = ((Deduce.DMIP.Business.Components.ControlValue)(resources.GetObject("dateBegin.PropertyData")));
            this.dateBegin.ReadOnly = false;
            this.dateBegin.RelatedInputValue = null;
            this.dateBegin.RelationTable = null;
            this.dateBegin.Script = "";
            this.dateBegin.SearchBox = null;
            this.dateBegin.SearchBoxPlus = null;
            this.dateBegin.SelectedIndex = -1;
            this.dateBegin.SelectedItem = null;
            this.dateBegin.SelectedValue = null;
            this.dateBegin.Size = new System.Drawing.Size(160, 20);
            this.dateBegin.SwitchFieldName = null;
            this.dateBegin.TabIndex = 22;
            this.dateBegin.TableName = "";
            this.dateBegin.TableNo = null;
            this.dateBegin.TagWidth = 0;
            editTrigger2.IsTriggerRebind = false;
            editTrigger2.RebindScript = "";
            editTrigger2.TriggerSelfProperty = selfProperty2;
            this.dateBegin.TriggerInfo = editTrigger2;
            this.dateBegin.ValueMember = null;
            this.dateBegin.WidthScale = 0;
            // 
            // gridCtrl
            // 
            this.gridCtrl.ContextMenuStrip = this.cmsResource;
            this.gridCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridCtrl.Location = new System.Drawing.Point(0, 79);
            this.gridCtrl.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridCtrl.MainView = this.gridView;
            this.gridCtrl.Name = "gridCtrl";
            this.gridCtrl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.LinkField});
            this.gridCtrl.Size = new System.Drawing.Size(890, 318);
            this.gridCtrl.TabIndex = 26;
            this.gridCtrl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
            // 
            // cmsResource
            // 
            this.cmsResource.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmDeleteRow,
            this.itmDeleteAll,
            this.itmSync});
            this.cmsResource.Name = "cmsSimilar";
            this.cmsResource.Size = new System.Drawing.Size(161, 92);
            // 
            // itmDeleteRow
            // 
            this.itmDeleteRow.Name = "itmDeleteRow";
            this.itmDeleteRow.Size = new System.Drawing.Size(160, 22);
            this.itmDeleteRow.Text = "删除选中行";
            this.itmDeleteRow.Click += new System.EventHandler(this.itmDeleteRow_Click);
            // 
            // itmDeleteAll
            // 
            this.itmDeleteAll.Name = "itmDeleteAll";
            this.itmDeleteAll.Size = new System.Drawing.Size(160, 22);
            this.itmDeleteAll.Text = "删除全部";
            this.itmDeleteAll.Visible = false;
            this.itmDeleteAll.Click += new System.EventHandler(this.itmDeleteAll_Click);
            // 
            // gridView
            // 
            this.gridView.AllowCopyData = true;
            this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcOB_OBJECT_ID,
            this.gcCreateTime,
            this.gcCode,
            this.gcGGRQ,
            this.gcTitle,
            this.gcExtension,
            this.gcResourceURL,
            this.gcINBBM,
            this.gcIGSDM});
            this.gridView.GridControl = this.gridCtrl;
            this.gridView.IndicatorWidth = 45;
            this.gridView.MultiSelect = true;
            this.gridView.Name = "gridView";
            this.gridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.Click;
            this.gridView.OptionsCustomization.AllowGroup = false;
            this.gridView.OptionsCustomization.AllowQuickHideColumns = false;
            this.gridView.OptionsSelection.MultiSelect = true;
            this.gridView.OptionsView.GroupDrawMode = DevExpress.XtraGrid.Views.Grid.GroupDrawMode.Standard;
            this.gridView.OptionsView.ShowFooter = true;
            this.gridView.OptionsView.ShowGroupPanel = false;
            this.gridView.ReadOnly = false;
            this.gridView.ShowCellToolTip = true;
            this.gridView.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView_CustomDrawRowIndicator);
            // 
            // gcOB_OBJECT_ID
            // 
            this.gcOB_OBJECT_ID.Caption = "OB_OBJECT_ID";
            this.gcOB_OBJECT_ID.FieldName = "OB_OBJECT_ID";
            this.gcOB_OBJECT_ID.ImageKey = "";
            this.gcOB_OBJECT_ID.IsAlwaysSort = false;
            this.gcOB_OBJECT_ID.Name = "gcOB_OBJECT_ID";
            // 
            // gcCreateTime
            // 
            this.gcCreateTime.Caption = "创建时间";
            this.gcCreateTime.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
            this.gcCreateTime.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gcCreateTime.FieldName = "CreateTime";
            this.gcCreateTime.ImageKey = "";
            this.gcCreateTime.IsAlwaysSort = false;
            this.gcCreateTime.Name = "gcCreateTime";
            this.gcCreateTime.OptionsColumn.AllowEdit = false;
            this.gcCreateTime.SummaryItem.DisplayFormat = "{0}";
            this.gcCreateTime.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Count;
            this.gcCreateTime.Visible = true;
            this.gcCreateTime.VisibleIndex = 0;
            this.gcCreateTime.Width = 104;
            // 
            // gcCode
            // 
            this.gcCode.Caption = "编码";
            this.gcCode.FieldName = "Code";
            this.gcCode.ImageKey = "";
            this.gcCode.IsAlwaysSort = false;
            this.gcCode.Name = "gcCode";
            this.gcCode.Visible = true;
            this.gcCode.VisibleIndex = 1;
            this.gcCode.Width = 87;
            // 
            // gcGGRQ
            // 
            this.gcGGRQ.Caption = "公告日期";
            this.gcGGRQ.FieldName = "GGRQ";
            this.gcGGRQ.ImageKey = "";
            this.gcGGRQ.IsAlwaysSort = false;
            this.gcGGRQ.Name = "gcGGRQ";
            this.gcGGRQ.OptionsColumn.AllowEdit = false;
            this.gcGGRQ.Visible = true;
            this.gcGGRQ.VisibleIndex = 2;
            this.gcGGRQ.Width = 109;
            // 
            // gcTitle
            // 
            this.gcTitle.Caption = "标题";
            this.gcTitle.FieldName = "Title";
            this.gcTitle.ImageKey = "";
            this.gcTitle.IsAlwaysSort = false;
            this.gcTitle.Name = "gcTitle";
            this.gcTitle.OptionsColumn.AllowEdit = false;
            this.gcTitle.Visible = true;
            this.gcTitle.VisibleIndex = 3;
            this.gcTitle.Width = 254;
            // 
            // gcExtension
            // 
            this.gcExtension.Caption = "文件类型";
            this.gcExtension.FieldName = "Extension";
            this.gcExtension.ImageKey = "";
            this.gcExtension.IsAlwaysSort = false;
            this.gcExtension.Name = "gcExtension";
            this.gcExtension.OptionsColumn.AllowEdit = false;
            this.gcExtension.Visible = true;
            this.gcExtension.VisibleIndex = 4;
            this.gcExtension.Width = 84;
            // 
            // gcResourceURL
            // 
            this.gcResourceURL.Caption = "公告地址";
            this.gcResourceURL.ColumnEdit = this.LinkField;
            this.gcResourceURL.FieldName = "ResourceURL";
            this.gcResourceURL.ImageKey = "";
            this.gcResourceURL.IsAlwaysSort = false;
            this.gcResourceURL.Name = "gcResourceURL";
            this.gcResourceURL.Visible = true;
            this.gcResourceURL.VisibleIndex = 5;
            this.gcResourceURL.Width = 312;
            // 
            // LinkField
            // 
            this.LinkField.AutoHeight = false;
            this.LinkField.Name = "LinkField";
            // 
            // gcINBBM
            // 
            this.gcINBBM.Caption = "INBBM";
            this.gcINBBM.FieldName = "INBBM";
            this.gcINBBM.ImageKey = "";
            this.gcINBBM.IsAlwaysSort = false;
            this.gcINBBM.Name = "gcINBBM";
            // 
            // gcIGSDM
            // 
            this.gcIGSDM.Caption = "IGSDM";
            this.gcIGSDM.FieldName = "IGSDM";
            this.gcIGSDM.ImageKey = "";
            this.gcIGSDM.IsAlwaysSort = false;
            this.gcIGSDM.Name = "gcIGSDM";
            // 
            // gcPYSEARCH
            // 
            this.gcPYSEARCH.Caption = "拼音缩写";
            this.gcPYSEARCH.FieldName = "PYSEARCH";
            this.gcPYSEARCH.ImageKey = "";
            this.gcPYSEARCH.IsAlwaysSort = false;
            this.gcPYSEARCH.Name = "gcPYSEARCH";
            this.gcPYSEARCH.OptionsColumn.ReadOnly = true;
            this.gcPYSEARCH.Width = 120;
            // 
            // gcUniqueFields
            // 
            this.gcUniqueFields.Caption = "唯一性字段";
            this.gcUniqueFields.FieldName = "UniqueFields";
            this.gcUniqueFields.ImageKey = "";
            this.gcUniqueFields.IsAlwaysSort = false;
            this.gcUniqueFields.Name = "gcUniqueFields";
            this.gcUniqueFields.OptionsColumn.ReadOnly = true;
            this.gcUniqueFields.Visible = true;
            this.gcUniqueFields.VisibleIndex = 0;
            this.gcUniqueFields.Width = 150;
            // 
            // gcRecordCount
            // 
            this.gcRecordCount.Caption = "记录数限制（每次）";
            this.gcRecordCount.FieldName = "RecordCount";
            this.gcRecordCount.ImageKey = "";
            this.gcRecordCount.IsAlwaysSort = false;
            this.gcRecordCount.Name = "gcRecordCount";
            this.gcRecordCount.Visible = true;
            this.gcRecordCount.VisibleIndex = 0;
            this.gcRecordCount.Width = 150;
            // 
            // gcIsAutoVerify
            // 
            this.gcIsAutoVerify.Caption = "自动校对";
            this.gcIsAutoVerify.FieldName = "IsAutoVerify";
            this.gcIsAutoVerify.ImageKey = "";
            this.gcIsAutoVerify.IsAlwaysSort = false;
            this.gcIsAutoVerify.Name = "gcIsAutoVerify";
            this.gcIsAutoVerify.Visible = true;
            this.gcIsAutoVerify.VisibleIndex = 0;
            this.gcIsAutoVerify.Width = 120;
            // 
            // gcIsWantToCheck
            // 
            this.gcIsWantToCheck.Caption = "审核发布";
            this.gcIsWantToCheck.FieldName = "IsWantToCheck";
            this.gcIsWantToCheck.ImageKey = "";
            this.gcIsWantToCheck.IsAlwaysSort = false;
            this.gcIsWantToCheck.Name = "gcIsWantToCheck";
            this.gcIsWantToCheck.OptionsColumn.ReadOnly = true;
            this.gcIsWantToCheck.Width = 100;
            // 
            // itmSync
            // 
            this.itmSync.Name = "itmSync";
            this.itmSync.Size = new System.Drawing.Size(160, 22);
            this.itmSync.Text = "同步到三板公告";
            this.itmSync.Click += new System.EventHandler(this.itmSync_Click);
            // 
            // frmLeakQueryDelete
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 397);
            this.Controls.Add(this.gridCtrl);
            this.Controls.Add(this.grpFilter);
            this.Name = "frmLeakQueryDelete";
            this.ShowIcon = false;
            this.Text = "三板公告未入库公告查询";
            this.grpFilter.ResumeLayout(false);
            this.grpFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridCtrl)).EndInit();
            this.cmsResource.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LinkField)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.GroupBox grpFilter;
        private DevExpress.XtraGrid.GridControl gridCtrl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private Business.Components.EditDateBox dateBegin;
        private DevExpress.XtraGrid.Columns.GridColumn gcPYSEARCH;
        private DevExpress.XtraGrid.Columns.GridColumn gcUniqueFields;
        private DevExpress.XtraGrid.Columns.GridColumn gcRecordCount;
        private DevExpress.XtraGrid.Columns.GridColumn gcIsAutoVerify;
        private DevExpress.XtraGrid.Columns.GridColumn gcIsWantToCheck;
        private Business.Components.EditDateBox dateEnd;
        private DevExpress.XtraGrid.Columns.GridColumn gcOB_OBJECT_ID;
        private DevExpress.XtraGrid.Columns.GridColumn gcCreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn gcCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcGGRQ;
        private DevExpress.XtraGrid.Columns.GridColumn gcTitle;
        private DevExpress.XtraGrid.Columns.GridColumn gcExtension;
        private DevExpress.XtraGrid.Columns.GridColumn gcINBBM;
        private DevExpress.XtraGrid.Columns.GridColumn gcIGSDM;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private DevExpress.XtraGrid.Columns.GridColumn gcResourceURL;
        private DevExpress.XtraEditors.Repository.RepositoryItemHyperLinkEdit LinkField;
        private System.Windows.Forms.CheckBox chkdefault;
        private System.Windows.Forms.TextBox titleTxt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip cmsResource;
        private System.Windows.Forms.ToolStripMenuItem itmDeleteRow;
        private System.Windows.Forms.ToolStripMenuItem itmDeleteAll;
        private System.Windows.Forms.ToolStripMenuItem itmSync;
    }
}