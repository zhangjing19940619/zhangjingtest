using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Deduce.Common.Components;
using Deduce.Common.Entity;
using Deduce.Common.Formula;
using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;
using Deduce.DMIP.Business.FileAnalyze;
using Deduce.DMIP.ResourceManage;
using Deduce.DMIP.Sys.SysData;

namespace Deduce.DMIP.ResourceSync
{
    public class AStockSyncHelper : DataClass
    {
        string _tableName = "usrGSGGYWFWB";
        string _dbName = "JYPRIME";
        DataTable _dtSync = null;
        DataTable _dtFWB = null;
        DataTable _dtMedias = null;
        StockCodeMatch _match = null;
        object _objLock = new object();
        NoDocHelper _ndHelper = new NoDocHelper();
        AStockUploadHelper _aStockUploadHelper = new AStockUploadHelper();
        Dictionary<string, string> _medias = null;
        string _menuID = "";
        string _tempFiles = Utils.GetTempPath;
        public AStockSyncHelper()
        {
            _menuID = GlobalData.CommonMenuID;
        }
        public DataTable GetTableStruct(DataTable dt, string tableName)
        {
            if (dt != null)
                return dt;

            return dt = _data.GetStructure(tableName, _menuID);
        }
        public void LoadMediaInfo(ref Dictionary<string, string> _medias)
        {
            if (_medias == null)
                _medias = _ndHelper.GetMedias(ref _dtMedias);
        }
        public DataTable LoadResource()
        {
            //此处加载Resource表中TableName='usrGSGGYWFWB' and sourceID is not null && IsDmpResource = 1 的资源
            string sql = @"Select a.*, b.MediaSource,b.sourceName From   (Select Top " + ServiceSetting.MaxTasks.ToString() +
                    " * From cfg.dmip_Resource(nolock) a Where TableName = '" + _tableName + 
                    "' and sourceID is not null and IsDmpResource = 1  Order By a.createTime desc ) a," +
                    "  cfg.dmip_GrabSite(nolock) b where a.sourceID=b.OB_OBJECT_ID and  not exists " +
                    "   (Select 0 From cfg.dmip_ResourceSync(nolock)  Where ob_object_id = a.ob_object_id)";

            DataTable dt = _data.GetDataTable(sql, _menuID);
            string msg = _data.GetLastMessage(_menuID);
            return dt;
        }
        public string GetMediaInfo(string mediaCode)
        {
            LoadMediaInfo(ref _medias);
            if (Utils.IsEmpty(mediaCode) || _medias == null || _medias.Keys.Count == 0 || !_medias.Keys.Contains(mediaCode))
                return "";

            return _medias[mediaCode];
        }
        public bool SaveSyncData(DataRow dr, bool isCalcMd5)
        {
            string objID = dr["ob_object_id"].ToString();
            string code = dr["code"].ToString();
            string inbbm = dr["inbbm"].ToString();
            string igsdm = dr["igsdm"].ToString();
            string mediaSource = GetMediaSource(dr["sourceID"].ToString(), dr["keyWords"].ToString());
            string mediaInfo = GetMediaInfo(mediaSource);
            SyncStatus status = SyncStatus.Nothing;
            if (Utils.IsEmpty(code))//当代码为空的情况下才刷
            {
                status = CheckCode(ref dr, ref inbbm, ref igsdm, ref code, mediaInfo);
            }
            else if (code == "TTTT99")
            {
                inbbm = "2611";
                igsdm = "2272";
            }
            ModifyType mType = (isCalcMd5) ? ModifyType.Insert : ModifyType.Update;

            _dtSync = GetTableStruct(_dtSync, "cfg.dmip_ResourceSync");
            DataTable dtNew = _dtSync.Clone();
            DataRow drNew = dtNew.NewRow();
            drNew["inbbm"] = inbbm;
            drNew["igsdm"] = igsdm;

            drNew["ob_object_id"] = objID;
            drNew["createTime"] = dr["createtime"];
            drNew["title"] = dr["title"];
            drNew["tableName"] = dr["tableName"];
            drNew["status"] = (int)status;
            drNew["txtFlag"] = (int)ToTextFlag.Original; //txt转换初始状态
            drNew["md5"] = Utils.GetFileMD5(dr["storePath"].ToString());
            drNew["MediaSource"] = mediaSource;
            drNew["url"] = dr["resourceURL"];
            drNew["publishDate"] = DateTime.Now;
            drNew["RelaCode"] = inbbm;

            if (status != SyncStatus.Success && mType == ModifyType.Update)
            {
                drNew["GGID"] = DBNull.Value;
            }

            dtNew.Rows.Add(drNew);
            if (!Utils.IsEmpty(code) && !Utils.IsEmpty(inbbm) && !Utils.IsEmpty(igsdm))
            {
                string q = "select * from cfg.dmip_Resource(nolock) where ob_object_id = '" + dr["ob_object_id"].ToString() + "'";
                DataTable dtResouce = _data.GetDataTable(q, GlobalData.CommonMenuID);
                if (dtResouce != null && dtResouce.Rows.Count > 0)
                {
                    dtResouce.Rows[0]["code"] = code;
                    dtResouce.Rows[0]["inbbm"] = inbbm;
                    dtResouce.Rows[0]["igsdm"] = igsdm;
                    bool s = _data.DataImport("cfg.dmip_Resource", dtResouce, ModifyType.Update, GlobalData.CommonMenuID, RunWay.Auto);
                    string sss = _data.GetLastMessage(_menuID);
                }
            }
            if (Utils.IsEmpty(inbbm) && mType == ModifyType.Update)
                return false;

            return _data.DataImport("cfg.dmip_ResourceSync", dtNew, mType, _menuID, RunWay.Auto);
        }
        private string GetMediaSource(string sourceID, string keyWords)
        {
            string media = "";
            if (Utils.IsEmpty(sourceID))
            {
                if (!Utils.IsEmpty(keyWords) && keyWords.Contains(' '))
                    media = keyWords.Split(' ').First();
            }
            else
            {
                string q = @"Select mediaSource From cfg.dmip_GrabSite(nolock) where ob_object_id = '" + sourceID + "'";
                DataTable dt = _data.GetDataTable(q, _menuID);
                if (dt != null && dt.Rows.Count != 0)
                    media = dt.Rows[0]["mediaSource"].ToString();
            }
            return media;
        }
        public bool WriteResourceSync(SyncStatus status, DataRow drResource, string lbbm, ToTextFlag txtFlag)
        {
            _dtSync = GetTableStruct(_dtSync, "cfg.dmip_ResourceSync");
            DataTable dt = _dtSync.Clone();
            DataRow drNew = dt.NewRow();
            drNew["ob_object_id"] = drResource["ob_object_id"];
            drNew["syncTime"] = DateTime.Now;
            drNew["LBBM"] = lbbm;
            drNew["GGID"] = drResource["GGID"];
            drNew["LBBM"] = drResource["LBBM"];
            drNew["inbbm"] = drResource["inbbm"];
            drNew["igsdm"] = drResource["igsdm"];
            drNew["title"] = drResource["title"];
            drNew["tableName"] = drResource["tableName"];
            drNew["md5"] = drResource["md5"];
            drNew["createTime"] = drResource["createTime"];
            drNew["publishDate"] = drResource["publishDate"];
            drNew["url"] = drResource["url"];
            drNew["MediaSource"] = drResource["MediaSource"];
            drNew["RelaCode"] = drResource["RelaCode"];

            int falg = (int)txtFlag;
            int sta = falg == 1 ? 2 : (int)status;
            drNew["status"] = sta;
            drNew["txtFlag"] = falg;
            dt.Rows.Add(drNew);
            Utils.WriteLog("在 WriteResourceSync 里面：" + dt.Rows[0]["title"].ToString() + "\t" + "status=" + (int)sta + ";" + "flag=" + (int)falg);
            bool suc = _data.DataImport("cfg.dmip_ResourceSync", dt, ModifyType.Update, _menuID);
            string msg = _data.GetLastMessage(_menuID);
            return suc;
        }
        public string CreateObjectKey(string ggid, string extension, string ggrq)
        {
            if (Utils.IsEmpty(ggrq))
            {
                ggrq = DateTime.Today.ToString("yyyy/MM/dd");
            }
            else
            {
                try
                {
                    DateTime t = Convert.ToDateTime(ggrq);
                    ggrq = t.ToString("yyyy/MM/dd");
                }
                catch
                {
                    ggrq = DateTime.Today.ToString("yyyy/MM/dd");
                }
            }
            ggrq = ggrq.Replace("-", "/");
            return @_dbName + "/" + _tableName + "/" + ggrq + "/" + ggid + extension;
        }
        public void LoadZQZB()
        {
            lock (_objLock)
            {
                try
                {
                    if (_match == null)
                    {
                        string q = @"select INBBM, IGSDM, GPDM, ZQJC,ZWMC from  usrZQZB(nolock) ";//where ZQSC in(83,90) ";
                        DataTable dt = _data.GetDataTable(q, _menuID);
                        if (Utils.IsEmpty(dt))
                            return;

                        _match = new StockCodeMatch(dt, false);
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }
        }
        public DataTable LoadResourceSync()
        {
            string q = @"select a.*, b.MediaSource,b.sourceName,c.md5 from cfg.dmip_Resource(nolock) a,cfg.dmip_GrabSite(nolock) b,
                  (select r.* from cfg.dmip_ResourceSync(nolock) r where r.tableName='" + _tableName + "' and r.status='3' ) c "
                 + " where c.ob_object_id = a.ob_object_id and a.sourceID=b.OB_OBJECT_ID and a.sourceID is not null and a.IsDmpResource = 1 ";

            return _data.GetDataTable(q, _menuID);
        }
        public void SetSleep()
        {
            int hous = DateTime.Now.Hour;
            int maxSleep = 5511;//(hous > 15 && hous < 22) ? 5111 : 55111;
            Application.DoEvents();
            Thread.Sleep(maxSleep);
        }
        Dictionary<string, Dictionary<string, StockCode>> _cacheQueryData = new Dictionary<string, Dictionary<string, StockCode>>();

        public SyncStatus CheckCode(ref DataRow drResource, ref string inbbm, ref string igsdm, ref string code, string media)
        {
            inbbm = drResource["inbbm"].ToString();
            igsdm = drResource["igsdm"].ToString();
            SyncStatus status = SyncStatus.Nothing;
            if (Utils.IsEmpty(inbbm) && Utils.IsEmpty(igsdm))
            {
                string temp = (Utils.IsEmpty(drResource["code"].ToString()))
                    ? drResource["title"].ToString()
                    : drResource["code"].ToString();
                if (_match == null)
                {
                    LoadZQZB();
                }
                _aStockUploadHelper.LoadCacheQueryData(media, _menuID, ref  _cacheQueryData);
                List<StockInfo> infos = (List<StockInfo>)_aStockUploadHelper.ExecuCodeRule(temp, media, _cacheQueryData, true);
                //如果没有刷到代码，则将代码置为：TTTT99
                if (infos == null || infos.Count == 0)
                {
                    StockInfo siTemp = new StockInfo();
                    siTemp.SecuCode = "TTTT99";
                    siTemp.Inbbm = "2611";
                    siTemp.Igsdm = "2272";
                    infos.Add(siTemp);
                }
                StockInfo si = infos.First();
                code = si.SecuCode;
                inbbm = si.Inbbm;
                igsdm = si.Igsdm;
                status = (Utils.IsEmpty(inbbm) && Utils.IsEmpty(igsdm)) ? SyncStatus.CodeNull : SyncStatus.Nothing;
                if (status == SyncStatus.Nothing)
                {
                    drResource["inbbm"] = inbbm;
                    drResource["igsdm"] = igsdm;
                    drResource["code"] = code;
                }
            }
            return status;
        }
        public DataTable LoadToOssResource(SyncStatus status, string md5 = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select top 100 storePath,b.ob_object_id, b.extension, b.opUser, c.sourceName, b.Summary,b.Keywords,b.webSiteURL,
                b.code,b.createtime,b.resourceURL,b.extension,b.ggrq, b.webSetID, b.sourceID,c.sourceName, 
               a.* from cfg.dmip_Resource(nolock) b , cfg.dmip_GrabSite(nolock) c , ");

            if (status == SyncStatus.Nothing || status == SyncStatus.Runing)
            {
                string temp = ((int)status).ToString();
                sb.Append(@"(select * from cfg.dmip_ResourceSync(nolock) a where a.status='" + temp + "' and a.tableName='"
                    + _tableName + "') a");
            }
            else if (status == SyncStatus.Success || status == SyncStatus.MD5Exists)
            {
                sb.Append(@"(select top 9999  * from cfg.dmip_ResourceSync(nolock) a where a.status='" + ((int)status).ToString()
                + "' and  a.tableName='" + _tableName + "'");
                if (!Utils.IsEmpty(md5))
                {
                    sb.Append(" and a.md5='" + md5 + "' ");
                }
                sb.Append(" order by publishDate desc ) a");
            }
            sb.Append(@" where b.ob_object_id=a.ob_object_id and b.sourceID=c.OB_OBJECT_ID and b.IsDmpResource = 1
                        and a.txtFlag = '0'  order by b.createTime desc");

            DataTable dt = _data.GetDataTable(sb.ToString(), _menuID);
            string msg = _data.GetLastMessage(_menuID);
            return dt;
        }
        /// <summary>
        /// 加载需要转成txt格式的资源
        /// 已经同步到OSS，但是写入对应业务表的XXNR仍然为空，需要再转一次。
        /// </summary>
        public DataTable LoadConvertResource(SyncStatus status, ToTextFlag txtFlag)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select top 100 b.storePath,b.extension,b.opUser,b.Summary,b.Keywords,b.webSiteURL,
                b.code,b.createtime,b.resourceURL,b.extension,b.ggrq, b.webSetID, c.sourceName,
               a.* from cfg.dmip_Resource(nolock) b , cfg.dmip_GrabSite(nolock) c ,");

            if (status == SyncStatus.Success)
            {
                sb.Append(@" (select top 9999  * from cfg.dmip_ResourceSync(nolock) where status = '" + ((int)status).ToString() + "' and txtFlag = '" + (int)txtFlag + "' and tableName =  '" + _tableName + "' )  a ");
            }
            sb.Append(@" where b.sourceID=c.OB_OBJECT_ID and b.ob_object_id=a.ob_object_id  and b.IsDmpResource = 1 and b.tableName = '" + _tableName + "' order by b.createTime desc ");
            return _data.GetDataTable(sb.ToString(), _menuID);
        }
        public void RemoveConvertList(string objID)
        {
            if (Utils.IsEmpty(objID))
                return;

            string q = @"select * from cfg.dmip_ResourceSync(nolock) where ob_object_id = '" + objID + "' and status='2'";
            DataTable dt = _data.GetDataTable(q, _menuID);
            if (dt != null && dt.Rows.Count != 0)
            {
                dt.Rows[0]["txtFlag"] = (int)ToTextFlag.NotNecessary;
                bool s = _data.DataImport("cfg.dmip_ResourceSync", dt, ModifyType.Update, _menuID);
            }
        }
        public void ConvertHisTxt(DataRow drSource)
        {
            if (drSource == null)
                return;

            string id = drSource["ob_object_id"].ToString();
            string q = @"select a.tableName, a.recordID, b.ob_object_id, b.extension, b.storePath from cfg.dmip_ResourceIDRela(nolock) a," +
           "cfg.dmip_Resource(nolock) b  where b.ob_object_id = a.syncID and a.syncID = '" + id + "' and a.tableName != 'usrGSGGYWFWB' and a.tableName != 'cmdWJ'";
            DataTable dtConvert = _data.GetDataTable(q, _menuID);
            if (Utils.IsEmpty(dtConvert))
            {
                //表示不需要转换格式
                RemoveConvertList(id);
                return;
            }

            DataRow dr = dtConvert.Rows[0];
            string tableName = dr["tableName"].ToString();
            string recordID = dr["recordID"].ToString();
            string objID = dr["ob_object_id"].ToString();
            string extension = dr["extension"].ToString();
            string storePath = dr["storePath"].ToString();

            q = "select  * from " + tableName + " where  ID = '" + recordID + "'";
            DataTable dtTemp = _data.GetDataTable(q, _menuID);
            if (Utils.IsEmpty(dtTemp))
                return;

            if (dtTemp.Columns.Contains("XXNR"))
            {
                ToTextFlag flag = ToTextFlag.Original;
                string content = ConvertToText(extension.ToUpper(), objID, storePath, ref flag);
                if (!Utils.IsEmpty(content))
                {
                    dtTemp.Rows[0]["XXNR"] = content;
                    dtTemp.Rows[0]["GKBZ"] = "3";//重新写XXNR之后，公开标识置为公开
                    ResetSyncStatus(objID, ToTextFlag.Success, SyncStatus.Success);
                    bool success = _data.DataImport(tableName, dtTemp, ModifyType.Update, _menuID);
                    if (!success)
                    {
                        dtTemp.Rows[0]["GKBZ"] = "1";//以未公开状态入库
                        _data.DataImport(tableName, dtTemp, ModifyType.Update, _menuID);
                    }
                }
            }
        }
        public List<string> GetIDs(int max)
        {
            return _data.GetIDs(max, _tableName);
        }
        public Dictionary<string, TableDesign> LoadFormulas(string exType)
        {
            Dictionary<string, TableDesign> formulas = new Dictionary<string, TableDesign>();
            string q = "select ob_object_id,moduleID,formula from cfg.dmip_ArchiveRule(nolock) where 0=0 ";
            q += (Utils.IsEmpty(exType)) ? " " : " and exType='" + exType + "' ";
            DataTable dt = _data.GetDataTable(q, _menuID);
            string msg = _data.GetLastMessage(_menuID);
            if (Utils.IsEmpty(dt))
                return formulas;

            BuildFormulas(dt.Rows[0], ref formulas);
            return formulas;
        }
        public void BuildFormulas(DataRow dr, ref  Dictionary<string, TableDesign> formulas)
        {
            if (dr == null || Utils.IsEmpty(dr["formula"].ToString()))
                return;

            formulas = Utils.DesrializeFrom64String(new Dictionary<string, TableDesign>(), dr["formula"].ToString());
            ServiceSetting.TimeStamp = DateTime.Now.AddMinutes(-3);
        }
        public bool WriteSync(SyncStatus status, DataRow drResource, Dictionary<string, TableDesign> formulas)
        {
            string lbbm = "";
            ToTextFlag txtFlag = ToTextFlag.Original;
            if (status == SyncStatus.Success)
            {
                bool success = WriteTable(drResource, formulas, out lbbm, ref txtFlag);
            }
            return WriteResourceSync(status, drResource, lbbm, txtFlag);
        }
        public void GetILeagalData()
        {
            string q = "Select * From cfg.dmip_ResourceSync(nolock) Where status=0 And (txtFlag = 1 Or txtFlag = 3)";
            DataTable dt = _data.GetDataTable(q, _menuID);
            if (Utils.IsEmpty(dt))
                return;

            foreach (DataRow dr in dt.Rows)
            {
                string flag = dr["txtFlag"].ToString();

                string title = dr["title"].ToString();
                Utils.WriteLog("异常记录:" + title);
                dr["status"] = 2;
                dr["txtFlag"] = flag;
            }
            bool s = _data.DataImport("cfg.dmip_ResourceSync", dt, ModifyType.Update, _menuID);
        }
        /// <summary>
        /// 通过媒体出处的名称找到其对应的代码
        /// 现有的媒体出处数据格式为：1 中国证券报
        /// </summary>
        public string GetMediaCode(Dictionary<string, string> medias, string name)
        {
            if (medias == null || medias.Keys.Count == 0 || Utils.IsEmpty(name))
                return "";

            foreach (string k in medias.Values)
            {
                if (k.Split(' ').Last() == name)
                {
                    return k.Split(' ').First();
                }
            }
            return "";
        }
        public bool MatchTable(DataRow drResource, DataFormula dfTable, List<string> relaFields)
        {
            if (drResource == null || dfTable == null || dfTable.Expression == null)
                return false;

            var pv = new ParaValueCollect();
            pv = BuildParaValue(drResource, relaFields);
            bool match = Convert.ToBoolean(FormulaLoader.Execute(dfTable.Expression, pv));
            return match;
        }
        /// <summary>
        /// 写表usrGSGGYWFWB数据，匹配对应规则
        /// cfg.dmip_ResourceSync->usrGSGGYWFWB
        /// </summary>
        public DataTable MatchFWBFieldsRule(DataRow drResource, List<DataFormula> Rules, ref string relaCodes,
            ref string ggrq, ref string objID, ref  string resStorePath, GKBZ gkbz = GKBZ.已检验)
        {
            _dtFWB = GetTableStruct(_dtFWB, "usrGSGGYWFWB");
            DataTable dtResult = _dtFWB.Clone();
            Utils.TableAddColumns(dtResult, "OP_XXNR", "string");
            DataRow drNew = dtResult.NewRow();
            GlobalData.SetDefaultFieldsValue(drNew, RunWay.Manual, false, gkbz);
            drNew["ID"] = _data.GetIDs(1, "usrGSGGYWFWB").First();
            drNew["INBBM"] = drResource["inbbm"];
            drNew["IGSDM"] = drResource["igsdm"];
            drNew["XXFBRQ"] = Utils.ConvertDate(drResource["ggrq"].ToString());
            string mediaCode = drResource["MediaSource"].ToString();
            //写入非文本的媒体出处需要转成对应的中文名称
            LoadMediaInfo(ref _medias);
            drNew["MTCC"] = _medias.Keys.Count != 0 && _medias.Keys.Contains(mediaCode) ? _medias[mediaCode].Split(' ').Last() : "";
            drNew["JZRQ"] = Utils.ConvertDate(drResource["ggrq"].ToString());
            drNew["XXBT"] = drResource["title"];
            drNew["WJGS"] = GetFileTypeAndCodes(drResource["ob_object_id"].ToString(), ref relaCodes, ref ggrq, ref resStorePath);
            drNew["WJM"] = "";
            drNew["FBSJ"] = drResource["publishDate"];
            drNew["GGLJ"] = drResource["url"];
            drNew["HASHCODE"] = drResource["md5"];
            drNew["OP_XXNR"] = "1";
            WriteFWBXXNR(drResource, ref drNew);//写信息内容
            objID = drResource["ob_object_id"].ToString();
            if (Rules != null && Rules.Count != 0)
            {
                BuildRule.RunExpression(Rules, drNew);
            }
            dtResult.Rows.Add(drNew);
            return dtResult;
        }
        private void WriteFWBXXNR(DataRow drResource, ref  DataRow dr)
        {
            if (drResource == null || dr == null || !drResource.Table.Columns.Contains("storePath"))
                return;

            string storePath = drResource["storePath"].ToString();
            if (Utils.IsEmpty(storePath))
                return;

            byte[] xxnr = File.ReadAllBytes(storePath);
            if (xxnr == null || xxnr.Length == 0)
                return;

            dr["XXNR"] = xxnr;
        }
        /// <summary>
        /// 写表usrGSLSGG
        /// </summary>
        public DataTable MatchLSGGFieldsRule(DataRow drSource, List<DataFormula> Rules, string ggrq, string content, GKBZ gkbz = GKBZ.已检验)
        {
            DataTable dtResult = _data.GetStructure("usrGSLSGG", _menuID);
            DataRow drNew = dtResult.NewRow();
            GlobalData.SetDefaultFieldsValue(drNew, RunWay.Auto, false, gkbz);
            drNew["ID"] = _data.GetIDs(1, "usrGSLSGG").First();
            drNew["GGRQ"] = Utils.IsEmpty(ggrq) ? drSource["JZRQ"] : ggrq;
            drNew["XXBT"] = drSource["XXBT"];
            LoadMediaInfo(ref _medias);
            string media = GetMediaCode(_medias, drSource["MTCC"].ToString());
            if (!Utils.IsEmpty(media))
                drNew["MTCC"] = media;
            drNew["XXNR"] = content;
            drNew["FBSJ"] = drSource["FBSJ"];
            drNew["FWBID"] = drSource["ID"];
            if (Rules != null && Rules.Count != 0)
            {
                BuildRule.RunExpression(Rules, drNew);
            }
            dtResult.Rows.Add(drNew);
            return dtResult;
        }
        /// <summary>
        /// 写表usrGSGGYW
        /// </summary>
        public DataTable MatchGGYWFieldsRule(DataRow drSource, List<DataFormula> Rules, string content, GKBZ gkbz = GKBZ.已检验)
        {
            DataTable dtResult = _data.GetStructure("usrGSGGYW", _menuID);
            DataRow drNew = dtResult.NewRow();
            GlobalData.SetDefaultFieldsValue(drNew, RunWay.Auto, false, gkbz);
            drNew["ID"] = _data.GetIDs(1, "usrGSGGYW").First();
            drNew["INBBM"] = drSource["INBBM"];
            drNew["IGSDM"] = drSource["IGSDM"];
            drNew["MTCC"] = drSource["MTCC"];
            drNew["NRLB"] = drSource["NRLB"];
            drNew["XXBT"] = drSource["XXBT"];
            drNew["XXNR"] = content;
            drNew["JZRQ"] = drSource["JZRQ"];
            drNew["XH"] = 0;
            drNew["XXFBRQ"] = drSource["XXFBRQ"];
            if (Rules != null && Rules.Count != 0)
            {
                BuildRule.RunExpression(Rules, drNew);
            }
            dtResult.Rows.Add(drNew);
            return dtResult;
        }
        private string GetFileTypeAndCodes(string objId, ref string relaCodes, ref string ggrq, ref string resStorePath)
        {
            if (Utils.IsEmpty(objId))
                return "";

            string q = @"Select extension, summary, ggrq, storePath, sourceID, code, inbbm From cfg.dmip_Resource(nolock)  where ob_object_id = '" + objId + "'";
            DataTable dt = _data.GetDataTable(q, _menuID);
            if (Utils.IsEmpty(dt))
                return "";

            string type = dt.Rows[0]["extension"].ToString().ToLower();
            string sourceID = dt.Rows[0]["sourceID"].ToString();
            relaCodes = dt.Rows[0]["inbbm"].ToString();
            ggrq = dt.Rows[0]["ggrq"].ToString();
            resStorePath = dt.Rows[0]["storePath"].ToString();
            return ResourceTypes.GetFileTypes.Keys.Contains(type) ? ResourceTypes.GetFileTypes[type].ToString() : "";
        }
        private bool WriteTable(DataRow drResource, Dictionary<string, TableDesign> formula, out string lbbm, ref ToTextFlag txtFlag)
        {
            lbbm = "";
            if (Utils.IsEmpty(drResource["inbbm"].ToString()) || Utils.IsEmpty(drResource["igsdm"].ToString()) || formula.Keys.Count == 0)
                return false;

            bool saveToFWB = true;
            bool saveToTable = true;
            string msg = "";
            string fwb = "公司公告原文-非文本 usrGSGGYWFWB";
            string relaCodes = "";
            string ggrq = "";
            string objID = "";
            string recordID = "";
            string storePath = "";
            TableDesign tableRule = null;
            DataTable dtSource = null;
            DataTable dtUniques = null;
            ModifyType mType = ModifyType.Insert;
            //①先匹配非文本表
            if (formula.Keys.Contains(fwb))
            {
                tableRule = formula[fwb];
                if (!MatchTable(drResource, tableRule.Rule, tableRule.RelaFields))
                    return false;

                dtSource = MatchFWBFieldsRule(drResource, tableRule.Rules, ref relaCodes, ref ggrq, ref objID, ref storePath);
                dtUniques = tableRule.dtUnique;
                LoadCache(dtUniques, dtSource, fwb.Split(' ').Last());
                recordID = dtSource.Rows[0]["ID"].ToString();
                mType = CheckRepeat(fwb.Split(' ').Last(), dtUniques, dtSource.Rows[0], recordID) ? ModifyType.Update : ModifyType.Insert;
                string key = GetUniqueKeys(dtSource.Rows[0], dtUniques, "usrGSGGYWFWB");
                Utils.WriteLog("正在写非文本：\t" + dtSource.Rows[0]["XXBT"].ToString());
                Utils.WriteLog("操作状态：" + (int)mType + "\tuniqueKey:" + key);
                if (mType == ModifyType.Update)
                {
                    if (_cacheDatas.Keys.Count != 0 && _cacheDatas.Keys.Contains(key))
                    {
                        dtSource.Rows[0]["ID"] = _cacheDatas[key]["ID"];
                    }
                }
                if (!SaveDataToTable("usrGSGGYWFWB", dtSource, mType, _menuID, true, relaCodes, "1"))
                {
                    dtSource.Rows[0]["GKBZ"] = "1";
                    if (!SaveDataToTable("usrGSGGYWFWB", dtSource, mType, _menuID, true, relaCodes, "1"))
                    {
                        saveToFWB = false;
                        msg = _data.GetLastMessage(_menuID);
                    }
                }
            }
            if (!saveToFWB)
            {
                Utils.WriteLog("写非文本失败:\t" + dtSource.Rows[0]["XXBT"].ToString() + "\n" + msg);
                return false;
            }
            //写入cmdWJ
            string wjID = "";
            if (WriteWJ(drResource, recordID, ref wjID))
            {
                //写ID关联表-cmdWJ
                SaveRelaIDTable(drResource["ob_object_id"].ToString(), "cmdWJ", wjID);
            }
            //写日志信息
            Utils.WriteLog("写非文本成功:\t" + dtSource.Rows[0]["XXBT"].ToString() + "\n" + msg);
            //写ID关联表-usrGSGGYWFWB
            SaveRelaIDTable(drResource["ob_object_id"].ToString(), "usrGSGGYWFWB", dtSource.Rows[0]["ID"].ToString());
            DataRow drFWB = dtSource.Rows[0];
            saveToTable = WriteLSGGYWB(drResource, drFWB, formula, fwb, objID, storePath, ggrq, relaCodes, ref txtFlag, wjID);
            return saveToFWB && saveToTable;
        }

        /// <summary>
        /// 再写入原文和临时公告两张表
        /// </summary>
        public bool WriteLSGGYWB(DataRow drResource, DataRow drFWB, Dictionary<string, TableDesign> formula, string fwb,
            string objID, string storePath, string ggrq, string relaCodes, ref ToTextFlag flag, string wjID)
        {
            DataTable dtUniques = null;
            ModifyType mType = ModifyType.Insert;
            bool saveToTable = true;
            foreach (KeyValuePair<string, TableDesign> kv in formula)
            {
                TableDesign tableRule = null;
                string tableName = kv.Key;
                TableDesign tbRule = kv.Value;
                if (tableName != fwb && CheckRelaTable(tbRule.RelaFields, "usrGSGGYWFWB"))
                {
                    tableRule = formula[tableName];
                    string engName = tableName.Split(' ').Last();
                    if (MatchTable(drFWB, tableRule.Rule, tableRule.RelaFields))
                    {
                        string txt = ConvertToText(drFWB["WJGS"].ToString(), objID, storePath, ref flag);
                        DataTable dtNew = engName == "usrGSLSGG" ?
                           MatchLSGGFieldsRule(drFWB, tbRule.Rules, ggrq, txt) : engName == "usrGSGGYW" ?
                           MatchGGYWFieldsRule(drFWB, tableRule.Rules, txt) : null;

                        dtUniques = tableRule.dtUnique;
                        LoadCache(dtUniques, dtNew, engName);
                        string recordID = dtNew.Rows[0]["ID"].ToString();
                        mType = CheckRepeat(engName, dtUniques, dtNew.Rows[0], recordID) ? ModifyType.Update : ModifyType.Insert;
                        string key = GetUniqueKeys(dtNew.Rows[0], dtUniques, tableName);
                        Utils.WriteLog("正在写：\t" + engName + "\t" + dtNew.Rows[0]["XXBT"].ToString());
                        Utils.WriteLog("操作状态：" + (int)mType + "\tuniqueKey:" + key);
                        if (mType == ModifyType.Update)
                        {
                            if (_cacheDatas.Keys.Count != 0 && _cacheDatas.Keys.Contains(key))
                            {
                                dtNew.Rows[0]["ID"] = _cacheDatas[key]["ID"];
                            }
                        }
                        bool slave = engName == "usrGSLSGG" ? true : false;
                        if (!SaveDataToTable(tableName.Split(' ').Last(), dtNew, mType, _menuID, slave, relaCodes, "2"))
                        {
                            dtNew.Rows[0]["GKBZ"] = "1"; //如果GKBZ=3 公开 存入有问题，则尝试以=1未公开来存入
                            if (!SaveDataToTable(tableName.Split(' ').Last(), dtNew, mType, _menuID, slave, relaCodes, "2"))
                            {
                                saveToTable = false;
                                string msg = _data.GetLastMessage(_menuID);
                            }
                        }
                        if (saveToTable)
                        {
                            //写ID关联表，方便后续更新或删除
                            SaveRelaIDTable(drResource["ob_object_id"].ToString(), engName, dtNew.Rows[0]["ID"].ToString());
                            string fieldName = engName == "usrGSLSGG" ? "LSGGID" : engName == "usrGSGGYW" ? "GGYWID" : "";
                            UpdateWJ(wjID, fieldName, recordID);
                            Utils.WriteLog("写表：\t" + engName + " 成功\t" + key);
                        }
                    }
                }
            }
            return saveToTable;
        }
        /// <summary>
        /// 写入FireBall里面的cmdWJ表
        /// </summary>
        public bool WriteWJ(DataRow drSync, string fwbID, ref string wjID, bool isDrag = false, string relas = "")
        {
            DataTable dtResult = _data.GetStructure("cmdWJ", _menuID);
            DataRow drResult = dtResult.NewRow();

            string objID = drSync["ob_object_id"].ToString();
            string inbbm = drSync["inbbm"].ToString();
            string ID = _data.GetIDs(1, "cmdWJ").First();
            string FBRQ = drSync["ggrq"].ToString();
            string fbsj = DateTime.Now.ToString("G");
            string mtcc = "";
            string relaCode = "";
            if (isDrag)
            {
                mtcc = drSync["keywords"].ToString();
                mtcc = mtcc == "" ? "" : mtcc.Split(' ').First();
                relaCode = Utils.IsEmpty(relas) ? inbbm : relas.Replace("/", ",");
            }
            else
            {
                mtcc = drSync["mediaSource"].ToString();
                relaCode = drSync["relaCode"].ToString().Replace('/', ',').ToString();
            }

            string gpdm = relaCode;
            string xxbt = drSync["title"].ToString();
            string wjm = drSync["storePath"].ToString();
            string GS_SITE = drSync["websiteUrl"].ToString();
            string GG_SITE = drSync["resourceUrl"].ToString();
            string XGSJ = DateTime.Now.ToString("G");
            string XXFBGSGPDM = drSync["code"].ToString();

            drResult["ID"] = wjID = ID;
            drResult["FBRQ"] = FBRQ;
            drResult["FBSJ"] = fbsj;
            drResult["MTCC"] = mtcc;
            drResult["GPDM"] = gpdm;
            drResult["XXBT"] = xxbt;
            drResult["WJM"] = wjm;
            drResult["GS_SITE"] = GS_SITE;
            drResult["GG_SITE"] = GG_SITE;
            drResult["XGSJ"] = XGSJ;
            drResult["FBZT"] = "0";
            drResult["HTZT"] = "1"; ;
            drResult["XGRY"] = "11111";
            drResult["XGRYCHN"] = "后台自动抓取(DMP)";
            drResult["SCZT"] = "0";
            drResult["CLBZ"] = "1";
            drResult["CODETYPE"] = "1";
            drResult["XXFBGSGPDM"] = XXFBGSGPDM;
            drResult["WJCD"] = File.ReadAllBytes(wjm).Length;
            drResult["FWBID"] = fwbID;

            dtResult.Rows.Add(drResult);
            dtResult.Columns.Remove("XMLLJ");
            bool s = _data.DataImport("cmdWJ", dtResult, ModifyType.Insert, _menuID);
            return s;
        }
        /// <summary>
        /// 更新FireBall里面的cmdWJ表
        /// </summary>
        /// <returns></returns>
        public bool UpdateWJ(string wjID, string fieldName, string recordID)
        {
            if (Utils.IsEmpty(wjID) || Utils.IsEmpty(fieldName) || Utils.IsEmpty(recordID))
                return true;

            string q = "select * from cmdWJ(nolock) where ID = '" + wjID + "'";
            DataTable dtWJ = _data.GetDataTable(q, _menuID);
            if (null == dtWJ || 0 == dtWJ.Rows.Count)
                return true;

            dtWJ.Rows[0][fieldName] = recordID;
            dtWJ.Columns.Remove("XMLLJ");
            bool s = _data.DataImport("cmdWJ", dtWJ, ModifyType.Update, _menuID);
            string msg = _data.GetLastMessage(_menuID);
            return s;
        }
        public bool ResetSyncStatus(string objID, ToTextFlag flag, SyncStatus status)
        {
            string q = "select * from cfg.dmip_ResourceSync(nolock) where ob_object_id = '" + objID + "'";
            DataTable dt = _data.GetDataTable(q, _menuID);
            if (Utils.IsEmpty(dt))
                return false;

            dt.Rows[0]["status"] = (int)status;
            dt.Rows[0]["txtFlag"] = (int)flag;
            Utils.WriteLog("在 ResetSyncStatus 里面：" + dt.Rows[0]["title"].ToString() + "\t" + "status=" + (int)status + ";" + "flag=" + (int)flag);
            bool s = _data.DataImport("cfg.dmip_ResourceSync", dt, ModifyType.Update, _menuID);
            return s;
        }
        public string ConvertToText(string extension, string objID, string resStorePath, ref ToTextFlag flag)
        {
            string result = "";
            DataTable dt = new DataTable();
            string xmlPath = "";
            string fileTye = Utils.IsNumeric(extension) ? ResourceTypes.GetExtension(extension).ToUpper() : extension;
            if (fileTye == ".PDF")
            {
                string q = @"select * from cfg.dmip_ConvertHashCode(nolock) where resourceID = '" + objID + "'";
                dt = _data.GetDataTable(q, _menuID);
                //如果已经成功转存成xml，需要获取xml路径
                if (dt != null && dt.Rows.Count != 0)
                {
                    xmlPath = dt.Rows[0]["toPath"].ToString();
                    //调用xml转txt的程序
                    string content = Utils.ReadTxtOrCsv(xmlPath);
                    Dictionary<int, SourceData> tables = new Dictionary<int, SourceData>();
                    XmlExtract.Extract(ref content, tables, null);
                    if (!Utils.IsEmpty(content))
                    {
                        FileInfo fi = new FileInfo(xmlPath);
                        string store = _tempFiles + fi.Name.Replace(".xml", ".txt"); // ci.ToPath.Replace(ServiceSetting.StorePath, ServiceSetting.TxtPath).Replace(fi.Extension, ".txt");
                        if (!Utils.WriteTxtFile(store, content, true))
                        {
                            flag = ToTextFlag.Failure;
                            return "";
                        }
                        flag = ToTextFlag.Success;
                        return File.ReadAllText(store, Encoding.UTF8);
                    }
                }
                else
                {
                    q = @"select * from cfg.dmip_ConvertTempStore(nolock) where ob_object_ID = '" + objID + "'";
                    dt = _data.GetDataTable(q, _menuID);
                    //表示转换失败，无需再处理了-直接将Sync中的状态置为2-失败
                    if (dt != null && dt.Rows.Count != 0)
                    {
                        flag = ToTextFlag.Failure;
                        return "";
                    }
                }
            }
            else if (fileTye == ".DOC" || fileTye == ".DOCX")
            {
                try
                {
                    ConvertInfo ci = new ConvertInfo();
                    ci.FromPath = resStorePath;

                    WordToTxt.ConvertToTxt(ci, ref result);
                    flag = ToTextFlag.Success;
                    if (Utils.IsEmpty(result))
                    {
                        flag = ToTextFlag.Failure;
                        Utils.WriteLog("Word转换失败:\t" + Path.GetFileName(resStorePath));
                    }
                }
                catch (Exception ex)
                {
                    Utils.WriteLog("Word转成txt失败，标题：" + Path.GetFileName(resStorePath) + ex.Message + ex.StackTrace);
                }
            }
            return result;
        }
        /// <summary>
        /// 检查指定表是否与给定的关联字段相关
        /// 即指定表是否就是关联字段的对应表
        /// </summary>
        /// <param name="relaFields">关联字段集，以usrGSGGYWFWB-NRLB形式体现</param>
        /// <param name="tableName">指定表名称</param>
        /// <returns></returns>
        private bool CheckRelaTable(List<string> relaFields, string tableName)
        {
            if (relaFields.Count == 0 || Utils.IsEmpty(tableName))
                return false;

            foreach (string rela in relaFields)
            {
                if (rela.Split('-').First() == tableName)
                    return true;
            }

            return false;
        }
        /// <summary>
        /// 写ID关联表cfg.dmip_ResourceIDRela
        /// </summary>
        private void SaveRelaIDTable(string syncID, string tableName, string recordID)
        {
            string q = "select * from cfg.dmip_ResourceIDRela(nolock) where tableName = '" + tableName + "' and recordID = '" + recordID + "'";
            DataTable dtOld = _data.GetDataTable(q, _menuID);
            if (dtOld == null)
                return;

            if (dtOld.Rows.Count == 0)
            {
                DataTable dt = dtOld.Clone();
                DataRow dr = dt.NewRow();
                dr["ob_object_id"] = Utils.BuildID;
                dr["syncID"] = syncID;
                dr["tableName"] = tableName;
                dr["recordID"] = recordID;
                dt.Rows.Add(dr);
                bool s = _data.DataImport("cfg.dmip_ResourceIDRela", dt, ModifyType.Insert, _menuID);
                string msg = _data.GetLastMessage(_menuID);
            }
        }
        private bool SaveDataToTable(string tableName, DataTable dtSource, ModifyType modifyType, string menuID, bool hasSlave, string relaCodes, string lbType)
        {
            bool saveSlave = false;
            string msg = "";
            if (hasSlave)
            {
                saveSlave = SaveSlaveTable(tableName + "_SL", dtSource.Rows[0]["ID"].ToString(), menuID, relaCodes, lbType);
                msg = _data.GetLastMessage(menuID);
            }
            //如果写的是原文表，并且关联代码有多个，则需写多条记录，XH字段要排序0~N.            
            bool saveTable = _data.DataImport(tableName, dtSource, modifyType, menuID);
            msg = _data.GetLastMessage(menuID);
            return hasSlave ? saveTable && saveSlave : saveTable;
        }
        private bool SaveSlaveTable(string slaveTable, string ID, string menuID, string relaCode, string lbType)
        {
            if (Utils.IsEmpty(relaCode) || Utils.IsEmpty(menuID))
                return false;

            string q = "select * from " + slaveTable + "(nolock) where ID  = '" + ID + "'";
            DataTable dtOld = _data.GetDataTable(q, _menuID);
            if (dtOld != null && dtOld.Rows.Count != 0)
            {
                bool sucDel = _data.DataImport(slaveTable, dtOld, ModifyType.Delete, _menuID);
                //删除不成功，直接返回false
                if (!sucDel)
                {
                    Utils.WriteLog("删除从表不成功：\t" + slaveTable + "\t" + ID);
                    return false;
                }
            }
            //对于自动下载的公告，不会有多个关联代码
            DataTable dt = Utils.TableAddColumns("ID,LB,DM");
            DataRow dr = dt.NewRow();
            dr["ID"] = ID;
            dr["LB"] = lbType;
            dr["DM"] = relaCode;
            dt.Rows.Add(dr);
            bool s = _data.DataImport(slaveTable, dt, ModifyType.Insert, menuID);
            string sss = _data.GetLastMessage(menuID);
            if (!s)
            {
                Utils.WriteLog("写入从表失败：" + slaveTable + "\t" + ID + ":" + sss);
            }
            return s;
        }
        private bool CheckRepeat(string tableName, DataTable uniqueFields, DataRow dr, string recordID)
        {
            DataRow drOld = Exists(dr, uniqueFields, tableName);
            return !(null == drOld) || CheckExistIDRela(tableName, recordID);
        }
        public bool CheckExistIDRela(string tableName, string recordID)
        {
            if (Utils.IsEmpty(tableName) || Utils.IsEmpty(recordID))
                return false;

            string q = "select * from cfg.dmip_ResourceIDRela(nolock) where tableName = '" + tableName + "' and recordID = '" + recordID + "'";
            DataTable dt = _data.GetDataTable(q, _menuID);
            return dt != null && dt.Rows.Count != 0;
        }
        Dictionary<string, DataRow> _cacheDatas = new Dictionary<string, DataRow>();
        public DataRow Exists(DataRow drScr, DataTable dtUniques, string tableName)
        {
            string keys = GetUniqueKeys(drScr, dtUniques, tableName);
            return (!_cacheDatas.ContainsKey(keys)) ? null : _cacheDatas[keys];
        }
        public void LoadCache(DataTable dtUniques, DataTable dtPara, string tableName)
        {
            _cacheDatas.Clear();
            if (Utils.IsEmpty(dtUniques))
                return;

            DataTable dt = GetCacheData(dtPara, dtUniques, tableName);
            if (Utils.IsEmpty(dt))
                return;

            string keys = "";
            foreach (DataRow dr in dt.Rows)
            {
                keys = GetUniqueKeys(dr, dtUniques, tableName);
                if (Utils.IsEmpty(keys) || _cacheDatas.ContainsKey(keys))
                    continue;

                _cacheDatas.Add(keys, dr);
            }
        }
        private string GetUniqueKeys(DataRow dr, DataTable dtUniques, string tableName)
        {
            if (dr == null || dr.ItemArray.Length == 0)
                return null;

            string keys = "";
            foreach (DataRow fieldName in dtUniques.Rows)
            {
                string fieldname = fieldName[1].ToString();
                if (dr.Table.Columns.Contains(fieldname))
                {
                    string rqType = GetFieldType(tableName, fieldName[1].ToString());
                    string val = dr[fieldname].ToString();
                    if (rqType == "date" || rqType == "datetime")
                    {
                        val = Utils.ConvertDate(val);
                    }
                    keys += Utils.TrimDateZero(val);
                }
            }
            return keys;
        }
        public string GetFieldType(string tableName, string fieldName)
        {
            if (Utils.IsEmpty(tableName) || Utils.IsEmpty(fieldName))
                return "";

            string q = @"SELECT sys.objects.name AS TABLENAME ,
                                sys.columns.name AS COLNAME ,
                                sys.types.name AS DATATYPE
                        FROM    sys.objects
                                JOIN sys.columns ON sys.objects.object_id = sys.columns.object_id
                                JOIN sys.types ON sys.columns.user_type_id = sys.types.user_type_id
                        WHERE   sys.objects.type = 'U'
                                AND sys.objects.name = '" + tableName + "' AND sys.columns.name = '" + fieldName + "' ";
            DataTable dt = _data.GetDataTable(q, _menuID);
            if (dt != null && dt.Rows.Count != 0)
                return dt.Rows[0]["DATATYPE"].ToString();

            return "";
        }
        public ParaValueCollect BuildParaValue(DataRow dr, List<string> RelaFields)
        {
            var ps = new ParaValueCollect();
            if (RelaFields.Count == 0)
                return null;

            foreach (string tf in RelaFields)
            {
                string f = tf.Split('-').Last();
                if (dr.Table.Columns.Contains(f))
                    ps.SetValues(tf, dr[f].ToString());
            }

            return ps;
        }
        public DataTable GetCacheData(DataTable dtPara, DataTable dtUniques, string tableName)
        {
            DataRequest r = new DataRequest();
            r.TableName = tableName;
            string menuID = _menuID;
            r.MenuID = menuID;

            r.Script = "select  * from " + tableName + "(nolock) where ";
            r.DataTablePara = dtPara;
            r.ParaType = QueryParaType.Table;
            StringBuilder sb = new StringBuilder();
            foreach (DataRow fieldName in dtUniques.Rows)
            {
                sb.Append(fieldName[1] + "=" + fieldName[1] + ",");
            }
            string temp = sb.ToString();
            r.ParaName = temp.Substring(0, temp.Length - 1);
            r.DataStyle = DataStyle.DataTable;
            bool runing = true;
            Thread t = new Thread((ThreadStart)delegate()
            {
                r = _data.GetDataCollect(r);
                runing = false;
            });
            t.IsBackground = true;
            t.Start();
            while (runing)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }

            if (r == null)
            {
                string msg = _data.GetLastMessage(menuID);
                return null;
            }
            return r.GetDataTable;
        }

        /// <summary>
        /// 将代码+简称 转换成对应的内码存储
        /// </summary>
        public string ConvertCodeToInner(string codeShort)
        {
            if (Utils.IsEmpty(codeShort))
                return "";

            List<string> lstCodes = codeShort.Split('/').ToList();
            if (lstCodes.Count == 0)
                return "";

            string result = "";
            lstCodes.ForEach(temp =>
            {
                string[] codes = temp.Split('-');
                string dm = codes.First();
                string q = "select INBBM from usrZQZB(nolock) where GPDM= '" + dm + "'";
                if (codes.Length == 2)
                {
                    string jc = codes.Last();
                    q += " and replace(ZQJC, ' ', '') = '" + jc + "'";
                }
                DataTable dt = _data.GetDataTable(q, _menuID);
                //全角情况下未获取到，用半角再查询一次
                //if ((dt == null || dt.Rows.Count == 0) && codes.Length == 2)
                //{
                //    string jc = codes.Last();
                //    q += " and replace(ZQJC, ' ', '') = '" + Utils.ConvertEncode(jc, 3) + "'";
                //}
                if (dt != null && dt.Rows.Count != 0)
                    result += (dt.Rows[0]["INBBM"].ToString() + "/");
            });
            return Utils.IsEmpty(result) ? "" : result.Substring(0, result.Length - 1);
        }
    }
}
