using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Deduce.Common.Entity;
using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;
using Deduce.DMIP.ResourceManage;
using Deduce.DMIP.Sys.SysData;

namespace Deduce.DMIP.ResourceSync
{
    public class HKStockSyncHelper : DataClass
    {
        string _tableName = "usrGSGGYWFWBGG";
        string _dbName = "JYPRIME";
        DataTable _dtSync = null;
        DataTable _dtGGFWB = null;
        DataTable _dtGSGG = null;
        StockCodeMatch _match = null;
        object _objLock = new object();
        string _menuID = "";
        AStockDragHelper _aStockDragHelper = new AStockDragHelper();
        Dictionary<string, string> _dicII = new Dictionary<string, string>();
        Dictionary<string, DataRow> _cacheDatas = new Dictionary<string, DataRow>();
        public HKStockSyncHelper()
        {
            _menuID = GlobalData.CommonMenuID;
        }
        public DataTable LoadResource()
        {
            //加载Resource表中TableName='usrGSGGYWFWBGG' and sourceID is not null && IsDmpResource = 1 且不存在于 ResourceSync表中的资源
            string q = @"Select a.*, b.MediaSource,b.sourceName From (Select Top " + ServiceSetting.MaxTasks.ToString() +
                    " * From cfg.dmip_Resource Where TableName = '" + _tableName +
                    "' and sourceID is not null and IsDmpResource = 1  Order By createTime DESC)a," +
                    "  cfg.dmip_GrabSite b where a.sourceID = b.ob_object_id and not exists " +
                    "  (Select 0 From cfg.dmip_ResourceSync Where ob_object_id = a.ob_object_id)";

            DataTable dt = _data.GetDataTable(q, _menuID);
            return dt;
        }
        public bool SaveSyncData(DataRow dr)
        {
            if (_dtSync == null)
                _dtSync = _data.GetStructure("cfg.dmip_ResourceSync", _menuID);
            DataTable dt = _dtSync.Clone();
            DataRow drNew = dt.NewRow();
            drNew["ob_object_id"] = dr["ob_object_id"];
            drNew["url"] = dr["resourceURL"];
            drNew["title"] = dr["title"];
            drNew["tableName"] = dr["tableName"];
            drNew["mediaSource"] = dr["mediaSource"];
            drNew["md5"] = Utils.GetFileMD5(dr["storePath"].ToString());
            drNew["createTime"] = dr["createtime"];
            drNew["publishDate"] = DateTime.Now;
            drNew["status"] = (int)SyncStatus.Nothing;
            drNew["txtFlag"] = (int)ToTextFlag.Original;
            string title = dr["title"].ToString();
            int index = title.LastIndexOf("&");
            if (index != -1)
            {
                int res = 0;
                string xh = title.Substring(index + 1).Trim();
                if (Int32.TryParse(xh, out res))
                {
                    drNew["INBBM"] = res; //存XH
                }
            }
            string relaCode = GetINBBMFromTitle(title);
            if (Utils.IsEmpty(relaCode))
                return false;
            drNew["relaCode"] = relaCode;
            dt.Rows.Add(drNew);
            return _data.DataImport("cfg.dmip_ResourceSync", dt, ModifyType.Insert, _menuID, RunWay.Auto);
        }

        public string GetINBBMFromTitle(string title)
        {
            string result = "";
            title = title.Replace("，", ",").Replace("（", "(").Replace("）", ")");
            int index1 = title.IndexOf("(");
            int index2 = title.IndexOf(")");
            if (index1 == -1 || index2 == -1)
                return "";

            string code = title.Substring(index1 + 1, index2 - index1 - 1);
            string[] codes = code.Split(',');
            string inbbm = "";
            string igsdm = "";
            for (int i = 0; i <= codes.Length - 1; i++)
            {
                _match.GetCode(codes[i], ref inbbm, ref igsdm);
                result += inbbm + "/";
            }
            result = Utils.IsEmpty(result) ? "" : result.Substring(0, result.Length - 1);
            return result;
        }
        public void SetSleep()
        {
            int maxSleep = 10000;
            Application.DoEvents();
            Thread.Sleep(maxSleep);
        }

