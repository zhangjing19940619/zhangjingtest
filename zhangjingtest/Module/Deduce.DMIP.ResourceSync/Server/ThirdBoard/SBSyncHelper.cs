using Deduce.Common.Entity;
using Deduce.Common.Formula;
using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;
using Deduce.DMIP.ResourceManage;
using Deduce.DMIP.Sys.SysData;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;

namespace Deduce.DMIP.ResourceSync.Server
{
    public class SBSyncHelper : SyncHelper
    {
        DataTable _dtSync = null;
        DataTable _dtSyncToOss = null;
        DataTable _dtSBGGLBB = null;
        List<SBGGFLZYB> _defaultSBFL = new List<SBGGFLZYB>();
        private readonly ILogger _logger;
        object _objLock = new object();

        public SBSyncHelper(ILogger<SBSyncHelper> logger,bool isInit = true)
        {
            _logger = logger;
            _tableName = "usrSBGGB";
            _dbName = "JYNQ";
            if (!isInit)
                return;

            _dtSync = _data.GetStructure("cfg.dmip_ResourceSync", GlobalData.CommonMenuID);
            _dtSyncToOss = _data.GetStructure("usrSBGGB", GlobalData.CommonMenuID);
            _dtSBGGLBB = _data.GetStructure("usrSBGGLBB", GlobalData.CommonMenuID);
            string q = "select top 9999 HASHCODE FROM usrSBGGB(nolock) where HASHCODE is not null" +
                "  order by FBSJ desc";
            DataTable dt = _data.GetDataTable(q, GlobalData.CommonMenuID);
            AppendMD5(dt);
            _defaultSBFL.Add(SBGGFLZYB.GetDefault());//默认分类的代码，约定为000000
        }

        private bool BuildUniqueCode(DataRow dr)
        {
            string ggrq = Convert.ToDateTime(dr["ggrq"].ToString()).ToString(GlobalData.DateFormat);
            string q = "select * from usrSBGGB(nolock) where IGSDM=" + dr["igsdm"] + " and " +
                   " INBBM=" + dr["inbbm"].ToString() + " AND GGBT='" + dr["ggbt"] + "' and GGRQ='" + ggrq + "'";

            DataTable dt = _data.GetDataTable(q, base.MenuID);
            return !Utils.IsEmpty(dt);
        }

        public override DataTable LoadResource()
        {
            if (_suspend)
                return null;
            _logger.LogInformationWithProps(new { type = "三板资源同步"}, "LoadResource开始");
            LoadZQZB();
            return base.LoadResource();
        }

