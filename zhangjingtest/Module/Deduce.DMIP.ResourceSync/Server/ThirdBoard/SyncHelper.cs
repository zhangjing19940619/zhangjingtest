using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Deduce.Common.Entity;
using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;
using Deduce.DMIP.ResourceManage;
using Deduce.DMIP.Sys.SysData;

namespace Deduce.DMIP.ResourceSync.Server
{
    public class SyncHelper : DataClass
    {
        protected string _tableName = "";
        protected string _dbName = "";        
        protected Dictionary<string, string> _md5s = new Dictionary<string, string>();
        object _lockObj = new object();
        protected StockCodeMatch _match = null;

        public SyncHelper()
        {
            base.MenuID = GlobalData.CommonMenuID;
            
        }

        protected bool _suspend = false;
        public bool Suspend
        {
            get { return _suspend; }
            set { _suspend = value; }
        }

        public Dictionary<string, ResourceSyncRule> LoadFormulas(string exType, string moduleID = "")
        {
            Dictionary<string, ResourceSyncRule> formulas = new Dictionary<string, ResourceSyncRule>();
            string q = "select ob_object_id,moduleID,formula from cfg.dmip_ArchiveRule(nolock) where 0=0 ";
            q += (Utils.IsEmpty(exType)) ? " " : " and exType='" + exType + "' ";
            q += (Utils.IsEmpty(moduleID)) ? " " : "  and moduleID='" + moduleID + "'";

            ////暂不考虑并发，后期需并发时，在此处作修改
            DataTable dt = _data.GetDataTable(q, base.MenuID);
            if (Utils.IsEmpty(dt))
            {
                _suspend = AutoHelper.Quit = true;
                return null;
            }
            BuildFormulas(dt, formulas);
            return formulas;
        }

        public void BuildFormulas(DataTable dt, Dictionary<string, ResourceSyncRule> formulas)
        {
            foreach (DataRow dr in dt.Rows)
            {
                string id = dr["ob_object_id"].ToString();
                if (Utils.IsEmpty(dr["formula"].ToString()))
                    continue;

                ResourceSyncRule rule = Utils.DesrializeFrom64String(new ResourceSyncRule(), dr["formula"].ToString());
                rule.CategoryID = dr["moduleID"].ToString();
                if (formulas.ContainsKey(id))
                {
                    formulas[id] = rule;
                    continue;
                }
                formulas.Add(id, rule);
            }
            ServiceSetting.TimeStamp = DateTime.Now.AddMinutes(-3);
        }

        /// <summary>
        /// 将需要同步的数据写至cfg.dmip_ResourceSync
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public virtual bool SaveSyncData(DataRow dr)
        {
            return false;
        }
        
        public virtual bool QueryMD5(string md5)
        {
            return false;
        }
        
        /// <summary>
        /// 加载需要生成同步信息的资源
        /// </summary>
        /// <returns></returns>
        public virtual DataTable LoadResource()
        {
            //string q = "select a.*,b.MediaSource,b.sourceName from (select top " + ServiceSetting.MaxTasks.ToString()
            //    + @" * from cfg.dmip_Resource(nolock) a where createTime>=GETDATE()-3 and tableName='" + _tableName+"'"
            //    + @"  order by a.createTime desc) a,cfg.dmip_GrabSite(nolock) b
            //   where a.sourceID=b.OB_OBJECT_ID and  not exists
            //   (select 0 from cfg. dmip_ResourceSync(nolock) where createTime>=GETDATE()-3 and ob_object_id=a.ob_object_id) ";

            //            string q = "select top " + ServiceSetting.MaxTasks.ToString()+ " a.*,b.MediaSource,b.sourceName from "      ----dingqj
            //+ @" (select * from cfg.dmip_Resource(nolock)a where createTime >= GETDATE() - 3 and tableName ='" + _tableName + "') a,"
            //+ @" cfg.dmip_GrabSite(nolock) b where a.sourceID = b.OB_OBJECT_ID"+
            //" and not exists(select 0 from cfg.dmip_ResourceSync(nolock) where createTime >= GETDATE() - 3 and ob_object_id = a.ob_object_id)";

            string q = "select top " + ServiceSetting.MaxTasks.ToString() + " a.*,a.MediaSrc as MediaSource,a.SrcName as sourceName,a.SrcName from "
+ @" (select * from cfg.dmip_Resource(nolock)a where createTime >= GETDATE() - 3 and tableName ='" + _tableName + "') a "+
" where not exists(select 0 from cfg.dmip_ResourceSync(nolock) where createTime >= GETDATE() - 3 and ob_object_id = a.ob_object_id)";

            return _data.GetDataTable(q, base.MenuID);
        }

        public DataTable LoadResourceSync()
        {
            //  string q = @"select a.*,b.MediaSource,b.sourceName,c.md5 from cfg.dmip_Resource(nolock) a,     ----dingqj
            //  cfg.dmip_GrabSite(nolock) b,(select r.* from cfg.dmip_ResourceSync(nolock) r where r.tableName='" 
            //  + _tableName + "' and r.status='3') c "
            //+ " where  a.sourceID=b.OB_OBJECT_ID and c.ob_object_id=a.ob_object_id";
            string q = @"select a.*,a.MediaSrc as MediaSource,a.SrcName as sourceName,a.SrcName,c.md5 from cfg.dmip_Resource(nolock) a,(select r.* from cfg.dmip_ResourceSync(nolock) r where r.tableName='"
  + _tableName + "' and r.status='3') c "
+ " where  c.ob_object_id=a.ob_object_id";

            //+ " AND a.title Like '%龙盛%'";            
            return _data.GetDataTable(q, base.MenuID);
        }