        public Dictionary<string, TableDesign> LoadFormulas(string exType)
        {
            Dictionary<string, TableDesign> formulas = new Dictionary<string, TableDesign>();
            string q = "select ob_object_id,moduleID,formula from cfg.dmip_ArchiveRule where 0=0 ";
            q += (Utils.IsEmpty(exType)) ? " " : " and exType='" + exType + "' ";
            DataTable dt = _data.GetDataTable(q, _menuID);
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
        }
        public DataTable LoadToOssResource(SyncStatus status, ToTextFlag flag)
        {
            string q = @"select top 20 a.storePath, a.ggrq, a.extension, a.keywords, b.*                      
                          from cfg.dmip_Resource a, cfg.dmip_ResourceSync b 
                          where a.ob_object_id = b.ob_object_id and a.sourceID is not null and a.isDmpResource = 1 
                          and b.status = '" + (int)status + "' and b.tableName = '" + _tableName + "' and b.txtFlag = '" + (int)flag + "'";

            q += @" order by relaCode desc, INBBM asc";
            return _data.GetDataTable(q, _menuID);
        }
        public void LoadHisResource(DataRow dr)
        {
            string objID = dr["ob_object_id"].ToString();
            string q = @"select * from cfg.dmip_ResourceIDRela where syncID = '" + objID + "' and tableName= 'usrGGGSGG'";
            DataTable dt = _data.GetDataTable(q, _menuID);
            if (Utils.IsEmpty(dt))
                return;

            string extension = dr["extension"].ToString().ToUpper();
            string storePath = dr["storePath"].ToString();
            string txt = "";
            foreach (DataRow drNew in dt.Rows)
            {
                string recordID = drNew["recordID"].ToString();
                txt = ConvertHisTxt(recordID, extension, storePath, objID);
            }
            if (!Utils.IsEmpty(txt))
                ResetSyncStatus(dr, ToTextFlag.Success, SyncStatus.Success);
        }

