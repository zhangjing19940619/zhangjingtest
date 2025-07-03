using Deduce.Common.Entity;
using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;
using Deduce.DMIP.ResourceManage;
using Deduce.DMIP.Sys.SysData;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Deduce.DMIP.ResourceSync
{
    /// <summary>
    /// 三板公告查询界面
    /// 重要说明：
    /// 1、对于“三版公告分类指引”中的各个分类节点切勿随意拖动，拖动之后需要对历史数据进行重刷，以便将公告调整至新的分类节点下；
    /// 2、查询公告时，对于曾经有过修改的分类节点，只展示其最新的归属，历史分类不展示，历史数据会进行重刷（章文俊 2015-10-12）；
    /// 3、当status！=2，即同步失败的情况下，结果集中的“修改时间”实际取的是“cfg.dmip_ResourceSync.createtime”，章文俊需要知道
    /// 该公告在库中存在了多久。
    /// 4、对于结果集的默认排序：（1）上传成功的，按照同步时间逆序排列；（2）上传失败的按照cfg.dmip_ResourceSync.createtime逆序排列；
    /// </summary>        
    public partial class frmSBGGQuery : DataForm
    {
        DataTable _dtSource = null;
        Dictionary<string, StockCode> _stockCodes = new Dictionary<string, StockCode>();
        List<string> _ggids = new List<string>();
        Dictionary<string, string> _xgry = new Dictionary<string, string>();//缓存数据-XGRY        
        DataTable _dtGPDM = null;
        DataTable _dtGGLY = null;
        DataTable _dtXGRY = null;
        DataTable _dtXGLJ = null;

        DataTable _dtSelectGPDM = null;
        DataTable _dtSelectGGLY = null;
        DataTable _dtSelectXGRY = null;
        DataTable _dtSelectXGLJ = null;

        Dictionary<string, string> _selectedGPDMs = new Dictionary<string, string>();
        Dictionary<string, string> _selectedGGLYs = new Dictionary<string, string>();
        Dictionary<string, string> _selectedXGRYs = new Dictionary<string, string>();
        Dictionary<string, string> _selectedXGLJs = new Dictionary<string, string>();
        private readonly SBQueryHelper _sbQueryHelper;
        private readonly frmSBGGModify _frmModify;
        private readonly ILogger<frmSBGGQuery> _logger;
        List<string> _GGIDs = new List<string>();
        Dictionary<string, string> _gglyPairs = new Dictionary<string, string>();
        Dictionary<string, string> _gglbs = new Dictionary<string, string>();
        Dictionary<string, string> _ggmclbs = new Dictionary<string, string>();
        int _rowIndex = 0;
        List<string> _selectedStatus = new List<string>();
        CacheData _cache = null;
        bool _setStatus = false;

        public frmSBGGQuery(SBQueryHelper sbQueryHelper, frmSBGGModify frmModify, ILogger<frmSBGGQuery> logger)
        {
            _sbQueryHelper = sbQueryHelper ?? throw new ArgumentNullException(nameof(sbQueryHelper));
            _frmModify = frmModify ?? throw new ArgumentNullException(nameof(frmModify));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            InitializeComponent();
        }
        public void DisplayWindow(string menuID)
        {
            base.MenuID = menuID;
            InitData();
            this.WindowState = FormWindowState.Maximized;
            this.Show();
        }

        private void InitData()
        {
            LoadTableData();
            _dtSource = Utils.TableAddColumns("序号,ID,同步状态,公告日期,证券代码,INBBM,IGSDM,证券简称,公告标题," +
                "公告类别,公开标识,修改日期,修改人员,公告来源,同步时间,文件格式,公告链接,md5");
            _dtSelectGPDM = Utils.TableAddColumns("代码,名称");
            _dtSelectGGLY = Utils.TableAddColumns("代码,名称");
            _dtSelectXGRY = Utils.TableAddColumns("代码,名称");
            _dtSelectXGLJ = Utils.TableAddColumns("代码,名称");
            _dtXGLJ = Utils.TableAddColumns("LJID,LJMC");
            txtGGRQStart.Text = DateTime.Today.ToShortDateString().Replace("/", "-");
            txtGGRQEnd.Text = DateTime.Today.ToShortDateString().Replace("/", "-"); ;
            txtXGRQStart.Text = DateTime.Today.ToShortDateString().Replace("/", "-");
            txtXGRQEnd.Text = DateTime.Today.ToShortDateString().Replace("/", "-");
            SetDropListData();
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            Query();
        }

        private void Query()
        {
            btnQuery.Enabled = false;
            pageQuery.DataPagingEvent -= new GridDataHander(BindData);
            pageQuery.DataPagingEvent += new GridDataHander(BindData);
            pageQuery.InitData(SetQueryText(), _menuID);
            btnQuery.Enabled = true;
        }
        private List<string> GetChkNodeIDs(List<string> ckNodes)
        {
            List<string> selectedlbs = new List<string>();
            if (ckNodes == null || ckNodes.Count == 0)
                return selectedlbs;

            foreach (var lbmc in ckNodes)
            {
                if (_ggmclbs.ContainsKey(lbmc))
                {
                    selectedlbs.Add(_ggmclbs[lbmc]);
                }
            }
            return selectedlbs;
        }
        private void BindData(DataTable dt)
        {
            this.gridSBGG.DataSource = GetDatas(dt);
            SetColumWidthAndVisible();
        }
        private void CheckStatusSet()
        {
            _selectedStatus.Clear();
            _setStatus = false;
            foreach (var item in statusSelect.CheckedIndices)
            {
                _setStatus = true;
                if (!_selectedStatus.Contains(item.ToString()))
                    _selectedStatus.Add(item.ToString());
            }
        }

        private void SetColumWidthAndVisible()
        {
            DataTable dt = (DataTable)gridSBGG.DataSource;
            if (dt != null && dt.Rows.Count != 0)
                gridSBGG.Rows[dt.Rows.Count].DefaultCellStyle.BackColor = Color.DarkGray;

            int width = gridSBGG.Width;
            foreach (DataGridViewColumn item in gridSBGG.Columns)
            {
                item.Width = width / 10;
                if (item.Name == "公告标题")
                    item.Width += 300;

                if (item.Name == "公开标识" || item.Name == "证券代码" || item.Name == "修改人员"
                    || item.Name == "序号" || item.Name == "证券简称" || item.Name == "公告日期")
                    item.Width -= 50;

                if (_setStatus && item.Name == "公告日期")
                    item.Width += 30;

                if (item.Name == "序号" || item.Name == "ID" || item.Name == "IGSDM" ||
                    item.Name == "INBBM" || item.Name == "公告来源" || item.Name == "文件格式" ||
                    item.Name == "公告链接" || item.Name == "md5")
                {
                    item.Visible = false;
                }

                if (item.Name == "同步状态")
                    item.Visible = _setStatus;
                else if (item.Name == "同步时间")
                    item.Visible = !_setStatus;
            }
        }
        private void btnClearRule_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes != MessageBox.Show("确定清除所有限定条件吗?", "清除", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                return;

            ClearCondition();
        }

        /// <summary>
        /// 清除所有查询限定条件
        /// </summary>
        private void ClearCondition()
        {
            //1、公告日期
            txtGGRQStart.Text = "";
            txtGGRQEnd.Text = "";

            //2、公告来源
            txtGGLY.Text = "";
            _dtSelectGGLY.Clear();
            _selectedGGLYs.Clear();
            gridSelectedGGLY.DataSource = _dtSelectGGLY;

            //3、证券代码
            txtGPDM.Text = "";
            _dtSelectGPDM.Clear();
            _selectedGPDMs.Clear();
            gridSelectedGPDM.DataSource = _dtSelectGPDM;

            //4、公告标题
            txtGGBT.Text = "";

            //5、公告类别
            txtGGLB.Text = "";
            frmSelectGGLB.CheckedLB.Clear();

            //6、修改人员
            txtXGRY.Text = "";
            _dtSelectXGRY.Clear();
            _selectedXGRYs.Clear();
            gridSelectedXGRY.DataSource = _dtSelectXGRY;

            //7、修改日期
            txtXGRQStart.Text = "";
            txtXGRQEnd.Text = "";

            //8、公开标识            
            boxPublic.Checked = false;
            boxUnpublic.Checked = false;

            //9、修改路径
            txtXGLJ.Text = "";
            _dtSelectXGLJ.Clear();
            _selectedXGLJs.Clear();
            gridSelectedXGLJ.DataSource = _dtSelectXGLJ;

            //10、去掉所有同步失败文件的选择
            ResetStatus();
        }
        private void ResetStatus()
        {
            if (!_setStatus)
                return;

            for (int i = 0; i < statusSelect.Items.Count; i++)
            {
                statusSelect.SetItemChecked(i, false);
            }
            _setStatus = false;
        }

        /// <summary>
        /// 依据查询条件设置SQL脚本
        /// </summary>
        /// <returns></returns>
        private string SetQueryText()
        {
            string q = "";
            string qAppend = "";
            #region 查看同步失败的资源文件
            if (_setStatus)
            {
                q = @"select  ROW_NUMBER() OVER(ORDER BY a.createtime desc) As ROW_NUM, a.*, b.GGRQ, b.extension 
                   from cfg.dmip_ResourceSync(nolock) a,cfg.dmip_Resource(nolock) b
                   where a.ob_object_id = b.ob_object_id and a.TableName = 'usrSBGGB' and  ";
                qAppend += _sbQueryHelper.GetQueryScript(_selectedStatus, "a.status");
                if (!Utils.IsEmpty(txtGGBT.Text))
                {
                    qAppend += @" and a.title like  '%" + txtGGBT.Text + "%'";
                }

                if (_dtSelectGPDM != null && _dtSelectGPDM.Rows.Count != 0)
                {
                    DataTable dt = Utils.TableAddColumns("内码");
                    foreach (DataRow dr in _dtSelectGPDM.Rows)
                    {
                        DataRow drnew = dt.NewRow();
                        if (!_stockCodes.ContainsKey(dr[0].ToString()))
                            continue;

                        drnew["内码"] = _stockCodes[dr[0].ToString()].INBBM;
                        dt.Rows.Add(drnew);
                    }
                    qAppend += _sbQueryHelper.AnalyseQueryFields(dt, "a.inbbm");
                }
                qAppend += _sbQueryHelper.GetRQRange(txtGGRQStart.Text, txtGGRQEnd.Text, "b.GGRQ");
                if (_dtSelectGGLY != null && _dtSelectGGLY.Rows.Count != 0)
                {
                    qAppend += _sbQueryHelper.AnalyseQueryFields(_dtSelectGGLY, "MediaSource");
                }
                return q + qAppend;
            }
            #endregion

            #region  查看同步成功的资源文件
            q = @"Select  ROW_NUMBER() OVER(ORDER BY a.GKBZ ASC, a.FBSJ DESC) As ROW_NUM ,a.*, b.LJGGBM,
                   b.WJGGBM, b.SIJGGBM,b.SAJGGBM,b.EJGGBM,  b.YJGGBM 
                    From  usrSBGGB(nolock) a, usrSBGGLBB(nolock) b  Where a.ID = b.GGID and  a.HASHCODE Is Not NULL ";
            //1、公告标题
            if (!Utils.IsEmpty(txtGGBT.Text))
                qAppend += @" and a.GGBT like  '%" + txtGGBT.Text + "%'";

            //2、证券代码
            if (_dtSelectGPDM != null && _dtSelectGPDM.Rows.Count != 0)
            {
                DataTable dt = Utils.TableAddColumns("内码");
                foreach (DataRow dr in _dtSelectGPDM.Rows)
                {
                    DataRow drnew = dt.NewRow();
                    if (!_stockCodes.ContainsKey(dr[0].ToString()))
                        continue;

                    drnew["内码"] = _stockCodes[dr[0].ToString()].INBBM;
                    dt.Rows.Add(drnew);
                }
                qAppend += _sbQueryHelper.AnalyseQueryFields(dt, "a.INBBM");
            }
            //3、公告来源
            if (_dtSelectGGLY != null && _dtSelectGGLY.Rows.Count != 0)
            {
                qAppend += _sbQueryHelper.AnalyseQueryFields(_dtSelectGGLY, "a.GGLY");
            }
            //4、修改人员
            if (_dtSelectXGRY != null && _dtSelectXGRY.Rows.Count != 0)
            {
                qAppend += _sbQueryHelper.AnalyseQueryFields(_dtSelectXGRY, "a.XGRY");
            }
            //5、公告日期
            qAppend += _sbQueryHelper.GetRQRange(txtGGRQStart.Text, txtGGRQEnd.Text, "a.GGRQ");

            //6、公开标识
            if (boxPublic.Checked && !boxUnpublic.Checked)
            {
                qAppend += @" and a.GKBZ = 3";
            }
            else if (!boxPublic.Checked && boxUnpublic.Checked)
            {
                qAppend += @" and a.GKBZ != 3";
            }

            //7、修改时间
            qAppend += _sbQueryHelper.GetRQRange(txtXGRQStart.Text, txtXGRQEnd.Text, "a.XGSJ");
            //8、公告类别
            List<string> selectLB = frmSelectGGLB.CheckedLB;
            List<string> selectedlbs = GetChkNodeIDs(selectLB);
            if (selectLB.Count != 0)
            {
                qAppend += (" and (" + _sbQueryHelper.GetQueryScripts(
                    selectedlbs, new List<string>() { "YJGGBM", "EJGGBM", "SAJGGBM", "SIJGGBM", "WJGGBM", "LJGGBM" }) + ")");
            }
            //9、修改路径-人工或者自动
            if (_selectedXGLJs.Keys.Count == 1)
            {
                GetXGLJ();
                qAppend += (_GGIDs.Count == 0) ? " and a.ID in ('') " :
                    (" and (" + _sbQueryHelper.GetQueryScript(_GGIDs, "a.ID") + ")");
            }
            #endregion
            return q + qAppend;
        }
        public void GetXGLJ()
        {
            string q = "select b.GGID as ID from cfg.dmip_Resource(nolock) a, cfg.dmip_ResourceSync(nolock) b  where ";
            q += (_selectedXGLJs.Keys.First() == "1") ? " a.resourceURL is not null" : "a.resourceURL is null";
            q += " and a.ob_object_id = b.ob_object_id";

            _GGIDs.Clear();
            DataTable dt = _data.GetDataTable(q, base.MenuID);
            if (dt != null && dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    _GGIDs.Add(dr["ID"].ToString());
                }
            }
        }

        private DataTable GetDatas(DataTable dt)
        {
            if (Utils.IsEmpty(dt))
                return null;

            DataTable drResult = _dtSource.Clone();
            //此处针对未能同步成功的数据-从表cfg.dmip_ResourceSync中查询结果
            if (_setStatus)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string ID = dr["GGID"].ToString();
                    DataRow row = drResult.NewRow();
                    row["序号"] = dr["ROW_NUM"].ToString();
                    row["ID"] = ID;
                    string status = dr["status"].ToString();
                    row["同步状态"] = SyncStatusDes.GetSyncStatus.ContainsKey(status) ?
                        SyncStatusDes.GetSyncStatus[status] : status;
                    row["公告标题"] = dr["title"].ToString();
                    row["公告日期"] = Convert.ToDateTime(dr["GGRQ"].ToString()).ToShortDateString();
                    row["修改日期"] = dr["createtime"].ToString();
                    row["公告链接"] = dr["url"].ToString();
                    row["同步时间"] = dr["syncTime"].ToString();
                    row["文件格式"] = dr["extension"].ToString();
                    row["md5"] = dr["md5"].ToString();
                    string inbbm = dr["inbbm"].ToString();
                    row["IGSDM"] = dr["igsdm"].ToString();
                    row["INBBM"] = dr["inbbm"].ToString();
                    if (_stockCodes.Keys.Count != 0 && _stockCodes.ContainsKey(inbbm))
                    {
                        row["证券代码"] = _stockCodes[inbbm].Code;
                        row["证券简称"] = _stockCodes[inbbm].Caption;
                    }
                    row["公告来源"] = dr["MediaSource"].ToString();

                    drResult.Rows.Add(row);
                }
                return drResult;
            }

            foreach (DataRow dr in dt.Rows)
            {
                string ID = dr["ID"].ToString();
                DataRow row = drResult.NewRow();
                row["序号"] = dr["ROW_NUM"].ToString();
                row["ID"] = ID;
                row["公告日期"] = Convert.ToDateTime(dr["GGRQ"].ToString()).ToShortDateString();
                row["公告标题"] = dr["GGBT"].ToString();
                row["公开标识"] = _sbQueryHelper.GetGKBZName(dr["GKBZ"].ToString());
                row["修改日期"] = dr["XGSJ"].ToString();
                row["修改人员"] = dr["XGRY"].ToString();
                string lbbm = _sbQueryHelper.GetGGLBBM(dr);
                row["公告类别"] = _gglbs.ContainsKey(lbbm.Trim()) ? _gglbs[lbbm.Trim()] : "";
                row["同步时间"] = dr["FBSJ"].ToString();
                row["IGSDM"] = dr["IGSDM"].ToString();
                row["INBBM"] = dr["INBBM"].ToString();
                if (_xgry.Keys.Count != 0 && _xgry.ContainsKey(dr["XGRY"].ToString()))
                    row["修改人员"] = _xgry[dr["XGRY"].ToString()];

                string inbbm = dr["INBBM"].ToString();
                if (_stockCodes.Keys.Count != 0 && _stockCodes.ContainsKey(inbbm))
                {
                    row["证券代码"] = _stockCodes[inbbm].Code;
                    row["证券简称"] = _stockCodes[inbbm].Caption;
                }
                row["公告来源"] = dr["GGLY"].ToString();
                row["文件格式"] = ResourceTypes.GetExtension(dr["WJGS"].ToString());

                drResult.Rows.Add(row);
            }
            return drResult;
        }

        private void btnSeeSQL_Click(object sender, EventArgs e)
        {
            frmSqlQuery frm = new frmSqlQuery(SetQueryText());
            frm.Text = "三板公告查询SQL脚本";
            frm.Show();
        }

        private void gridSBGG_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                if (gridSBGG == null || gridSBGG.Rows.Count == 0)
                    return;

                int index = gridSBGG.CurrentRow.Index;
                _rowIndex = index;
                LoadModifyForm(index);
                _frmModify.RefreshDataEvent += new RefreshData(ResetGridData);
                e.Handled = true;
            }
        }

        private void ResetGridData(SBGGData sd)
        {
            if (sd == null)
                return;

            DataGridViewRow row = gridSBGG.Rows[_rowIndex];
            row.Cells["公告标题"].Value = sd.GGBT;
            row.Cells["证券代码"].Value = sd.GPDM;
            row.Cells["证券简称"].Value = _stockCodes.ContainsKey(sd.GPDM) ? _stockCodes[sd.GPDM].Caption : "";
            row.Cells["公告来源"].Value = sd.GGLY;
            if (_setStatus)
            {
                row.Cells["同步状态"].Value = "0";
                return;
            }

            row.Cells["公告日期"].Value = sd.GGRQ;
            row.Cells["公告类别"].Value = sd.GGLB;
            row.Cells["公开标识"].Value = _sbQueryHelper.GetGKBZName(sd.GKBZ);
            row.Cells["修改日期"].Value = sd.XGSJ;
            row.Cells["修改人员"].Value = sd.XGRY;
            row.Cells["同步时间"].Value = sd.TBSJ;
        }

        /// <summary>
        /// 加载修改界面
        /// </summary>
        private void LoadModifyForm(int currentSelectedRow)
        {
            DataGridViewRow dr = gridSBGG.Rows[currentSelectedRow];
            string gridRowsCount =(gridSBGG == null || gridSBGG.Rows.Count == 0)
                ? "0" : (gridSBGG.Rows.Count - 1).ToString();
            if (_cache == null)
                _cache = new CacheData(_dtGPDM, _dtGGLY, _stockCodes, _gglyPairs);

            if (_frmModify == null)
            {
                _frmModify.HideGGInfo();
            }
            _frmModify.DataIndex = currentSelectedRow;
            gridSBGG.Rows[currentSelectedRow].Selected = true;
            _frmModify.GridClickEvent -= new GridClickDelegateHander(ResetSelectGridData);
            _frmModify.GridClickEvent += new GridClickDelegateHander(ResetSelectGridData);
            _frmModify.DisplayWindows(dr, _cache, currentSelectedRow, _setStatus, base.MenuID, gridRowsCount);
        }
        private void ResetSelectGridData(int index)
        {
            _rowIndex = index;
            gridSBGG.ClearSelection();

            gridSBGG.Rows[_rowIndex].Selected = true;
            gridSBGG.FirstDisplayedScrollingRowIndex = index;
            _frmModify.DataIndex = index;
            _frmModify.ResetModifyData(gridSBGG.Rows[index]);
        }

        private void LoadTableData()
        {
            string query = "";
            //1、加载证券主表数据--若已经加载则无需加载
            if (Utils.IsEmpty(_dtGPDM )|| _stockCodes.Keys.Count == 0)
            {
                query = @" Select INBBM, GPDM+' '+ZQJC+'-'+ZWMC As ZQJC1, GPDM, IGSDM, ZQJC  
                            From usrZQZB(nolock) Where ZQSC = 81 OR ZQSC = 18 OR INBBM = 2611";
                _dtGPDM = _data.GetDataTable(query, this.MenuID);
                if (_dtGPDM != null && _dtGPDM.Rows.Count != 0)
                {
                    foreach (DataRow dr in _dtGPDM.Rows)
                    {
                        StockCode sc = new StockCode(dr);
                        if (!_stockCodes.ContainsKey(sc.INBBM))
                        {
                            _stockCodes.Add(sc.INBBM, sc);
                        }
                        if (!_stockCodes.ContainsKey(sc.Code))
                        {
                            _stockCodes.Add(sc.Code, sc);
                        }
                    }
                }
            }

            //2、加载公告来源信息--若已经加载则无需加载
            if (Utils.IsEmpty(_dtGGLY))
            {
                query = @"Select LYDM, str(LYDM) + ' ' + LYMS As  LYMS From cmdXXLYPPGZ(nolock) Where SYZT = 1  
                         Group By LYDM, LYMS Order By LYDM";
                _dtGGLY = _data.GetDataTable(query, base.MenuID);
                if (_dtGGLY != null && _dtGGLY.Rows.Count != 0)
                {
                    foreach (DataRow dr in _dtGGLY.Rows)
                    {
                        if (!_gglyPairs.ContainsKey(dr["LYDM"].ToString()))
                        {
                            _gglyPairs.Add(dr["LYDM"].ToString(), dr["LYMS"].ToString());
                        }
                    }
                }
            }

            //3、加载修改人员表--若已经加载则无需加载
            if (Utils.IsEmpty(_dtXGRY))
            {
                query = @" Select USERID, USERID+' ' + CNNAME As XGRY, CNNAME From cfg.dmip_OPERATEUSER(nolock) Where USERID  != ''";
                _dtXGRY = _data.GetDataTable(query, base.MenuID);
                if (_dtXGRY != null && _dtXGRY.Rows.Count != 0)
                {
                    foreach (DataRow dr in _dtXGRY.Rows)
                    {
                        if (!_xgry.ContainsKey(dr["USERID"].ToString()))
                        {
                            _xgry.Add(dr["USERID"].ToString(), dr["CNNAME"].ToString());
                        }
                    }
                }
            }

            //4、加载公告类别数据--若已经加载则无需加载
            if (_gglbs.Keys.Count == 0)
            {
                query = @"select LBBM, LBMC from usrSBGGFLZYB(nolock) ";
                DataTable dt = _data.GetDataTable(query, base.MenuID);
                if (dt != null && dt.Rows.Count != 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (!_gglbs.ContainsKey(dr["LBBM"].ToString()))
                        {
                            _gglbs.Add(dr["LBBM"].ToString().Trim(), dr["LBMC"].ToString());
                        }
                        if (!_ggmclbs.ContainsKey(dr["LBMC"].ToString().Trim()))
                        {
                            _ggmclbs.Add(dr["LBMC"].ToString(), dr["LBBM"].ToString().Trim());
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 设定筛选字段-下拉框选择
        /// </summary>
        private void SetDropListData()
        {
            txtGPDM.DataSource = _dtGPDM;
            txtGPDM.DisplayMember = "ZQJC1";
            txtGPDM.ValueMember = "INBBM";

            txtGGLY.DataSource = _dtGGLY;
            txtGGLY.DisplayMember = "LYMS";
            txtGGLY.ValueMember = "LYDM";

            txtXGRY.DataSource = _dtXGRY;
            txtXGRY.DisplayMember = "XGRY";
            txtXGRY.ValueMember = "USERID";
        }
        private void btnGGLB_Click(object sender, EventArgs e)
        {
            string[] lbs = Utils.SplitString(txtGGLB.Text, ";");
            if (lbs != null && lbs.Count() != 0)
            {
                frmSelectGGLB.CheckedLB = lbs.ToList();
            }

            frmSelectGGLB frm = new frmSelectGGLB(base.MenuID);
            frm.ClickEvent += new ClickDelegateHander(GetMessage);
            frm.Show();
        }
        public void GetMessage(string message)
        {
            this.txtGGLB.Text = message;
        }

        private void gridSBGG_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,e.RowBounds.Location.Y,
                gridSBGG.RowHeadersWidth - 4,e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                gridSBGG.RowHeadersDefaultCellStyle.Font, rectangle, Color.Red,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void txtGGLY_KeyDown(object sender, KeyEventArgs e)
        {
            txtGGLY.GetSelectedValueEvent -= new GetSelectedValueHander(SetGGLYText);
            txtGGLY.GetSelectedValueEvent += new GetSelectedValueHander(SetGGLYText);
        }
        private void SetGGLYText(string selectedText)
        {
            txtGGLY.Text = selectedText;
            string showMsg = @"该公告来源已添加，请重新选择！!";
            _sbQueryHelper.AddSelectedData(_selectedGGLYs, _dtSelectGGLY, txtGGLY.Text, showMsg, gridSelectedGGLY);
            txtGGLY.Focus();
            txtGGLY.SelectAll();
        }
        private void txtGPDM_KeyDown(object sender, KeyEventArgs e)
        {
            txtGPDM.GetSelectedValueEvent -= new GetSelectedValueHander(SetGPDMText);
            txtGPDM.GetSelectedValueEvent += new GetSelectedValueHander(SetGPDMText);
        }
        private void SetGPDMText(string selectedText)
        {
            txtGPDM.Text = selectedText;
            string showMsg = @"该证券代码已添加，请重新选择！!";
            _sbQueryHelper.AddSelectedData(_selectedGPDMs, _dtSelectGPDM, txtGPDM.Text, showMsg, gridSelectedGPDM);
            txtGPDM.Focus();
            txtGPDM.SelectAll();
        }
        private void txtXGRY_KeyDown(object sender, KeyEventArgs e)
        {
            txtXGRY.GetSelectedValueEvent -= new GetSelectedValueHander(SetXGRYText);
            txtXGRY.GetSelectedValueEvent += new GetSelectedValueHander(SetXGRYText);
        }
        private void SetXGRYText(string selectedText)
        {
            txtXGRY.Text = selectedText;
            string showMsg = @"该修改人员已添加，请重新选择！!";
            _sbQueryHelper.AddSelectedData(_selectedXGRYs, _dtSelectXGRY, txtXGRY.Text, showMsg, gridSelectedXGRY);
            txtXGRY.Focus();
            txtXGRY.SelectAll();
        }
        private void gridSelectedGGLY_DoubleClick(object sender, EventArgs e)
        {
            RemoveData(gridSelectedGGLY, _selectedGGLYs);
            if (_selectedGGLYs.Keys.Count == 0)
                txtGGLY.Text = "";
        }
        private void gridSelectedGPDM_DoubleClick(object sender, EventArgs e)
        {
            RemoveData(gridSelectedGPDM, _selectedGPDMs);
            if (_selectedGPDMs.Keys.Count == 0)
                txtGPDM.Text = "";
        }
        private void gridSelectedXGRY_DoubleClick(object sender, EventArgs e)
        {
            RemoveData(gridSelectedXGRY, _selectedXGRYs);
            if (_selectedXGRYs.Keys.Count == 0)
                txtXGRY.Text = "";
        }
        private void gridSelectedXGLJ_DoubleClick(object sender, EventArgs e)
        {
            RemoveData(gridSelectedXGLJ, _selectedXGLJs);
            if (_selectedXGLJs.Keys.Count == 0)
                txtXGLJ.Text = "";
        }
        private void RemoveData(DataGridView dgv, Dictionary<string, string> data)
        {
            if (dgv == null || dgv.Rows.Count == 0)
                return;

            int currentIndex = dgv.CurrentRow.Index;
            string key = dgv.Rows[currentIndex].Cells[0].Value.ToString();
            data.Remove(key);
            dgv.Rows.RemoveAt(currentIndex);
        }

        private void txtGGLB_Leave(object sender, EventArgs e)
        {
            if (Utils.IsEmpty(txtGGLB.Text))
            {
                frmSelectGGLB.CheckedLB.Clear();
            }
        }
        private void gridClearAllSelectedZQDM(object sender, MouseEventArgs e)
        {
            string txt = txtGPDM.Text;
            ClearAllSelectedItem(_dtSelectGPDM, _selectedGPDMs, gridSelectedGPDM.DataSource, e.Button, ref txt);
            txtGPDM.Text = "";
        }
        private void gridClearAllSelectedGGLY(object sender, MouseEventArgs e)
        {
            string txt = txtGGLY.Text;
            ClearAllSelectedItem(_dtSelectGGLY, _selectedGGLYs, gridSelectedGGLY.DataSource, e.Button, ref txt);
            txtGGLY.Text = txt;
        }
        private void gridClearAllSelectedXGRY(object sender, MouseEventArgs e)
        {
            string txt = txtXGRY.Text;
            ClearAllSelectedItem(_dtSelectXGRY, _selectedXGRYs, gridSelectedXGRY.DataSource, e.Button, ref txt);
            txtXGRY.Text = txt;
        }
        private void gridClearAllSelectedXGLJ(object sender, MouseEventArgs e)
        {
            string txt = txtXGLJ.Text;
            ClearAllSelectedItem(_dtSelectXGLJ, _selectedXGLJs, gridSelectedXGLJ.DataSource, e.Button, ref txt);
            txtXGLJ.Text = txt;
        }
        private void ClearAllSelectedItem(DataTable dtSelected, Dictionary<string, string> dicSelected, object DataSource, MouseButtons bt, ref string txt)
        {
            if (Utils.IsEmpty(dtSelected))
                return;

            if (bt != System.Windows.Forms.MouseButtons.Right)
                return;

            if (DialogResult.Yes != MessageBox.Show("确定清除所有选中项吗？", "清除提示", MessageBoxButtons.YesNo))
                return;

            dtSelected.Clear();
            dicSelected.Clear();
            DataSource = dtSelected;
            txt = "";
        }

        private void frmSBGGQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Q && e.Modifiers == Keys.Control)
            {
                Query();
            }
        }

        private void gridSBGG_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            _rowIndex = gridSBGG.CurrentRow.Index;
            LoadModifyForm(_rowIndex);
            _frmModify.RefreshDataEvent += new RefreshData(ResetGridData);
        }
        private void txtXGLJ_SelectedIndexChanged(object sender, EventArgs e)
        {
            string showMsg = @"该修改路径已添加，请重新选择！!";
            _sbQueryHelper.AddSelectedData(_selectedXGLJs, _dtSelectXGLJ, txtXGLJ.Text, showMsg, gridSelectedXGLJ);
        }
        private void statusSelect_Leave(object sender, EventArgs e)
        {
            CheckStatusSet();
        }
    }
}