        public void LoadZQZB()
        {
            if (_suspend)
                return;

            lock (_objLock)
            {
                try
                {
                    string q = @"select INBBM, IGSDM, GPDM, ZQJC,ZWMC from  usrZQZB(nolock) where ZQSC = 81 ";
                    DataTable dt = _data.GetDataTable(q, base.MenuID);
                    //证券主表读取不可能为空，为空表示网络出现了异常，则强制退出
                    if (Utils.IsEmpty(dt))
                    {
                        _suspend = AutoHelper.Quit = true;
                        return;
                    }

                    if (_match == null)
                    {
                        _match = new StockCodeMatch(dt, false);
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }
        }

        public DataTable QuerySBGGLB()
        {
            //string q = "select * from usrSBGGB  WHERE ID NOT IN(SELECT GGID FROM usrSBGGLBB) order by FBSJ DESC ";
            string q = @"SELECT * FROM usrSBGGB(nolock) A WHERE NOT EXISTS (SELECT GGID FROM usrSBGGLBB(nolock) WHERE GGID = A.ID) ORDER BY FBSJ DESC";
            return _data.GetDataTable(q, GlobalData.CommonMenuID);
        }

        public Dictionary<string, List<SBGGFLZYB>> BuildFLZYB()
        {
            Dictionary<string, List<SBGGFLZYB>> gglb = new Dictionary<string, List<SBGGFLZYB>>();
            DataTable dt = _data.GetDataTable("select * from usrSBGGFLZYB(nolock) ", GlobalData.CommonMenuID);
            if (Utils.IsEmpty(dt))
            {
                _suspend = AutoHelper.Quit = true;
                return gglb;
            }
            DataRow[] rs = dt.Select("FQLBBM='0'");
            foreach (DataRow dr in rs)
            {
                string lbbm = dr["LBBM"].ToString();
                if (gglb.ContainsKey(lbbm))
                    continue;

                List<SBGGFLZYB> ids = new List<SBGGFLZYB>();
                SBGGFLZYB lb = new SBGGFLZYB(dr);
                ids.Add(lb);
                gglb.Add(lbbm, ids);
                BuildSBGGFLZYB(lbbm, dt, gglb);
            }
            return gglb;
        }
        private void BuildSBGGFLZYB(string fqlbbm, DataTable dt, Dictionary<string, List<SBGGFLZYB>> gglb)
        {
            DataRow[] rs = dt.Select("FQLBBM='" + fqlbbm + "'");
            if (rs == null || rs.Length == 0)
                return;

            foreach (DataRow dr in rs)
            {
                string lbbm = dr["LBBM"].ToString();
                if (gglb.ContainsKey(lbbm))
                    continue;

                List<SBGGFLZYB> ids = new List<SBGGFLZYB>();
                SBGGFLZYB lb = new SBGGFLZYB(dr);
                ids.Add(lb);
                ids.AddRange(gglb[fqlbbm]);
                gglb.Add(lbbm, ids);
                BuildSBGGFLZYB(lbbm, dt, gglb);
            }
        }

        public SyncStatus CheckCode(DataRow drResource, ref string inbbm, ref string igsdm,
            ref string code)
        {
            inbbm = drResource["inbbm"].ToString();
            igsdm = drResource["igsdm"].ToString();
            SyncStatus status = SyncStatus.Nothing;
            if (Utils.IsEmpty(inbbm) && Utils.IsEmpty(igsdm))
            {
                //string temp = (Utils.IsEmpty(drResource["code"].ToString())) ?
                //             drResource["title"].ToString() : drResource["code"].ToString();
                if (_match == null)
                {
                    LoadZQZB();
                }

                code = _match.GetCode(drResource["code"].ToString(), ref inbbm, ref igsdm,
                    drResource["title"].ToString());
                status = (Utils.IsEmpty(inbbm) && Utils.IsEmpty(igsdm)) ? SyncStatus.CodeNull : SyncStatus.Nothing;
                if (status == SyncStatus.Nothing)
                {
                    drResource["inbbm"] = inbbm;
                    drResource["igsdm"] = igsdm;
                }
            }
            return status;
        }

        public void CheckBackupResource(bool isAll)
        {
            string dir = (ServiceSetting.City == ServerCity.Nothing || ServiceSetting.City == ServerCity.ShTest
                   || ServiceSetting.City == ServerCity.HzTest) ? "" : GlobalData.BackupPath + CreateObjectKey("", "", "", false).Replace("/", @"\");
            if (Utils.IsEmpty(dir))
                return;

            string q = @"select ggrq from (select a.storePath,a.ggrq,b.GGID,a.extension from 
                cfg.dmip_Resource(nolock) a,cfg.dmip_ResourceSync(nolock) b where a.tableName='usrSBGGB' and b.status=2
                and b.ob_object_id=a.ob_object_id ";

            if (!isAll)
            {
                int preDays = ServiceSetting.MaxThreadCount * (-1);
                string startDate = DateTime.Now.AddDays(preDays).ToString(GlobalData.DateFormat);
                q += " and a.ggrq>='" + startDate + "' ";
            }
            q += "  ) a  group by ggrq order by a.ggrq desc";

            string path = Utils.CurrentPath + @"\resBackup.dat";
            List<string> dates = new List<string>();
            if (isAll && File.Exists(path))
            {
                dates = Utils.ReadWriteBinaryObject(dates, path, true);
                if (dates == null)
                {
                    dates = new List<string>();
                }
            }
            DataTable dtDate = _data.GetDataTable(q, GlobalData.CommonMenuID);
            if (Utils.IsEmpty(dtDate))
                return;

            foreach (DataRow dr in dtDate.Rows)
            {
                DateTime t = Convert.ToDateTime(dr["ggrq"].ToString());
                string ggrq = t.ToString(GlobalData.DateFormat);
                if (isAll && dates.Contains(ggrq))
                    continue;

                q = @"select a.storePath,a.ggrq,b.GGID,a.extension from cfg.dmip_Resource(nolock) a,
                cfg.dmip_ResourceSync(nolock) b where a.tableName='usrSBGGB' and b.status=2 and a.ggrq='" + ggrq + "' "
                + " and b.ob_object_id=a.ob_object_id ";
                DataTable dtRes = _data.GetDataTable(q, GlobalData.CommonMenuID);
                if (Utils.IsEmpty(dtRes))
                    continue;
                string rqDir = ggrq.Replace("-", @"\");
                foreach (DataRow res in dtRes.Rows)
                {
                    try
                    {
                        string newPath = dir + rqDir + @"\" + res["ggid"].ToString() + res["extension"].ToString();
                        if (File.Exists(newPath))
                            continue;

                        string oldPath = res["storePath"].ToString();
                        if (!File.Exists(oldPath))
                            continue;

                        File.Copy(oldPath, newPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(" Exception Error:" + ex.Message);
                    }
                }
                dates.Add(ggrq);
                if (isAll)
                {
                    Utils.ReadWriteBinaryObject(dates, path, false);
                }
            }

        }

    
        /// <summary>
        /// 推送资源中心kafka队列
        /// </summary>
        public bool SendToResourceCenterKafka(string dbName, string tableName, string resourceID, string recordID, DateTime ggrq, string source = "DMPSC")
        {
            string apiHost = GlobalData.GetSystemParameters(SystemParaType.AutoServiceWebApi)[0].ParaValue;
            //string apiHost = "http://10.106.0.235:8001";
            string kafkaHost = GlobalData.GetSystemParameters(SystemParaType.ResourceCenterKafkaHost)[0].ParaValue;

            RsKafkaMessageDto dto = new RsKafkaMessageDto()
            {
                schemaName = dbName,
                tableName = tableName,
                source = source,
                md5 = resourceID,
                releaseDate = ggrq.ToString(GlobalData.DateFormat),
                targetId = recordID,
            };
            string msg = ExtractJson.ToJson(dto);
            string requestJson = ExtractJson.ToJson(new
            {
                key = resourceID,
                brokerList = kafkaHost,
                topicName = "dmp_relation_to_rc",
                message = msg
            });

            string responseJson = new HttpHelper().GetHtml(new HttpItem()
            {
                Method = "POST",
                URL = apiHost + "/api/KafkaJump/Produce",
                Postdata = requestJson,
                ContentType = "application/json"
            }).Html;
            if (Utils.IsEmpty(responseJson))
            {
                return false;
            }

            ApiResponse response = ExtractJson.ToObject<ApiResponse>(responseJson);
            if (response.Code == 0)
            {
                return true;
            }

            return false;

        }

        public bool SyncFast2Oss(DataRow rowRes)
        {
            bool success = RcHelper.SyncFastDfsToOss(
                rowRes["GGID"].ToString(),
                rowRes["ob_object_id"].ToString(),
                _dbName, _tableName, 
                Convert.ToDateTime(rowRes["ggrq"]),
                rowRes["extension"].ToString().ToUpper().Trim('.'));
            return success;
        }

        /// <summary>
        /// 获取kafka消息
        /// </summary>
        /// <returns></returns>
        public RsKafkaMessageDto InitKafkaMessage(DataRow rowRes)
        {
            RsKafkaMessageDto dto = new RsKafkaMessageDto();
            dto.schemaName = _dbName;
            dto.tableName = _tableName;
            dto.source = "DMPSC";

            dto.md5 = rowRes["ob_object_id"].ToString();
            dto.releaseDate = Convert.ToDateTime(rowRes["ggrq"]).ToString(GlobalData.DateFormat);
            dto.targetId = rowRes["GGID"].ToString();
            return dto;
        }

        public void CheckResourceSyncStatus()
        {
            string q = @"select * from cfg.dmip_ResourceSync(nolock) where status=0 and GGID is not null 
                    and md5 is not null order by createtime desc";

            try
            {
                DataTable dt = _data.GetDataTable(q, GlobalData.CommonMenuID);
                if (Utils.IsEmpty(dt))
                    return;

                DataTable dtSync = dt.Clone();
                foreach (DataRow dr in dt.Rows)
                {
                    q = "select * from usrSBGGB(nolock) WHERE HASHCODE='" + dr["md5"].ToString() + "'";
                    DataTable dtSBGGB = _data.GetDataTable(q, GlobalData.CommonMenuID);
                    if (dtSBGGB != null && dtSBGGB.Rows.Count > 0)
                        continue;
                    DataRow drSync = dtSync.NewRow();
                    drSync.ItemArray = dr.ItemArray;
                    drSync["GGID"] = DBNull.Value;
                    dtSync.Rows.Add(drSync);
                }
                if (dtSync.Rows.Count > 0)
                {
                    _data.DataImport("cfg.dmip_ResourceSync", dtSync, ModifyType.Update, GlobalData.CommonMenuID);
                }
            }
            catch (Exception ex)
            {
                Utils.WriteLog("SBSyncHelper CheckResourceSyncStatus 异常！" + ex.Message + ex.StackTrace);
            }
        }

        public bool SaveSyncData(DataRow dr, bool isCalcMd5)
        {
            string code = "";
            string inbbm = "";
            string igsdm = "";
            SyncStatus status = CheckCode(dr, ref inbbm, ref igsdm, ref code);
            if (status == SyncStatus.CodeNull && !isCalcMd5)
                return false;

            DataTable dt = _dtSync.Clone();
            DataRow drNew = dt.NewRow();
            drNew["inbbm"] = inbbm;
            drNew["igsdm"] = igsdm;
            drNew["ob_object_id"] = dr["ob_object_id"];
            drNew["createTime"] = dr["createtime"];
            drNew["title"] = dr["title"];
            drNew["tableName"] = dr["tableName"];
            drNew["status"] = (int)status;
            drNew["md5"] = (isCalcMd5) ? Utils.GetFileMD5(dr["storePath"].ToString()) : dr["md5"];
            drNew["MediaSource"] = dr["MediaSource"];
            drNew["url"] = dr["resourceURL"];
            drNew["publishDate"] = DateTime.Now;

            ModifyType mType = (isCalcMd5) ? ModifyType.Insert : ModifyType.Update;
            if (status != SyncStatus.Success && mType == ModifyType.Update)
            {
                drNew["GGID"] = DBNull.Value;
            }

            dt.Rows.Add(drNew);
            if (!Utils.IsEmpty(code) && !Utils.IsEmpty(inbbm) && !Utils.IsEmpty(igsdm))
            {
                string q = "select * from cfg.dmip_Resource(nolock) where ob_object_id='"
                    + dr["ob_object_id"].ToString() + "'";
                DataTable dtResouce = _data.GetDataTable(q, base.MenuID);
                if (dtResouce != null && dtResouce.Rows.Count > 0)
                {
                    dtResouce.Rows[0]["code"] = code;
                    dtResouce.Rows[0]["inbbm"] = inbbm;
                    dtResouce.Rows[0]["igsdm"] = igsdm;
                    _data.DataImport("cfg.dmip_Resource", dt, ModifyType.Update, base.MenuID, RunWay.Auto);
                }
            }
            if (Utils.IsEmpty(inbbm) && mType == ModifyType.Update)
                return false;

            return _data.DataImport("cfg.dmip_ResourceSync", dt, mType, base.MenuID, RunWay.Auto);
        }

        public bool WriteSync(SyncStatus status, DataRow drResource, Dictionary<string, List<SBGGFLZYB>> cates)
        {
            string lbbm = "";
            if (status == SyncStatus.Success)
            {
                bool success = WriteTable(drResource, cates, out lbbm);
                if (!success)
                {
                    status = SyncStatus.Nothing;
                }
            }
            return WriteResourceSync(status, drResource, lbbm);
        }

        public bool WriteResourceSync(SyncStatus status, DataRow drResource, string lbbm)
        {
            if (status == SyncStatus.MD5Exists)
            {
                string q = "select * from cfg.dmip_ResourceSync(nolock) where md5='"
                    + drResource["md5"].ToString() + "'";
                DataTable dt1 = _data.GetDataTable(q, GlobalData.CommonMenuID);
                if (dt1 != null && dt1.Rows.Count <= 1)
                {
                    status = SyncStatus.Success;
                }
            }
            else if (status == SyncStatus.Success)
            {
                string q = "select * from cfg.dmip_ResourceSync(nolock) where GGID='" + drResource["GGID"] + "'";
                DataTable dt1 = _data.GetDataTable(q, GlobalData.CommonMenuID);
                if (dt1.Rows.Count > 1)
                    return true;
            }

            DataTable dtNew = _dtSync.Clone();
            DataRow drNew = dtNew.NewRow();
            dtNew.Rows.Add(drNew);

            drNew["syncTime"] = DateTime.Now;
            drNew["GGID"] = drResource["GGID"];
            drNew["LBBM"] = lbbm;
            if (status != SyncStatus.Success)
            {
                drNew["syncTime"] = DBNull.Value;
                drNew["GGID"] = DBNull.Value;
                drNew["LBBM"] = DBNull.Value;
            }
            drNew["status"] = (int)status;
            drNew["ob_object_id"] = drResource["ob_object_id"];
            drNew["inbbm"] = drResource["inbbm"];
            drNew["igsdm"] = drResource["igsdm"];
            if (Utils.IsEmpty(drResource["inbbm"].ToString()))
            {
                drNew["inbbm"] = DBNull.Value;
                drNew["igsdm"] = DBNull.Value;
            }
            drNew["title"] = drResource["title"];
            drNew["tableName"] = drResource["tableName"];
            drNew["md5"] = drResource["md5"];
            drNew["createTime"] = drResource["createTime"];
            drNew["publishDate"] = drResource["publishDate"];

            drNew["url"] = drResource["url"];
            drNew["MediaSource"] = drResource["MediaSource"];
            return _data.DataImport("cfg.dmip_ResourceSync", dtNew, ModifyType.Update, GlobalData.CommonMenuID, RunWay.Auto);
        }

        private bool AppendMD5(DataTable dt)
        {
            if (Utils.IsEmpty(dt))
                return false;

            foreach (DataRow dr in dt.Rows)
            {
                AppendHash(dr["hashcode"].ToString());
            }
            return true;
        }

        public override bool QueryMD5(string md5)
        {
            if (_md5s.ContainsKey(md5))
                return true;
            string q = "select HASHCODE FROM usrSBGGB(nolock) where HASHCODE='" + md5 + "'";
            DataTable dt = _data.GetDataTable(q, GlobalData.CommonMenuID);
            return AppendMD5(dt);
        }

        private bool WriteTable(DataRow drResource, Dictionary<string, List<SBGGFLZYB>> cates, out string lbbm)
        {
            lbbm = "";
            if (Utils.IsEmpty(drResource["inbbm"].ToString()) || Utils.IsEmpty(drResource["igsdm"].ToString()))
                return false;

            DataTable dt = _dtSyncToOss.Clone();
            DataRow drNew = dt.NewRow();
            GlobalData.SetDefaultFieldsValue(drNew, RunWay.Auto, true, GKBZ.已检验);
            string ggid = drResource["GGID"].ToString();
            drNew["ID"] = ggid;
            drNew["IGSDM"] = drResource["igsdm"];
            drNew["INBBM"] = drResource["inbbm"];
            drNew["GGRQ"] = drResource["ggrq"];
            drNew["GGBT"] = drResource["title"];
            //if (!Utils.IsEmpty(drResource["MediaSource"].ToString()))
            //{
            //    drNew["GGLY"] = drResource["MediaSource"];
            //}
            drNew["GGLY"] = "73";
            drNew["GGLJ"] = drResource["url"];
            drNew["WJGS"] = GetFileType(drResource["extension"].ToString());
            drNew["HASHCODE"] = drResource["md5"];
            if (!Utils.IsEmpty(drResource["opUser"].ToString()))
            {
                drNew["XGRY"] = drResource["opUser"];
            }
            bool exists = BuildUniqueCode(drNew);
            drNew["GKBZ"] = (exists) ? 1 : 3;
            dt.Rows.Add(drNew);

            bool success = _data.DataImport("usrSBGGB", dt, ModifyType.Insert, GlobalData.CommonMenuID, RunWay.Auto);
            if (success)
            {
                WriteLBB(drResource, ggid, cates, ref lbbm);
            }
            return success;
        }

        public Dictionary<string, List<SBGGFLZYB>> BuildCategory(DataRow drResource,
            Dictionary<string, List<SBGGFLZYB>> gglb, Dictionary<string, ResourceSyncRule> formulas)
        {
            Dictionary<string, List<SBGGFLZYB>> datas = new Dictionary<string, List<SBGGFLZYB>>();
            int repeat = 0;
            try
            {
                do
                {
                    foreach (ResourceSyncRule rule in formulas.Values)
                    {
                        ParaValueCollect pv = BindParameter.GetParameter(BindParameter.BuildPara(drResource));
                        Execute(pv, rule, datas, gglb);
                        if (datas.Count > 0)
                            break;
                    }
                    Thread.Sleep(29);
                    repeat++;
                }
                while (datas.Count == 0 && repeat < 3);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return datas;
        }

        public bool WriteLBB(DataRow drResource, string ggid, Dictionary<string, List<SBGGFLZYB>> cates, ref string lbbm)
        {
            if (cates.Count == 0)
            {
                cates.Add(_defaultSBFL[0].LBBM, _defaultSBFL);
            }
            DataTable dt = _dtSBGGLBB.Clone();
            List<string> ids = _data.GetIDs(cates.Count, "usrSBGGLBB");
            foreach (KeyValuePair<string, List<SBGGFLZYB>> lbs in cates)
            {
                lbbm = lbs.Key;
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);
                dr["ID"] = ids[0];
                ids.RemoveAt(0);
                dr["GGID"] = ggid;
                dr["SXRQ"] = DateTime.Now;
                GlobalData.SetDefaultFieldsValue(dr, RunWay.Auto, true, GKBZ.已检验);
                foreach (SBGGFLZYB zyb in lbs.Value)
                {
                    if (zyb.LBJB == "1")
                    {
                        dr["YJGGBM"] = zyb.LBBM;
                        dr["YJGGMC"] = zyb.LBMC;
                    }
                    else if (zyb.LBJB == "2")
                    {
                        dr["EJGGBM"] = zyb.LBBM;
                        dr["EJGGMC"] = zyb.LBMC;
                    }
                    else if (zyb.LBJB == "3")
                    {
                        dr["SAJGGBM"] = zyb.LBBM;
                        dr["SAJGGMC"] = zyb.LBMC;
                    }
                    else if (zyb.LBJB == "4")
                    {
                        dr["SIJGGBM"] = zyb.LBBM;
                        dr["SIJGGMC"] = zyb.LBMC;
                    }
                    else if (zyb.LBJB == "5")
                    {
                        dr["WJGGBM"] = zyb.LBBM;
                        dr["WJGGMC"] = zyb.LBMC;
                    }
                    else if (zyb.LBJB == "6")
                    {
                        dr["LJGGBM"] = zyb.LBBM;
                        dr["LJGGMC"] = zyb.LBMC;
                    }
                }
            }
            return _data.DataImport("usrSBGGLBB", dt, ModifyType.Insert, GlobalData.CommonMenuID, RunWay.Auto);
        }

        private void Execute(ParaValueCollect pv, ResourceSyncRule rule,
            Dictionary<string, List<SBGGFLZYB>> datas, Dictionary<string, List<SBGGFLZYB>> gglb)
        {
            if (rule.CategoryExpress == null || Utils.IsEmpty(rule.CategoryExpress.Caption))
                return;

            try
            {
                bool result = Convert.ToBoolean(FormulaLoader.Execute(rule.CategoryExpress, pv).ToString());
                if (!result)
                    return;

                List<SBGGFLZYB> ids = gglb[rule.CategoryID];
                if (datas.ContainsKey(rule.CategoryID))
                    return;

                datas.Add(rule.CategoryID, ids);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
    }

    /// <summary>
    /// 资源中心kafka消息体
    /// </summary>
    public class RsKafkaMessageDto
    {
        /// <summary>
        /// 资源中心md5
        /// </summary>
        public string md5 { get; set; }
        /// <summary>
        /// 下游数据库名
        /// </summary>
        public string schemaName { get; set; }
        /// <summary>
        /// 下游表名
        /// </summary>
        public string tableName { get; set; }
        /// <summary>
        /// 下游目标表id
        /// </summary>
        public string targetId { get; set; }
        /// <summary>
        /// 发布日期
        /// (格式：yyyy-MM-dd)
        /// </summary>
        public string releaseDate { get; set; }
        /// <summary>
        /// 来源方(DMPSC)
        /// </summary>
        public string source { get; set; }
    }
}