        public string ConvertHisTxt(string recordID, string extension, string storePath, string objID)
        {
            string q = @"select * from usrGGGSGG where ID = '" + recordID + "'";
            DataTable dt = _data.GetDataTable(q, base.MenuID);
            if (Utils.IsEmpty(dt))
                return "";
            ToTextFlag flag = ToTextFlag.Original;
            string errorMsg = "";
            string txt = _aStockDragHelper.ConvertToText(extension, storePath, ref flag, ref errorMsg, objID, true);
            if (!Utils.IsEmpty(txt))
            {
                dt.Rows[0]["XXNR"] = txt;
                bool isSucc = _data.DataImport("usrGGGSGG", dt, ModifyType.Update, _menuID);
            }
            return txt;
        }
        public List<string> GetIDs(int max)
        {
            return _data.GetIDs(max, _tableName);
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

        public bool WriteTable(DataRow drResource, Dictionary<string, TableDesign> formulas, bool isAuto, ref DataTable dtMsg, string code = "")
        {
            if (formulas.Keys.Count == 0)
                return false;

            string errorMsg = "";
            bool saveToGGFWB = false;
            bool saveToGSGG = false;
            string ggfwb = "公司公告原文-非文本-港股 usrGSGGYWFWBGG";
            string objID = drResource["ob_object_id"].ToString();
            string ggrq = Convert.ToDateTime(drResource["ggrq"]).ToString(GlobalData.DateFormat);
            string title = drResource["title"].ToString();
            string media = drResource["keywords"].ToString();
            TableDesign tableRule = null;
            DataTable dtGGFWB = null;
            DataTable dtUniques = null;
            ModifyType mType = ModifyType.Insert;
            ToTextFlag flag = ToTextFlag.Original;
            bool isWriteGGGSGG = false;
            if (formulas.Keys.Contains(ggfwb))
            {
                tableRule = formulas[ggfwb];
                string tbName = ggfwb.Split(' ').Last();
                string caption = ggfwb.Split(' ').First();
                if (!_aStockDragHelper.MatchTable(drResource, tableRule.Rule, tableRule.RelaFields))
                    return false;

                dtGGFWB = MatchGGFWBFieldsRule(drResource, tableRule.Rules, isAuto);
                if (Utils.IsEmpty(dtGGFWB))
                    return false;

                for (int i = 0; i < dtGGFWB.Rows.Count; i++)
                {
                    dtUniques = tableRule.dtUnique;
                    _aStockDragHelper.LoadCache(dtUniques, dtGGFWB, tbName);
                    _cacheDatas = _aStockDragHelper.CacheDatas;
                    mType = _aStockDragHelper.CheckRepeat(tbName, dtUniques, dtGGFWB.Rows[i]) ? ModifyType.Update : ModifyType.Insert;
                    if (mType == ModifyType.Update)
                    {
                        string key = _aStockDragHelper.GetUniqueKeys(dtGGFWB.Rows[i], dtUniques, tbName);
                        if (_cacheDatas.Keys.Count != 0 && _cacheDatas.Keys.Contains(key))
                        {
                            dtGGFWB.Rows[i]["ID"] = _cacheDatas[key]["ID"];
                        }
                    }
                    DataTable dtNew = dtGGFWB.Clone();
                    DataRow dr = dtNew.NewRow();
                    dr.ItemArray = dtGGFWB.Rows[i].ItemArray;
                    dtNew.Rows.Add(dr);
                    saveToGGFWB = _data.DataImport(tbName, dtNew, mType, _menuID);
                    if (!saveToGGFWB)
                    {
                        string s1 = _data.GetLastMessage(_menuID).Replace("\r\n", "").Replace("¤", "");
                        dtNew.Rows[0]["GKBZ"] = 1;
                        saveToGGFWB = _data.DataImport(tbName, dtNew, mType, _menuID);
                        if (saveToGGFWB)
                            errorMsg += caption + tbName + "以未检验的方式入库成功:" + s1 + "\n";
                        else
                        {
                            string s2 = _data.GetLastMessage(_menuID).Replace("\r\n", "").Replace("¤", "");
                            errorMsg += caption + tbName + "入库失败:" + s1 + "\n" + s2 + "\n";
                        }
                    }
                    if (!saveToGGFWB)
                    {
                        if (!Utils.IsEmpty(errorMsg) && !isAuto)
                            GetErrorMsg(errorMsg, ggrq, title, code, media, ref dtMsg);
                        return saveToGGFWB;
                    }

                    _aStockDragHelper.SaveRelaIDTable(objID, "usrGSGGYWFWBGG", dtNew.Rows[0]["ID"].ToString());
                    saveToGSGG = WriteGGGSGG(drResource, dtNew.Rows[0], formulas, ggfwb, ref errorMsg, ref flag, ref isWriteGGGSGG, isAuto);
                    if (!saveToGSGG)
                    {
                        if (!Utils.IsEmpty(errorMsg) && !isAuto)
                            GetErrorMsg(errorMsg, ggrq, title, code, media, ref dtMsg);
                        return saveToGSGG;
                    }
                }
                if (isWriteGGGSGG)
                    ResetSyncStatus(drResource, flag, SyncStatus.Success);
                else
                    ResetSyncStatus(drResource, ToTextFlag.NotNecessary, SyncStatus.Success);
                if (!Utils.IsEmpty(errorMsg) && !isAuto)
                    GetErrorMsg(errorMsg, ggrq, title, code, media, ref dtMsg);
            }
            return saveToGGFWB && saveToGSGG;
        }

        public void GetErrorMsg(string errorMsg, string ggrq, string title, string relaCode, string media, ref DataTable dtMsg)
        {
            DataRow dr = dtMsg.NewRow();
            dr["修改时间"] = DateTime.Now;
            dr["日期"] = ggrq;
            dr["标题"] = title;
            dr["代码"] = relaCode;
            dr["媒体出处"] = media;
            dr["报错信息"] = errorMsg;
            dtMsg.Rows.Add(dr.ItemArray);
        }
        public bool WriteGGGSGG(DataRow drResource, DataRow drGGFWB, Dictionary<string, TableDesign> formulas, string ggfwb, ref string errorMsg, ref ToTextFlag flag, ref bool isWriteGGGSGG, bool isAuto)
        {
            bool isSucc = false;
            string storePath = drResource["storePath"].ToString();
            foreach (KeyValuePair<string, TableDesign> kv in formulas)
            {
                string tbName = kv.Key;
                string tableName = tbName.Split(' ').Last();
                string caption = tbName.Split(' ').First();
                TableDesign tableRule = kv.Value;
                if (tbName != ggfwb && _aStockDragHelper.CheckRelaTable(tableRule.RelaFields, "usrGSGGYWFWBGG"))
                {
                    if (!_aStockDragHelper.MatchTable(drGGFWB, tableRule.Rule, tableRule.RelaFields))
                        continue;

                    flag = ToTextFlag.Original;
                    string txt = _aStockDragHelper.ConvertToText(drGGFWB["WJGS"].ToString(), storePath, ref flag, ref errorMsg, drResource["ob_object_id"].ToString(), isAuto);
                    isWriteGGGSGG = true;
                    if (tableName != "usrGGGSGG")
                        continue;

                    DataTable dtGSGG = MatchGSGGFields(drGGFWB, tableRule.Rules, txt);
                    DataTable dtUniques = tableRule.dtUnique;
                    _aStockDragHelper.LoadCache(dtUniques, dtGSGG, tableName);
                    _cacheDatas = _aStockDragHelper.CacheDatas;
                    ModifyType mType = _aStockDragHelper.CheckRepeat(tableName, dtUniques, dtGSGG.Rows[0]) ? ModifyType.Update : ModifyType.Insert;
                    if (mType == ModifyType.Update)
                    {
                        string key = _aStockDragHelper.GetUniqueKeys(dtGSGG.Rows[0], dtUniques, tableName);

                        if (_cacheDatas.Keys.Count != 0 && _cacheDatas.Keys.Contains(key))
                        {
                            dtGSGG.Rows[0]["ID"] = _cacheDatas[key]["ID"];
                        }
                    }
                    isSucc = _data.DataImport(tableName, dtGSGG, mType, _menuID);
                    if (!isSucc)
                    {
                        string s1 = _data.GetLastMessage(_menuID).Replace("\r\n", "").Replace("¤", "");
                        errorMsg += caption + tableName + "入库失败：" + s1;
                        return isSucc;
                    }
                    _aStockDragHelper.SaveRelaIDTable(drResource["ob_object_id"].ToString(), tableName, dtGSGG.Rows[0]["ID"].ToString());
                }
            }
            return true;
        }
        public DataTable MatchGSGGFields(DataRow drGGFWB, List<DataFormula> rules, string txt)
        {
            if (_dtGSGG == null)
                _dtGSGG = _data.GetStructure("usrGGGSGG", _menuID);

            DataTable dtResult = _dtGSGG.Clone();
            DataRow dr = dtResult.NewRow();
            dr["ID"] = _data.GetIDs(1, "usrGGGSGG").First();
            dr["YWID"] = drGGFWB["ID"];
            dr["INBBM"] = drGGFWB["INBBM"];
            dr["XXFBRQ"] = drGGFWB["XXFBRQ"];
            dr["YJGGLB"] = drGGFWB["YJGGLB"];
            dr["GGBT"] = drGGFWB["WJM"];
            dr["NRLB"] = 1;
            dr["XXNR"] = txt;
            dr["RID"] = drGGFWB["ID"];
            GlobalData.SetDefaultFieldsValue(dr, RunWay.Auto, true, GKBZ.未检验);
            if (rules != null && rules.Count != 0)
            {
                BuildRule.RunExpression(rules, dr);
            }
            string secondDM = GetSL(_tableName + "_SL", drGGFWB["ID"].ToString(), "2");
            if (!Utils.IsEmpty(secondDM))
                SaveDataToSL("usrGGGSGG_SL", dr["ID"].ToString(), secondDM, "1");
            string thirdDM = GetSL(_tableName + "_SL", drGGFWB["ID"].ToString(), "3");
            if (!Utils.IsEmpty(thirdDM))
                SaveDataToSL("usrGGGSGG_SL", dr["ID"].ToString(), thirdDM, "2");
            dtResult.Rows.Add(dr);
            return dtResult;
        }
        public string GetSL(string tableName, string ID, string lb)
        {
            string q = @"select DM from " + tableName + " where ID = '" + ID + "' and LB = '" + lb + "'";
            DataTable dt = _data.GetDataTable(q, _menuID);
            if (Utils.IsEmpty(dt))
                return "";
            return dt.Rows[0][0].ToString();
        }

        /// <summary>
        /// 写表usrGSGGYWFWBGG数据，匹配对应规则
        /// cfg.dmip_ResourceSync->usrGSGGYWFWBGG
        /// </summary>
        public DataTable MatchGGFWBFieldsRule(DataRow drResource, List<DataFormula> Rules, bool isAuto)
        {
            if (_dtGGFWB == null)
                _dtGGFWB = _data.GetStructure("usrGSGGYWFWBGG", _menuID);

            DataTable dtResult = _dtGGFWB.Clone();
            Utils.TableAddColumns(dtResult, "OP_XXNR", "string");
            string relaCode = drResource["relaCode"].ToString();
            string[] relaCodes = relaCode.Split('/');
            for (int i = 0; i < relaCodes.Length; i++)
            {
                if (Utils.IsEmpty(relaCodes[i]))
                    continue;

                DataRow drNew = dtResult.NewRow();
                drNew["ID"] = _data.GetIDs(1, _tableName).First();
                drNew["INBBM"] = relaCodes[i];
                string igsdm = "";
                if (!isAuto)
                {
                    igsdm = GetIGSDM(drNew["INBBM"].ToString());
                    if (Utils.IsEmpty(igsdm))
                    {
                        MessageBox.Show("港股证券主表里没有对应的记录");
                        return null;
                    }
                }
                else
                    igsdm = _dicII.ContainsKey(relaCodes[i]) ? _dicII[relaCodes[i]] : "";
                drNew["IGSDM"] = igsdm;
                drNew["XXFBRQ"] = Utils.ConvertDate(drResource["ggrq"].ToString());
                drNew["MTCC"] = "香港交易所";
                drNew["JZRQ"] = Utils.ConvertDate(drResource["ggrq"].ToString());
                drNew["YYLB"] = 5;
                string type = drResource["extension"].ToString().ToLower();
                drNew["WJGS"] = ResourceTypes.GetFileTypes.Keys.Contains(type) ? ResourceTypes.GetFileTypes[type].ToString() : "";
                drNew["HASHCODE"] = drResource["md5"].ToString();
                GlobalData.SetDefaultFieldsValue(drNew, RunWay.Auto, true, GKBZ.已检验);
                drNew["OP_XXNR"] = "1";
                WriteGGFWBXH(drResource["title"].ToString(), ref drNew);
                WriteGGFWBXXNR(drResource, ref drNew);//写信息内容          
                if (Rules != null && Rules.Count != 0)
                {
                    BuildRule.RunExpression(Rules, drNew);
                }
                dtResult.Rows.Add(drNew);
            }
            return dtResult;
        }
        public string GetIGSDM(string inbbm)
        {
            string q = @"select IGSDM from usrGGZQZB(nolock) where INBBM = '" + inbbm + "' and SSZT <> 5";
            DataTable dt = _data.GetDataTable(q, _menuID);
            if (Utils.IsEmpty(dt))
                return "";
            return dt.Rows[0][0].ToString();
        }
        public void WriteGGFWBXXNR(DataRow drResource, ref  DataRow drNew)
        {
            if (drResource == null || drNew == null)
                return;

            string storePath = drResource["storePath"].ToString();
            if (Utils.IsEmpty(storePath))
                return;

            byte[] xxnr = File.ReadAllBytes(storePath);
            if (xxnr == null || xxnr.Length == 0)
                return;

            drNew["XXNR"] = xxnr;
        }

        public void WriteGGFWBXH(string title, ref DataRow drNew)
        {
            int index1 = title.IndexOf(")");
            if (index1 == -1)
                return;
            int index2 = title.IndexOf(",", index1);
            int index3 = title.IndexOf("&", index1);
            int index4 = title.LastIndexOf("&");
            int index5 = title.IndexOf("-", index1);
            int index6 = title.IndexOf("[", index1);
            int index7 = title.IndexOf("]", index1);
            drNew["XXBT"] = title;
            if (title.Contains("&") && index3 != -1 && index4 != -1 && index3 < index4)
            {
                drNew["XH"] = title.Substring(index4 + 1).Trim();
                drNew["XXXBT"] = title.Substring(index3 + 1, index4 - index3 - 1);
                drNew["XXBT"] = title.Substring(0, index3);
            }
            if (index2 != -1 && index3 != -1)
                drNew["WJM"] = title.Substring(index2 + 1, index3 - index2 - 1);
            else if (index2 != -1 && index3 == -1)
                drNew["WJM"] = title.Substring(index2 + 1);
            string name = "";
            string firstLB = "";
            string secondLB = "";
            string thirdLB = "";
            if (index5 != -1)
                name = title.Substring(index1 + 1, index5 - index1 - 1).Trim();
            else if (index2 != -1)
                name = title.Substring(index1 + 1, index2 - index1 - 1).Trim();

            firstLB = GetGGLB(name, "1", "");
            drNew["YJGGLB"] = firstLB;

            if (index6 != -1 && index7 != -1)
            {
                name = title.Substring(index6 + 1, index7 - index6 - 1).Trim();
                secondLB = GetGGLB(name, "2", firstLB);
                thirdLB = GetGGLB(name, "3", firstLB);
            }
            SaveDataToSL("usrGSGGYWFWBGG_SL", drNew["ID"].ToString(), drNew["INBBM"].ToString(), "1");
            if (Utils.IsEmpty(secondLB) && !Utils.IsEmpty(thirdLB))
                secondLB = thirdLB.Substring(0, 4);
            if (!Utils.IsEmpty(secondLB))
                SaveDataToSL("usrGSGGYWFWBGG_SL", drNew["ID"].ToString(), secondLB, "2");
            if (!Utils.IsEmpty(thirdLB))
                SaveDataToSL("usrGSGGYWFWBGG_SL", drNew["ID"].ToString(), thirdLB, "3");
        }
        public bool SaveDataToSL(string tableName, string ID, string dm, string lb)
        {
            DataTable dt = Utils.TableAddColumns("ID,LB,DM");
            string q = @"select * from " + tableName + " where ID = '" + ID + "' and LB = '" + lb + "'";
            DataTable dtOld = _data.GetDataTable(q, _menuID);
            bool isSucc = false;
            if (dtOld != null && dtOld.Rows.Count != 0)
            {
                isSucc = _data.DataImport(tableName, dtOld, ModifyType.Delete, _menuID);
                if (!isSucc)
                    return isSucc;
            }
            DataRow dr = dt.NewRow();
            dr["ID"] = ID;
            dr["LB"] = lb;
            dr["DM"] = dm;
            dt.Rows.Add(dr);
            isSucc = _data.DataImport(tableName, dt, ModifyType.Insert, _menuID);
            return isSucc;
        }
        public string GetGGLB(string name, string GGLBJB, string parentGGLB)
        {
            string q = @"select GGLBBM from usrGGGGLBJGB where GGLBZWMC = '" + name + "' and GGLBJB = '" + GGLBJB + "'";
            if (GGLBJB != "1")
            {
                q += " and FGGLBNBBM like'%" + parentGGLB + "%'";
            }
            DataTable dt = _data.GetDataTable(q, _menuID);
            if (Utils.IsEmpty(dt))
                return "";
            return dt.Rows[0][0].ToString();
        }

        public bool ResetSyncStatus(DataRow drResource, ToTextFlag flag, SyncStatus status)
        {
            string q = @"select * from cfg.dmip_ResourceSync where ob_object_id = '" + drResource["ob_object_id"].ToString() + "'";
            DataTable dt = _data.GetDataTable(q, _menuID);
            if (Utils.IsEmpty(dt))
                return false;

            dt.Rows[0]["GGID"] = drResource["GGID"].ToString();
            dt.Rows[0]["syncTime"] = DateTime.Now;
            dt.Rows[0]["status"] = (int)status;
            dt.Rows[0]["txtFlag"] = (int)flag;
            bool isSucc = _data.DataImport("cfg.dmip_ResourceSync", dt, ModifyType.Update, _menuID);
            return isSucc;
        }

        public void LoadGGZQZB()
        {
            lock (_objLock)
            {
                try
                {
                    if (_match == null || _dicII == null || _dicII.Count == 0)
                    {
                        string q = @"select INBBM, IGSDM, GPDM, ZQJC, ZWMC from usrGGZQZB(nolock) where SSZT <> 5";
                        DataTable dt = _data.GetDataTable(q, _menuID);
                        if (Utils.IsEmpty(dt))
                            return;

                        foreach (DataRow dr in dt.Rows)
                        {
                            string inbbm = dr["INBBM"].ToString();
                            string igsdm = dr["IGSDM"].ToString();
                            if (_dicII.ContainsKey(inbbm))
                                continue;
                            _dicII.Add(inbbm, igsdm);
                        }
                        _match = new StockCodeMatch(dt, true);
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }
        }
    }
}
