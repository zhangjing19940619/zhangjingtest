using System;
using System.Collections.Generic;
using System.Data;
using Deduce.Common.Utility;
using Deduce.DMIP.Business.Components;

namespace Deduce.DMIP.ResourceSync
{
    class HtmlSyncHelper : DataClass
    {
        string _tableName = "usrGSGGYWFWB";
        string _dbName = "JYPRIME";
        Dictionary<string, string> _ids = new Dictionary<string, string>();
        DataTable _dtHtml = null;

        public HtmlSyncHelper()
        {
            _dtHtml = _data.GetStructure("cfg.dmip_ResNonConvert", GlobalData.CommonMenuID);
        }

        public Dictionary<string, string> ConvertEndIDs
        {
            get { return _ids; }
        }

        public bool QueryNonConvert(string id)
        {
            string q = "select ob_object_id from cfg.dmip_ResNonConvert(nolock) where ob_object_id='" + id + "'";
            DataTable dt = _data.GetDataTable(q, GlobalData.CommonMenuID);
            if (Utils.IsEmpty(dt))
                return false;

            if (!_ids.ContainsKey(id))
            {
                _ids.Add(id, null);
            }
            return true;
        }

        public void WriteHtmlConvert(string id, int status)
        {
            if (_dtHtml == null)
            {
                _dtHtml = _data.GetStructure("cfg.dmip_ResNonConvert", GlobalData.CommonMenuID);
            }

            if(!_ids.ContainsKey(id))
            {
                _ids.Add(id,null);
            }

            try
            {
                DataTable dt = _dtHtml.Clone();
                DataRow dr = dt.NewRow();
                dr["ob_object_id"] = id;
                dr["createTime"] = DateTime.Now;
                dr["status"] = status;
                dt.Rows.Add(dr);
                _data.DataImport("cfg.dmip_ResNonConvert", dt, ModifyType.Insert, GlobalData.CommonMenuID);
            }
            catch { }
        }

        /// <summary>
        /// 获取需要解析的pdf文件的路径及id(文件名)值
        /// </summary>
        /// <returns></returns>
        public DataTable GetResourceData(string rq)
        {
            string q = @"select XXFBRQ,ID,HASHCODE,XXBT from usrGSGGYWFWB(nolock)  
                    where INBBM in (select INBBM from usrZQZB(nolock) where ZQSC in (83,90) and ZQLB in (1,2)) 
                    and WJGS=1 and XXFBRQ='" + rq + "' " +
                   @" union all 
                  select * from (select top 5000 XXFBRQ,ID,HASHCODE,XXBT from usrGSGGYWFWB(nolock)  where 
                  INBBM in (select INBBM from usrZQZB(nolock) where ZQSC in (83,90) and ZQLB in (1,2)) 
                   and WJGS=1  order by XXFBRQ desc) a ";
            return _data.GetDataTable(q, GlobalData.CommonMenuID);
        }

        public string GetDmpResource(string md5)
        {
            string q = "select storePath from cfg.dmip_Resource(nolock) where resMD5='" + md5 + "'";
            DataTable dt = _data.GetDataTable(q, GlobalData.CommonMenuID);
            return Utils.IsEmpty(dt) ? "" : dt.Rows[0][0].ToString();
        }

        private void LoadCache(string where)
        {
            string q = @"select top 99999 ob_object_id from cfg.dmip_ResNonConvert(nolock) " + where;
            DataTable dt = _data.GetDataTable(q, GlobalData.CommonMenuID);
            if (Utils.IsEmpty(dt))
                return;

            foreach (DataRow dr in dt.Rows)
            {
                string id = dr[0].ToString();
                if (_ids.ContainsKey(id))
                    continue;

                _ids.Add(id, null);
            }
        }

        public DataTable GetXXFBRQ(string publishDate,bool isInit)
        {
            string q = "";
            if (!isInit)
            {
                LoadCache("");
            }
            else
            {
                q = "select rq from cfg.dmip_ResNonConvert group by  rq order by rq desc ";
                DataTable dtRq = _data.GetDataTable(q, GlobalData.CommonMenuID);

                foreach (DataRow drRq in dtRq.Rows)
                {
                    string rq = drRq[0].ToString();
                    string where = "";
                    if (Utils.IsEmpty(rq))
                    {
                        where = " where rq is null ";
                    }
                    else
                    {
                        where = " where rq='" + rq + "' ";
                    }
                    LoadCache(where);
                }
            }

            if(Utils.IsEmpty(publishDate))
            {
                publishDate = "2017-07-01";
            }
            q = @"select XXFBRQ from usrGSGGYWFWB(nolock) where INBBM
               in (select INBBM from usrZQZB(nolock) where ZQSC in (83,90) and ZQLB in (1,2)) 
               and WJGS = 1 and XXFBRQ>= '"+publishDate+"' group by XXFBRQ order by XXFBRQ ";

            return _data.GetDataTable(q, GlobalData.CommonMenuID);
        }

        public string CreateObjectKey(string ggid = "", string extension = "", string ggrq = "",
            bool isDailyRuning = true)
        {
            if (Utils.IsEmpty(ggrq) && isDailyRuning)
            {
                ggrq = DateTime.Today.ToString("yyyy/MM/dd");
            }
            else if (isDailyRuning)
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

    }
}
