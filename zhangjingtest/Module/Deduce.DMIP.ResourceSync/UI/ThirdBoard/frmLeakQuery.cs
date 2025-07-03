using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;
using DevExpress.XtraGrid.Views.Grid;

namespace Deduce.DMIP.ResourceSync
{
    public partial class frmLeakQuery : DataForm
    {
        private DataTable _dataSource;
        public frmLeakQuery()
        {
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
            string q = @"SELECT TableName,(Caption +' '+ TableName) Caption From cfg.Grouptable(nolock)";
            DataTable dt = _data.GetDataTable(q, base.MenuID);
            txtTableName.DataSource = dt;
            txtTableName.ValueMember = "TableName";
            txtTableName.DisplayMember = "Caption";
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
            string where = " Where 0=0";
            if (!Utils.IsEmpty(dateBegin.Text))
                where += " AND n.GGRQ>='" + Convert.ToDateTime(dateBegin.Text) + "'";
            if (!Utils.IsEmpty(dateEnd.Text))
                where += " AND n.GGRQ<'" + Convert.ToDateTime(dateEnd.Text).AddDays(1) + "'";
            if (!Utils.IsEmpty(txtTableName.Text))
                where += " AND n.TableName='" + txtTableName.QueryValue + "'";

            string q = @"SELECT OB_OBJECT_ID,CreateTime,TableName,Code,GGRQ,Title,Extension,INBBM,IGSDM,
                      ResourceURL From cfg.dmip_Resource Where storepath In
                     (Select storepath From cfg.dmip_ResourceSync m LEFT JOIN cfg.dmip_Resource n 
                      ON n.ob_object_id=m.ob_object_id" + where 
                    + " Group By md5,storepath Having Count(0)>1)";
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
    }
}
