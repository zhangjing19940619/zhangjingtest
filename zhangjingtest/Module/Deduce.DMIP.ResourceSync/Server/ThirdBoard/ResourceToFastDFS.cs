using Deduce.Common.Utility;
using Deduce.DMIP.Business.WebAnalyze;
using Deduce.DMIP.Sys.OperateData;
using Deduce.DMIP.Sys.SysData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Deduce.DMIP.ResourceManage.Server
{
    public class FastDFS
    {
        public FastDFS(DataRow dr)
        {
            this.CreateTime = dr["createTime"].ToString();
            this.TableName = dr["tableName"].ToString();
            this.Status = 1;
        }
        public string CreateTime { get; set; }
        public string TableName { get; set; }

        /// <summary>
        /// 0 无需上传
        /// 1 需要上传到FastDFS中
        /// </summary>
        public int Status { get; set; }

    }
    public static class ResourceToFastDFS
    {
        static ConcurrentDictionary<string, FastDFS> _wheres = new ConcurrentDictionary<string, FastDFS>();
        static OperateData _data = OperateData.Instance;
        static ResourceToFastDFS()
        {
        }

        private static void InitData()
        {
            if (_wheres.Count > 0)
                return;

            string q = @"select * from (
             select CONVERT(varchar(10),createTime,120) as createTime,'cfg.dmip_Resource' as tableName
             from cfg.dmip_Resource(nolock)
             group by CONVERT(varchar(10), createTime, 120)
             union all
             select CONVERT(varchar(10),createTime,120) as createTime,'cfg.dmip_Resource_bak' as tableName
             from cfg.dmip_Resource_bak(nolock)
             group by CONVERT(varchar(10), createTime, 120)
             ) a order by a.createTime ";

            DataTable dt = _data.GetDataTable(q);
            if (Utils.IsEmpty(dt))
                return;

            foreach (DataRow dr in dt.Rows)
            {
                FastDFS dfs = new FastDFS(dr);
                if (_wheres.ContainsKey(dfs.CreateTime))
                    continue;

                _wheres.TryAdd(dfs.CreateTime, dfs);
            }
        }

        public static void Startup()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        string startDate = DateTime.Now.ToString(GlobalData.DateFormat);
                        string endDate = startDate + " 23:59:59.999";
                        QueryUploadResource(startDate, endDate, "cfg.dmip_Resource", false);
                        Random rand = new Random();
                        Thread.Sleep(rand.Next(2111, 9999));
                    }
                    catch (Exception ex)
                    {
                        Utils.WriteLog("ResourceToFastDFS 异常！" + ex.Message + ex.StackTrace);
                    }
                }
            });

            InitData();

            Task.Factory.StartNew(() =>
            {
                RunUpload(false);
            });

            Task.Factory.StartNew(() =>
            {
                RunUpload(true);                
            });
        }

        private static void RunUpload(bool isDesc)
        {
            while (true)
            {
                try
                {
                    ScanHistory(isDesc);
                    Random rand = new Random();
                    Thread.Sleep(rand.Next(60111, 99999));
                }
                catch (Exception ex)
                {
                    Utils.WriteLog("ScanHistory 异常！" + ex.Message + ex.StackTrace);
                }
            }
        }

        private static void ScanHistory(bool isDesc)
        {
            if (_wheres.Count == 0)
            {
                InitData();
            }

            List<FastDFS> vals = new List<FastDFS>();
            if (!isDesc)
            {
                vals = _wheres.Values.OrderBy(o => o.CreateTime).ToList();
            }
            else
            {
                vals = _wheres.Values.OrderByDescending(o => o.CreateTime).ToList();
            }

            foreach (FastDFS v in vals)
            {
                string startDate = v.CreateTime;
                string endDate=v.CreateTime + " 23:59:59.999";
                if (v.Status == 0 || startDate == DateTime.Now.ToString(GlobalData.DateFormat))
                    continue;
                
                QueryUploadResource(startDate, endDate,v.TableName,true);
            }
        }

        private static void QueryUploadResource(string startDate,string endDate,
            string tableName,bool md5IsNull)
        {
            if(Utils.IsEmpty(tableName))
            {
                tableName = "cfg.dmip_Resource";
            }
            if(Utils.IsEmpty(endDate))
            {
                endDate = startDate + " 23:59:59.999";
            }

            string q = "select top 999 a.ob_object_id,a.createTime,a.resMD5,a.storePath " +
                " from " + tableName + "(nolock) a " +
                " where a.createTime>='" + startDate + "' and a.createTime<='" + endDate + "' ";
            if(!md5IsNull)
            {
                q += " and a.resMD5 is not null ";
            }
            q+= " and not exists(select 0 from cfg.dmip_ResToFastDFS(nolock) b where b.createTime>='" +
                startDate + "' and b.createTime<='" + endDate + "' and " +
                " a.ob_object_id=b.ob_object_id) order by a.createTime desc ";
            DataTable dt = _data.GetDataTable(q);
            if(dt==null || dt.Rows.Count==0)
            {
                if (_wheres.ContainsKey(startDate))
                {
                    _wheres[startDate].Status = 0;
                }
                return;
            }

            WebKingCode.ToFastDFS(dt);
            QueryUploadResource(startDate, endDate,tableName, md5IsNull);
        }
    }
}
