using Deduce.Common.Formula;
using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;
using Deduce.DMIP.Business.DataAnalyze;
using Deduce.DMIP.Sys.SysData;
using System;
using System.Data;
using System.Windows.Forms;

namespace Deduce.DMIP.ResourceSync
{
    public partial class frmSBFLRuleDesign : DataForm
    {
        ResourceSyncRule _rule = new ResourceSyncRule();//资源同步规则设定存储体        
        DataTable _dtRule = null;
        string _objID = "";
        ArchivingSetting _arcSet = new ArchivingSetting();
        TableField _tableField = new TableField();
        bool _isModify = false;

        public frmSBFLRuleDesign()
        {
            InitializeComponent();
        }

        private void Reset()
        {
            _rule = new ResourceSyncRule();//资源同步规则设定存储体        
            _dtRule = null;
            _objID = "";
            _arcSet = new ArchivingSetting();
            _tableField = new TableField();
            txtFormula.Text = "";
        }

        public void DisplayWindow(string objID)
        {
            if (_objID == objID)
                return;

            CheckModifyStatus();
            Reset();
            _objID = objID;
            _tableField.FillType = FieldsFill.Query;
            string q = "select * from cfg.dmip_ArchiveRule where exType=3 and moduleID='" + objID + "'";
            _dtRule = _data.GetDataTable(q, base.MenuID);
            if (_dtRule == null)
            {
                _dtRule = _data.GetStructure("cfg.dmip_ArchiveRule ", base.MenuID);
            }
            if (Utils.IsEmpty(_dtRule))
                return;

            _rule = Utils.DesrializeFrom64String(new ResourceSyncRule(), _dtRule.Rows[0]["formula"].ToString());
            txtFormula.Text = _rule.CategoryExpress.Caption;
        }

        public void CheckModifyStatus()
        {
            if (!_isModify)
                return;

            DialogResult dr = MessageBox.Show("上一次编辑的公式未保存，需要保存吗？", "提示信息", MessageBoxButtons.YesNo);
            if (dr != DialogResult.Yes)
                return;

            SaveRule();
        }

        private void btnDesign_Click(object sender, EventArgs e)
        {
            try
            {
                var paras = BindParameter.GetParameter();
                Expression express = FormulaLoader.Design(this, paras, _rule.CategoryFormula);
                if (express == null)
                {
                    ResetFormula();
                }
                else
                {
                    txtFormula.Text = express.Caption;
                    _rule.CategoryExpress = express;
                }
                _isModify = FormulaLoader.IsOK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveRule();
        }

        private void SaveRule()
        {
            if (Utils.IsEmpty(txtFormula.Text))
            {
                MessageBox.Show("资源归类规则为空!!!");
                return;
            }

            DataFormula df = new DataFormula();
            df.Formula = Utils.SerializeTo64String(_rule);
            df.Caption = _rule.CategoryExpress.Caption;

            ArchivingSettingModel archivingSetting = new ArchivingSettingModel();
            archivingSetting.ModuleID = _objID;
            archivingSetting.ArchType = ArchivingType.ThirdBoard;

            _isModify = !_arcSet.SaveArchiving(df, _dtRule, archivingSetting);
        }

        private void ResetFormula()
        {
            txtFormula.Text = "";
        }

        private void btnClearRule_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes != MessageBox.Show("确定清除吗?", "清除", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                return;


            _arcSet.ClearArchiving(_objID, ArchivingType.ThirdBoard);
            ResetFormula();
        }
    }
}