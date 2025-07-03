using Deduce.Common.Entity;
using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;
using Deduce.DMIP.ResourceSync.Server;
using Deduce.DMIP.Sys.SysData;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Deduce.DMIP.ResourceSync
{
    public delegate void GridClickDelegateHander(int index);
    public delegate void RefreshData(SBGGData msg);
    public partial class frmSBGGModify : ExtendDataForm
    {
       public event GridClickDelegateHander GridClickEvent;
        public event RefreshData RefreshDataEvent;
        
        private readonly SBQueryHelper _sbQueryhelper;
        private readonly SBSyncHelper _syncHelper;
        private readonly UpdateSBGGTables _updateSBTables;
        private readonly frmLBModify _frmLBModify;
        
        private string _fileDownLoadPath = "";
        private string _exchangeFilePath = "";
        private string _exchangeMD5 = "";
        private SBGGData _sd = null;
        private List<string> _comparesData = new List<string>();
        private CacheData _cacheData = null;
        private string _ggCount = "0";
        private Dictionary<string, string> _chkNodeParents = new Dictionary<string, string>();
        private TreeNode _curNode = null;
        private int _index = 0;
        private bool _setStatus = false;

        public frmSBGGModify(SBQueryHelper sbQueryhelper, SBSyncHelper syncHelper, UpdateSBGGTables updateSBTables, frmLBModify frmLBModify)
        {
            _sbQueryhelper = sbQueryhelper ?? throw new ArgumentNullException(nameof(sbQueryhelper));
            _syncHelper = syncHelper ?? throw new ArgumentNullException(nameof(syncHelper));
            _updateSBTables = updateSBTables ?? throw new ArgumentNullException(nameof(updateSBTables));
            _frmLBModify= frmLBModify ?? throw new ArgumentNullException(nameof(frmLBModify));
            InitializeComponent();
            _isEscapeClose = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            _fileDownLoadPath = Utils.GetTempPath;
        }


        public void DisplayWindows(DataGridViewRow dr, CacheData cd, int index, bool setStatus, string menuID, string ggCount)
        {
            if (dr == null)
                return;

            base.MenuID = menuID;
            _cacheData = cd;
            _ggCount = ggCount;
            _index = index;
            _setStatus = setStatus;
            InitData(dr, cd);
            this.ShowDialog();
        }
        private void InitData(DataGridViewRow dr, CacheData cd)
        {
            if (dr == null)
                return;

            _sd = new SBGGData(dr);
            _comparesData = new List<string>() { _sd.INBBM, _sd.GGBT, _sd.GGRQ, _sd.GKBZ, _sd.GGLB, _sd.ID };
            if (!_setStatus)
            {
                boxGKBZ.DropDownStyle = ComboBoxStyle.DropDown;
                boxGKBZ.DropDownHeight = 100;
                btnExchangeFile.Enabled = true;
                btnGGLB.Enabled = true;
                txtGGLB.ReadOnly = false;
                btnExchangeFile.Text = "替换文件";
            }
            else
            {
                boxGKBZ.DropDownStyle = ComboBoxStyle.DropDownList;
                boxGKBZ.DropDownHeight = 1;
                btnGGLB.Enabled = false;
                txtGGLB.ReadOnly = true;
                //此处针对Md5存在的情况进行处理
                if (!Utils.IsEmpty(_sd.TBZT) && (_sd.TBZT.Equals("Md5存在") || _sd.TBZT.Equals("4")))
                {
                    btnExchangeFile.Enabled = true;
                    if (btnExchangeFile.Text == "隐藏信息")
                    {
                        ShowGGInfo();
                    }
                    else
                    {
                        btnExchangeFile.Text = "OSS文件";
                    }
                }
                else
                {
                    btnExchangeFile.Enabled = false;
                    btnExchangeFile.Text = "替换文件";
                }
            }
            txtGGBT.Text = _sd.GGBT;
            txtGGLB.Text = _sd.GGLB;
            txtGGLY.Text = _sd.GGLY;
            if (cd.GGLYPair.ContainsKey(txtGGLY.Text))
            {
                txtGGLY.Text = cd.GGLYPair[txtGGLY.Text].Replace(" ", "");
            }
            txtGGRQ.DataValue = Utils.IsEmpty(_sd.GGRQ) ? "" : _sd.GGRQ;
            txtGPDM.Text = _sd.GPDM;
            txtSelectedCode.Text = _sd.GPDM;
            if (cd.stockCode.ContainsKey(txtGPDM.Text))
            {
                txtGPDM.Text += (cd.stockCode[_sd.GPDM].Caption.Replace(" ", ""));
                txtSelectedCode.Text += ("-" + cd.stockCode[_sd.GPDM].Caption.Replace(" ", ""));
            }

            boxGKBZ.Text = ResourceTypes.GKBZData.GetGKBZMc.ContainsKey(_sd.GKBZ) ? ResourceTypes.GKBZData.GetGKBZMc[_sd.GKBZ] + " " + _sd.GKBZ : _sd.GKBZ;
            txtGGCount.Text = _index + 1 + "/" + _ggCount;
            //当公开标识为3=公开时，则不能修改为其他的状态
            if (!Utils.IsEmpty(boxGKBZ.Text) && (boxGKBZ.Text == "3 公开" || boxGKBZ.Text == "公开"))
            {
                boxGKBZ.DropDownHeight = 1;
            }
            //缓存数据
            txtGPDM.DataSource = cd.dtGPDM;
            txtGPDM.DisplayMember = "ZQJC1";
            txtGPDM.ValueMember = "INBBM";

            txtGGLY.DataSource = cd.dtGGLY;
            txtGGLY.DisplayMember = "LYMS";
            txtGGLY.ValueMember = "LYDM";
        }
        public void ShowGGInfo()
        {
            SetGGInfo();
            this.pnlExistFile.Visible = true;
        }
        public void HideGGInfo()
        {
            this.pnlExistFile.Visible = false;
            this.Width = 360;
        }
        private void btnSeeFile_Click(object sender, EventArgs e)
        {
            if (_setStatus)
            {
                if (!Utils.IsEmpty(_sd.LINK))
                {
                    System.Diagnostics.Process.Start(_sd.LINK);
                }
                return;
            }
            _sbQueryhelper.GetOssFile(_sd, _fileDownLoadPath);
        }

        private void btnExchangeFile_Click(object sender, EventArgs e)
        {
            if (!Utils.IsEmpty(_sd.TBZT) && (_sd.TBZT.Equals("Md5存在") || _sd.TBZT.Equals("4")))
            {
                if (btnExchangeFile.Text == "OSS文件")
                {
                    btnExchangeFile.Text = "隐藏信息";
                    ShowGGInfo();
                }
                else if (btnExchangeFile.Text == "隐藏信息")
                {
                    btnExchangeFile.Text = "OSS文件";
                    HideGGInfo();
                }
            }
            else
            {
                opFileDialog.Title = "请选择替换文件";
                opFileDialog.Filter = "所有文件 (*.*)|*.*";
                if (opFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string[] fileNamesArray = opFileDialog.FileNames;
                    if (fileNamesArray == null || fileNamesArray.Length == 0)
                        return;

                    _exchangeFilePath = fileNamesArray[0];
                    _exchangeMD5 = Utils.GetFileMD5(_exchangeFilePath);
                }
            }
        }

        public int DataIndex
        {
            get { return _index; }
            set { _index = value; }
        }
        public void ResetModifyData(DataGridViewRow dr)
        {
            InitData(dr, _cacheData);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!SaveData())
                return;
            
            if (RefreshDataEvent != null)
            {
                RefreshDataEvent(_sd);
            }
        }
        
        private bool SaveData()
        {
            if (DialogResult.Yes != MessageBox.Show("确定保存更改吗?", "保存", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                return false;

            string ID = _sd.ID;
            //1、对于写表usrSBGGLBB，需要根据当前选中的节点逐次写入各级类别;2、涉及表：Sync、SBGGB、SBGGLBB            
            _sd.GPDM = Utils.IsEmpty(txtGPDM.Text) ? txtGPDM.Text : txtGPDM.Text.Substring(0, 6);
            string code = _sbQueryhelper.GetFindString(txtSelectedCode.Text, "-", false);
            Tuple<string, string> tp = GetCode(code, _cacheData.stockCode);
            if (tp != null)
            {
                _sd.INBBM = tp.Item1;
                _sd.IGSDM = tp.Item2;
            }
            _sd.GGRQ = txtGGRQ.Text;
            _sd.GGBT = txtGGBT.Text;
            _sd.GGLY = _sbQueryhelper.GetDMByDes(txtGGLY.Text);
            _sd.GGLY = (_sd.GGLY == null || Utils.IsEmpty(_sd.GGLY)) ? ""
                : _cacheData.GGLYPair.Where(ms => ms.Value.Trim().Replace(" ", "") == _sd.GGLY).First().Key;
            _sd.GGLB = txtGGLB.Text;
            _sd.GKBZ = _sbQueryhelper.GetDMByDes(boxGKBZ.Text);
            if (_setStatus)
            {
                SaveSyncData(ID, _sd, _setStatus);
            }
            else
            {
                if (!SaveSBGGBData(ID, _sd))
                    return false;
                
                //2、写表usrSBGGLBB-只有当公告类别变化之后才写表数据 
                // 若公告类别没有变化，先将保存标志设置为true 左永忠
                _updateSBTables.SaveLBBSuccess = true;
                if (!Utils.IsEmpty(txtGGLB.Text.Trim()) && _comparesData[4] != txtGGLB.Text.Trim())
                {
                    SaveLBBData(ID, _sd);
                }                
                //3、写表cfg.dmip_ResourceSync-选择替换文件之后才会写表cfg.dmip_ResourceSync
                if (!Utils.IsEmpty(_exchangeFilePath))
                {
                    SaveSyncData(ID, _sd, _setStatus);
                }
                else
                {
                    _updateSBTables.SaveSyncSuccess = true;
                }
            }

            if (_updateSBTables.SaveSBGGBSuccess && _updateSBTables.SaveLBBSuccess && _updateSBTables.SaveSyncSuccess
                || (_setStatus && _updateSBTables.SaveSyncSuccess))
            {
                MessageBox.Show("保存成功！");
                return true;
            }
            return false;
        }
        private bool UniqueFieldsCheck(SBGGData sd)
        {
            if (sd == null)
                return false;

            //未公开状态下无需检查唯一性
            if ((sd.GKBZ == "1 未公开" || sd.GKBZ == "未公开" || sd.GKBZ == "1")
                && (_comparesData[3] == "1 未公开" || _comparesData[3] == "1" || _comparesData[3] == "未公开"))
            {
                return false;
            }

            string q = "select * from usrSBGGB  where INBBM = " + sd.INBBM + " and GGBT = '" + sd.GGBT + "' and GGRQ = '" + sd.GGRQ + "' and GKBZ = '3'";
            DataTable dt = _data.GetDataTable(q, _menuID);
            if (Utils.IsEmpty(dt))
                return false;
            
            if ((sd.GKBZ == "3 公开" || sd.GKBZ == "公开" || sd.GKBZ == "3")
                 && (_comparesData[3] == "1 未公开" || _comparesData[3] == "1" || _comparesData[3] == "未公开"))
                return true;

            foreach (DataRow dr in dt.Rows)
            {
                if (!dr["ID"].ToString().Equals(sd.ID)
                    && (dr["GKBZ"].ToString() == "3" || dr["GKBZ"].ToString() == "3 公开" || dr["GKBZ"].ToString() == "公开"))
                    return true;

                if ((sd.GKBZ == "3 公开" || sd.GKBZ == "公开" || sd.GKBZ == "3")
                    && (_comparesData[3] == "3 公开" || _comparesData[3] == "3" || _comparesData[3] == "公开")
                    && sd.ID == dr["ID"].ToString())
                    return false;
            }
            return false;
        }
        public void SaveSyncData(string ggid, SBGGData sd, bool setStatus)
        {
            string q = (setStatus) ?
                @"select * from cfg.dmip_ResourceSync where md5 = '" + sd.MD5 + "'" :
                @"select a.*, b.GGRQ from cfg.dmip_ResourceSync a, cfg.dmip_Resource b  
                  where a.ob_object_id = b.ob_object_id and a.GGID = '" + ggid + "'";

            DataTable dt = _data.GetDataTable(q, GlobalData.CommonMenuID);
            if (Utils.IsEmpty(dt))
                return;

            DataRow dr = dt.Rows[0];
            dr["inbbm"] = sd.INBBM;
            dr["igsdm"] = sd.IGSDM;
            dr["title"] = sd.GGBT;
            dr["MediaSource"] = sd.GGLY;
            if (setStatus)
            {
                dr["status"] = 0;
                dr["syncTime"] = DBNull.Value;
            }
            else
            {
                dr["LBBM"] = _curNode != null ? _curNode.ToolTipText : "";
                if (Utils.IsEmpty(_exchangeMD5))
                    return;

                string objKey = _syncHelper.CreateObjectKey(dr["GGID"].ToString(), sd.WJGS.ToUpper(), dr["GGRQ"].ToString());
                SBGGSync sync = new SBGGSync();
                sync = _data.PutToOss(File.ReadAllBytes(_exchangeFilePath), objKey, sync);
                if (sync == null || !sync.Success || _exchangeMD5 != sync.OutMD5)
                    return;

                dr["md5"] = _exchangeMD5;
                dr["syncTime"] = DateTime.Now;
                dr["status"] = (Utils.IsEmpty(sd.INBBM) || Utils.IsEmpty(sd.IGSDM)) ?
                    (int)SyncStatus.MD5Exists : (int)SyncStatus.Success;
                dr["publishDate"] = DateTime.Now;
                dr["syncTime"] = DateTime.Now;
            }

            _updateSBTables.SaveSyncSuccess = _data.DataImport("cfg.dmip_ResourceSync", dt, ModifyType.Update, GlobalData.CommonMenuID);
            if (!_updateSBTables.SaveSyncSuccess)
            {
                MessageBox.Show("更新表cfg.dmip_ResourceSync失败！" + "\n" + GetLastMessage());
            }
        }

        public bool SaveLBBData(string ggid, SBGGData sd)
        {
            _updateSBTables.SaveLBBSuccess = false;
            string q = @"select * from usrSBGGLBB(nolock) where GGID = " + ggid + "";
            DataTable dt = _data.GetDataTable(q, GlobalData.CommonMenuID);
            if (Utils.IsEmpty(dt))
            {
                MessageBox.Show("usrSBGGLBB表无法查询到公告，表可能被锁，请等会再试。。。");
                return false;
            }

            _chkNodeParents = frmLBModify.CheckedParents;
            int nodeLevel = frmLBModify.SelectedLBNode != null ? frmLBModify.SelectedLBNode.Level : -1;
            if (_chkNodeParents == null || _chkNodeParents.Keys.Count == 0 || nodeLevel == -1)
                return false;

            DataRow dr = dt.Rows[0];
            GlobalData.SetDefaultFieldsValue(dr, RunWay.Manual, false, GKBZ.已检验);//此处公开标识需要再确认;
            SetLBRecord(dr, _chkNodeParents);
            _updateSBTables.SaveLBBSuccess = _data.DataImport("usrSBGGLBB", dt, ModifyType.Update, GlobalData.CommonMenuID);
            if(!_updateSBTables.SaveLBBSuccess)
            {
                MessageBox.Show("usrSBGGLBB保存失败！"+ GetLastMessage());
            }
            return _updateSBTables.SaveLBBSuccess;
        }

        private void SetLBRecord(DataRow dr, Dictionary<string, string> lbs)
        {
            if (lbs == null || lbs.Keys.Count == 0)
                return;

            //先将所有层级的类别代码和名称置为空
            dr["YJGGBM"] = ""; dr["YJGGMC"] = "";
            dr["EJGGBM"] = ""; dr["EJGGMC"] = "";
            dr["SAJGGBM"] = ""; dr["SAJGGMC"] = "";
            dr["SIJGGBM"] = ""; dr["SIJGGMC"] = "";
            dr["WJGGBM"] = ""; dr["WJGGMC"] = "";
            dr["LJGGBM"] = ""; dr["LJGGMC"] = "";

            foreach (KeyValuePair<string, string> kp in lbs)
            {
                string level = _sbQueryhelper.GetFindString(kp.Value, "/", true);
                string lbmc = _sbQueryhelper.GetFindString(kp.Value, "/", false);
                string lbbm = kp.Key.Trim();
                switch (level)
                {
                    case "1": dr["YJGGBM"] = lbbm; dr["YJGGMC"] = lbmc; break;
                    case "2": dr["EJGGBM"] = lbbm; dr["EJGGMC"] = lbmc; break;
                    case "3": dr["SAJGGBM"] = lbbm; dr["SAJGGMC"] = lbmc; break;
                    case "4": dr["SIJGGBM"] = lbbm; dr["SIJGGMC"] = lbmc; break;
                    case "5": dr["WJGGBM"] = lbbm; dr["WJGGMC"] = lbmc; break;
                    case "6": dr["LJGGBM"] = lbbm; dr["LJGGMC"] = lbmc; break;
                }
            }
        }

        /// <summary>
        /// 更新表：usrSBGGB
        /// 需要更新的字段 INBBM IGSDM  GGBT GGRQ GGLY HASHCODE GKBZ
        /// GlobalData.SetDefaultFieldsValue(drNew, RunWay.Auto, true, GKBZ.已检验);
        /// </summary>
        public bool SaveSBGGBData(string ggid, SBGGData sd)
        {
            _updateSBTables.SaveSBGGBSuccess = false;
            string q = @"select * from usrSBGGB(nolock) where ID = " + ggid + " ";
            DataTable dt = _data.GetDataTable(q, GlobalData.CommonMenuID);
            if (Utils.IsEmpty(dt))
            {
                MessageBox.Show("usrSBGGB表无法查询到当前公告，表可能被锁，请等会再试。。。");
                return false;
            }

            if (UniqueFieldsCheck(sd))
            {
                MessageBox.Show("字段唯一性错误,请检查INBBM、IGSDM、GGBT、GGRQ不重复!");
                _updateSBTables.SaveSBGGBSuccess = false;
                return false;
            }
            DataRow dr = dt.Rows[0];
            GlobalData.SetDefaultFieldsValue(dr, RunWay.Manual, false, GKBZ.已检验);//此处公开标识需要再确认;
            dr["INBBM"] = sd.INBBM;
            dr["IGSDM"] = sd.IGSDM;
            dr["GGRQ"] = sd.GGRQ;
            dr["GGBT"] = sd.GGBT;
            dr["GKBZ"] = _sbQueryhelper.GetGKBZCode(sd.GKBZ);

            if (sd.GGLY != null && (!Utils.IsEmpty(sd.GGLY)))
            {
                dr["GGLY"] = sd.GGLY;
            }
            if (!Utils.IsEmpty(_exchangeMD5))
            {
                dr["HASHCODE"] = _exchangeMD5;
            }
            _sd.XGRY = dr["XGRY"].ToString();
            _sd.XGSJ = dr["XGSJ"].ToString();
            _updateSBTables.SaveSBGGBSuccess = _data.DataImport("usrSBGGB", dt, ModifyType.Update, this.MenuID);
            if (!_updateSBTables.SaveSBGGBSuccess)
            {
                MessageBox.Show("更新表usrSBGGB失败！"+ _data.GetLastMessage(this.MenuID));
            }
            return _updateSBTables.SaveSBGGBSuccess;
        }
        private Tuple<string, string> GetCode(string gpdm, Dictionary<string, StockCode> stockCode)
        {
            if (stockCode.Keys.Count == 0)
                return null;

            foreach (StockCode sc in stockCode.Values)
            {
                if (sc.Code == gpdm)
                {
                    Tuple<string, string> Tp = new Tuple<string, string>(sc.INBBM, sc.IGSDM);
                    return Tp;
                }
            }
            return null;
        }
        private void btnGGLB_Click(object sender, EventArgs e)
        {
            //frmLBModify frmLb = new frmLBModify(base.MenuID, txtGGLB.Text);
            _frmLBModify.ClickEvent += new LBClickDelegateHander(SetGGLBText);
            _frmLBModify.Show();
        }
        private void SetGGLBText(TreeNode chkNode)
        {
            txtGGLB.Text = chkNode.Text;
        }
        private void btnGGLB_Leave(object sender, EventArgs e)
        {
            txtGGLB.Text = frmLBModify.SelectedLBNode != null ? frmLBModify.SelectedLBNode.Text : "";
            _curNode = frmLBModify.SelectedLBNode;
        }
        private void txtGGLY_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtGGLY.SelectAll();
            }
        }
        private void txtGPDM_KeyDown(object sender, KeyEventArgs e)
        {
            txtGPDM.GetSelectedValueEvent -= new GetSelectedValueHander(SetGPDMText);
            txtGPDM.GetSelectedValueEvent += new GetSelectedValueHander(SetGPDMText);
        }

        private void SetGPDMText(string selectedValue)
        {
            if (Utils.IsEmpty(selectedValue))
                return;

            txtGPDM.Text = selectedValue;
            int index = txtGPDM.Text.IndexOf(" ");
            int index2 = txtGPDM.Text.IndexOf("-");
            txtSelectedCode.Text = index != -1 ?
                (txtGPDM.Text.Substring(0, index2).Replace(" ", "").Insert(index, "-")) : txtSelectedCode.Text;
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_index + 1 >= Convert.ToInt32(_ggCount))
            {
                MessageBox.Show("已经是最后一条了!");
                return;
            }
            MoveRecord(_index + 1);
        }

        private void MoveRecord(int index)
        {
            if (GridClickEvent != null)
            {
                GridClickEvent(index);
            }
            SetGGInfo();
        }
        private void btnPre_Click(object sender, EventArgs e)
        {
            if (_index - 1 < 0)
            {
                MessageBox.Show("已经是第1条数据了!");
                return;
            }
            MoveRecord(_index - 1);
        }

        /// <summary>
        /// 跳转到指定行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGGCount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            try
            {
                int index = Convert.ToInt32(_sbQueryhelper.GetFindString(txtGGCount.Text, "/", false));
                if (index - 1 > Convert.ToInt32(_ggCount) || index - 1 < 0)
                {
                    MessageBox.Show("给定序号超出范围，请重新设置！");
                    return;
                }
                MoveRecord(index - 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入格式非法，请重新输入！\n" + ex.Message + ex.StackTrace);
            }
        }
        private void frmSBGGModify_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Modifiers == Keys.Control)
            {
                e.Handled = true;
                btnSave.Focus();
                SaveData();
            }
        }
        private void txtGGBT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
                e.Handled = true;
            }
        }
        private void btnFirst_Click(object sender, EventArgs e)
        {
            MoveRecord(0);
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            MoveRecord(Convert.ToInt32(_ggCount) - 1);
        }

        private void frmSBGGModify_FormClosing(object sender, FormClosingEventArgs e)
        {
            boxGKBZ.SelectedIndex = -1;
            boxGKBZ.SelectedText = "";
            _sd = null;
            HideGGInfo();
        }

        private void btnSeeExistFile_Click(object sender, EventArgs e)
        {
            _sbQueryhelper.GetOssFile(SBDataCompare, _fileDownLoadPath);
        }
        private SBGGData _sbDataCompare = null;
        public SBGGData SBDataCompare
        {
            get { return _sbDataCompare; }
            set { _sbDataCompare = value; }
        }
        private void SetGGInfo()
        {
            if (btnExchangeFile.Text == "OSS文件")
                return;

            string md5 = _sd.MD5;
            string q = @"select a.*,b.LJGGBM, b.WJGGBM,b.SIJGGBM, b.SAJGGBM, b.EJGGBM,b.YJGGBM from usrSBGGB a, 
                            usrSBGGLBB b where a.HASHCODE = '" + md5 + "' and a.ID = b.GGID ";
            DataTable dt = _data.GetDataTable(q, this.MenuID);
            if (Utils.IsEmpty(dt))
                return;

            DataRow dr = dt.Rows[0];
            SBGGData sbdata = new SBGGData();
            string inbbm = dr["INBBM"].ToString();//此内码来源于：usrSBGGB，进而查找到对应的简称等字段
            string zqdm = _cacheData.stockCode.ContainsKey(inbbm) ?
                _cacheData.stockCode[inbbm].Code + "-" + _cacheData.stockCode[inbbm].Caption : "";
            txtZQDM_OSS.Text = zqdm;
            sbdata.ID = dr["ID"].ToString();
            sbdata.WJGS = ResourceTypes.GetExtension(dr["WJGS"].ToString()).ToUpper();
            sbdata.GGRQ = Convert.ToDateTime(dr["GGRQ"].ToString()).ToShortDateString().Replace("-", "/");
            sbdata.GGBT = dr["GGBT"].ToString();
            _sbDataCompare = sbdata;
            string gkdm = dr["GKBZ"].ToString();
            txtXGRY_OSS.Text = dr["XGRY"].ToString();
            txtXGRQ_OSS.Text = dr["XGSJ"].ToString();
            txtGGRQ_OSS.Text = dr["GGRQ"].ToString();
            txtGGBT_OSS.Text = dr["GGBT"].ToString();
            txtGKBZ_OSS.Text = ResourceTypes.GKBZData.GetGKBZ.ContainsKey(gkdm) ? gkdm + "-" + ResourceTypes.GKBZData.GetGKBZ[gkdm] : gkdm;
            string lbbm = _sbQueryhelper.GetGGLBBM(dr);
            txtGGLB_OSS.Text = "";
            if (Utils.IsEmpty(lbbm))
                return;

            q = "select LBMC from usrSBGGFLZYB where LBBM = '" + lbbm + "'";
            dt = _data.GetDataTable(q, this.MenuID);
            if (Utils.IsEmpty(dt))
                return;

            txtGGLB_OSS.Text = dt.Rows[0]["LBMC"].ToString();
        }
    }
}