        public void CheckResourceSync(string  recordNum)
        {
            string q = "select top " + recordNum +
                " * from cfg.dmip_ResourceSync(nolock) where status=2 order by syncTime desc";
            DataTable dt=_data.GetDataTable(q, base.MenuID);
            if (Utils.IsEmpty(dt))
                return;

            List<string> md5s = new List<string>();
            foreach(DataRow dr in dt.Rows)
            {
                md5s.Add(dr["md5"].ToString());
            }
            dt = Response.GetDataTable("usrSBGGB", base.MenuID, "select * from usrSBGGB where ", "HASHCODE", md5s);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string md5 = dr["hashcode"].ToString();
                    if (md5s.Contains(md5))
                    {
                        md5s.Remove(md5);
                    }
                }
            }
            if (md5s.Count == 0)
                return;

            dt = Response.GetDataTable("cfg.dmip_ResourceSync", base.MenuID,
                "select * from cfg.dmip_ResourceSync where ", "md5", md5s);
            if (Utils.IsEmpty(dt))
                return;

            foreach(DataRow dr in dt.Rows)
            {
                dr["status"] = 0;
            }
            _data.DataImport("cfg.dmip_ResourceSync", dt, ModifyType.Update, base.MenuID, RunWay.Auto);
        }
        
        /// <summary>
        /// 加载需要上传到OSS的资源
        /// </summary>
        /// <returns></returns>
        public DataTable LoadToOssResource(SyncStatus status,string md5="")
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append(@"select top 100 b.ob_object_id,storePath,b.extension,b.opUser,c.sourceName ,b.Summary,b.Keywords,
            //   b.webSiteURL,b.code,b.createtime,b.resourceURL,b.extension,b.ggrq,
            //   a.* from cfg.dmip_Resource(nolock) b ,cfg.dmip_GrabSite(nolock) c"); ----dingqj
            sb.Append(@"select top 100 b.ob_object_id,storePath,b.extension,b.opUser,b.SrcName as sourceName,b.SrcName ,b.Summary,b.Keywords,
               b.webSiteURL,b.code,b.createtime,b.resourceURL,b.extension,b.ggrq,
               a.* from cfg.dmip_Resource(nolock) b ,");

            if (status == SyncStatus.Nothing || status == SyncStatus.Runing)
            {
                string temp = ((int)status).ToString();
                sb.Append(@"(select * from cfg.dmip_ResourceSync(nolock) a where a.status='" + temp 
                    + "' and a.tableName='"+ _tableName + "' and a.GGID is null ) a");
            }
            else if (status == SyncStatus.Success || status==SyncStatus.MD5Exists)
            {
                sb.Append(@"(select top 9999  * from cfg.dmip_ResourceSync(nolock) a where a.status='" 
               + ((int)status).ToString()+"' and a.tableName='"+ _tableName + "'");
                if (!Utils.IsEmpty(md5))
                {
                    sb.Append(" and a.md5='" + md5 + "' ");
                }
                sb.Append(" order by publishDate desc ) a");
            }
            sb.Append(@" where b.ob_object_id=a.ob_object_id and b.createTime>'" + DateTime.Now.AddDays(-3).ToString() + "' order by b.createTime desc ");
            //sb.Append(@" where b.ob_object_id=a.ob_object_id and b.sourceID=c.OB_OBJECT_ID and b.createTime>'"+ DateTime.Now.AddDays(-3).ToString()+"' order by b.createTime desc ");
            return _data.GetDataTable(sb.ToString(), base.MenuID);
        }

        /// 通过指定的文件扩展名返回文件的类型枚举值
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public int GetFileType(string extension)
        {
            return !(ResourceTypes.GetFileTypes.ContainsKey(extension)) ? -1 : ResourceTypes.GetFileTypes[extension];
        }

        public List<string> GetIDs(int max)
        {
            return _data.GetIDs(max, _tableName);
        }

        public string CreateObjectKey(string ggid = "", string extension = "", string ggrq = "",
            bool isDailyRuning=true)
        {
            if (Utils.IsEmpty(ggrq) && isDailyRuning)
            {
                ggrq = DateTime.Today.ToString("yyyy/MM/dd");
            }
            else if(isDailyRuning)
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
            return (Utils.IsEmpty(ggid) && Utils.IsEmpty(extension) && !isDailyRuning)
                ? _dbName + "/" + _tableName + "/" : _dbName + "/" + _tableName + "/" + ggrq + "/" + ggid + extension;
        }
        public void AppendHash(string md5)
        {
            lock (_lockObj)
            {
                try
                {
                    if (!_md5s.ContainsKey(md5))
                    {
                        _md5s.Add(md5, null);
                    }
                }
                catch (Exception ex)
                {
                    Utils.WriteLog("SyncHelper AppendMD5异常错误！" + ex.Message);
                }
            }
        }
    }
}