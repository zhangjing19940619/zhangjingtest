using Deduce.Common.Entity;
using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;
using Deduce.DMIP.ResourceSync.Server;
using DevExpress.XtraGrid.Views.Grid;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace Deduce.DMIP.ResourceSync
{
    public partial class frmLeakQueryDelete : DataForm
    {
        private readonly SBSyncHelper _sbSync;
        private readonly SBSyncToOssService _toOss;
        private readonly ILogger<frmLeakQueryDelete> _logger;
        private DataTable _dataSource;
        public frmLeakQueryDelete(SBSyncHelper sbSync, SBSyncToOssService toOss, ILogger<frmLeakQueryDelete> logger)
        {
            _sbSync = sbSync ?? throw new ArgumentNullException(nameof(sbSync));
            _toOss = toOss ?? throw new ArgumentNullException(nameof(toOss));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            InitializeComponent();
            Init();
        }

        public void DisplayWindow(string menuID)
        {
            dateBegin.Focus();
            this.MenuID = menuID;
            this.WindowState = FormWindowState.Maximized;
            this.Show();
        }

        private void Init()
        {
            dateBegin.Text = DateTime.Today.AddDays(-1).ToString(CultureInfo.InvariantCulture);
            dateEnd.Text = DateTime.Today.ToString(CultureInfo.InvariantCulture);
        }

        private void btnQuery_Click(object sender, System.EventArgs e)
        {
            btnQuery.Enabled = false;
            using (BackgroundWorker bw = new BackgroundWorker())
            {
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                bw.RunWorkerAsync("Test");
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Query();
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            gridCtrl.DataSource = _dataSource;
            btnQuery.Enabled = true;
        }

        private void Query()
        {
            string where = " ";
            if (!Utils.IsEmpty(dateBegin.Text))
                where += string.Format(" and a.GGRQ>='{0}'", Convert.ToDateTime(dateBegin.Text).ToString("yyyy-MM-dd 00:00:00"));
            if (!Utils.IsEmpty(dateEnd.Text))
                where += string.Format("and a.GGRQ<'{0}'", Convert.ToDateTime(dateEnd.Text).ToString("yyyy-MM-dd 23:59:59"));
            if (!Utils.IsEmpty(titleTxt.Text))
                where += string.Format("and a.Title like '%{0}%'", titleTxt.Text.Trim());
            string q = "";
            if (!chkdefault.Checked)
            {
                q = @" SELECT a.OB_OBJECT_ID,a.CreateTime,a.Code,a.GGRQ,a.Title,a.Extension,a.INBBM,a.IGSDM,
                        a.ResourceURL From cfg.dmip_Resource  a
                        Inner Join cfg.dmip_ResourceSync b On a.ob_object_id =b.ob_object_id  And a.TableName=b.TableName
                        Where b.status = 2 And b.TableName='usrSBGGB' And  b.GGID Is Null  " + where + " order by a.GGRQ desc";
            }
            else
            {
                q = @"  SELECT a.OB_OBJECT_ID,a.CreateTime,a.Code,a.GGRQ,a.Title,a.Extension,a.INBBM,a.IGSDM,
                        a.ResourceURL From cfg.dmip_Resource  a
                        Inner Join cfg.dmip_ResourceSync b On a.ob_object_id =b.ob_object_id  And a.TableName=b.TableName
                        Where b.TableName='usrSBGGB'   " + where + " order by a.GGRQ desc";
            }

            _dataSource = _data.GetDataTable(q, base.MenuID);
        }

        private void gridView_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            GridView gv = sender as GridView;
            if (!e.Info.IsRowIndicator)
                return;

            if (e.RowHandle >= 0)
            {
                e.Info.DisplayText = Convert.ToString(e.RowHandle + 1);
                DataRow dr = gv.GetDataRow(e.RowHandle);
                if (dr == null)
                    return;
                if (gv.DicDefaultRowStyle.ContainsKey(dr))
                    e.Info.Appearance.ForeColor = gv.DicDefaultRowStyle[dr].BackColor;
            }
            else if (e.RowHandle < 0 && e.RowHandle > -1000)
            {
                e.Info.Appearance.BackColor = System.Drawing.Color.AntiqueWhite;
                e.Info.DisplayText = "G" + e.RowHandle.ToString();
            }
        }

        private void itmDeleteRow_Click(object sender, EventArgs e)
        {
            DataRow dr = gridView.GetFocusedDataRow();
            if (dr == null) return;
            if (MessageBox.Show("是否确认删除？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.OK) 
            return;
          
            string ObjectIDs = "";
            DataTable currentData = _dataSource.Copy();
            for (int i = 0; i < gridView.SelectedRows.Count; i++)
            {
                DataRow dr1 = gridView.SelectedRows[i].DataRow;
                DataRow drcurrent = currentData.Rows[i];
                ObjectIDs += dr1["ob_object_id"].ToString() + ",";
                currentData.Rows.Remove(drcurrent);
            }
            ObjectIDs = ObjectIDs.Trim(',');
            ObjectIDs = ObjectIDs.Replace(",", "','");
            if (DeleteData(ObjectIDs))
            {
                _dataSource = currentData;
                gridCtrl.DataSource = _dataSource;
                MessageBox.Show("删除选中行成功!!!");
            }
            else
            {
                MessageBox.Show("删除选中行失败!!!");
            }
        }

        private void itmDeleteAll_Click(object sender, EventArgs e)
        {
            //暂时隐藏
            DataTable dt = _dataSource as DataTable;
            if (Utils.IsEmpty(dt)) return;
            string ObjectIDs = "";
            foreach (DataRow row in dt.Rows)
            {
                ObjectIDs += row["ob_object_id"].ToString() + ",";
            }
            ObjectIDs = ObjectIDs.Trim(',');
            ObjectIDs = ObjectIDs.Replace(",", "','");
            if (DeleteData(ObjectIDs))
            {
                _dataSource = null;
                gridCtrl.DataSource = null;
                MessageBox.Show("删除全部成功!!!");
            }
            else
            {
                MessageBox.Show("删除全部失败!!!");
            }

        }


        private bool DeleteData(string objectids)
        {
            if (Utils.IsEmpty(objectids)) 
                return false;

            string q = "select * from cfg.dmip_ResourceSync where ob_object_id in ('" + objectids + "')";
            DataTable dt = _data.GetDataTable(q, base.MenuID);
            if (Utils.IsEmpty(dt))
                return false;

            return _data.DataImport("cfg.dmip_ResourceSync", dt, ModifyType.Delete, base.MenuID);

        }
        
        private void itmSync_Click(object sender, EventArgs e)
        {
            DataRow dr = gridView.GetFocusedDataRow();
            if (dr == null) return;
            if (gridView.SelectedRows.Count > 1)
            {
                MessageBox.Show("手动同步一次只能同步一个");
                return;
            }
            if (MessageBox.Show("是否确认同步？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.OK)
                return;
          
           
            string ObjectIDs = "";
            DataTable currentData = _dataSource.Copy();
            for (int i = 0; i < gridView.SelectedRows.Count; i++)
            {
                DataRow dr1 = gridView.SelectedRows[i].DataRow;
                DataRow drcurrent = currentData.Rows[i];
                ObjectIDs += dr1["ob_object_id"].ToString() + ",";
                currentData.Rows.Remove(drcurrent);
            }
            ObjectIDs = ObjectIDs.Trim(',');
            ObjectIDs = ObjectIDs.Replace(",", "','");
            if (ToOSS(ObjectIDs))
            {
                MessageBox.Show("同步成功!!!");
            }
            else
            {
                MessageBox.Show("同步失败!!!");
            }


        }


        private bool ToOSS(string objectids)
        {
            if (Utils.IsEmpty(objectids)) return false;
            string q = string.Format(@"Select  b.ob_object_id,storePath,b.extension,b.opUser,c.sourceName,b.Summary,b.Keywords,
               b.webSiteURL,b.code,b.createtime,b.resourceURL,b.extension,b.ggrq,
               a.* From cfg.dmip_Resource(nolock) b
Inner Join cfg.dmip_GrabSite(nolock) c On b.sourceID = c.OB_OBJECT_ID
Inner Join  cfg.dmip_ResourceSync(nolock) a On b.ob_object_id = a.ob_object_id
Where a.TableName = 'usrSBGGB' And a.ob_object_id In('{0}')
  ORDER By b.createTime Desc",objectids);
            DataTable dt = _data.GetDataTable(q, base.MenuID);
            if (Utils.IsEmpty(dt))
                return false;
            try
            {               
                return _toOss.ToOSSExtend(dt);
               
            }
            catch
            {
                return false;
            }
        }
    }
}
